using System.Collections.Generic;
using ImmersiveTrader.Components;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

public static class NpcPrefabRegistry
{
    private static bool _registered;

    // Uses only vanilla Valheim prefabs. If a source prefab is unavailable after a game
    // update, that trader safely falls back to Haldor instead of breaking registration.
    private static readonly Dictionary<string, (string Prefab, float Scale)> Looks = new()
    {
        ["midka"] = ("Hildir", 1.00f),
        ["troldad"] = ("Troll", 0.42f),
        ["grimvald"] = ("Haldor", 1.00f),
        ["rudy_warg"] = ("Dverger", 1.00f),
        ["mokra_dzika"] = ("BogWitch", 1.00f),
        ["encek"] = ("Draugr", 0.95f),
        ["hrothgar"] = ("Fenring", 0.82f),
        ["ylva_frost"] = ("DvergerMage", 0.95f),
        ["bjarki_goldtooth"] = ("Goblin", 1.05f),
        ["ragnar_turnipson"] = ("GoblinBrute", 0.78f),
        ["cmok"] = ("DvergerMage", 0.90f),
        ["grelka"] = ("Dverger", 0.95f),
        ["spalony_zenek"] = ("Haldor", 1.00f),
        ["skjold_cinderborn"] = ("Hildir", 1.00f)
    };

    // Reserved human/player-style visual source. Do not clone Player directly into the
    // live roster yet: Player carries input, inventory, camera and networking behaviour.
    // The next visual layer can copy only its humanoid/VisEquipment presentation onto a
    // neutral NPC shell while keeping TraderNpc and NPC networking authoritative.
    public const string PlayerStyleVisualSource = "Player";

    public static void Register()
    {
        if (_registered) return;

        foreach (var trader in TraderRegistry.Traders)
        {
            if (trader.IsLegendary) continue;

            var look = Looks.TryGetValue(trader.Id, out var selected) ? selected : ("Haldor", 1f);
            string source = PrefabManager.Instance.GetPrefab(look.Item1) != null ? look.Item1 : "Haldor";

            var prefab = PrefabManager.Instance.CreateClonedPrefab($"ImmersiveTrader_NPC_{trader.Id}", source);
            if (prefab == null) continue;

            MakePassive(prefab);
            prefab.transform.localScale = Vector3.one * look.Item2;

            var interaction = prefab.GetComponent<TraderNpc>() ?? prefab.AddComponent<TraderNpc>();
            interaction.TraderId = trader.Id;

            // Creature-derived visuals keep their Character/AI only for model animation.
            // Interaction is handled by a dedicated NPC hover anchor, matching the
            // reliable trader-style path used by Hildir/BogWitch rather than rewriting
            // Player's private hover state.
            if (prefab.GetComponent<Character>() != null)
                PrepareCreatureTrader(prefab, interaction);

            PrefabManager.Instance.AddPrefab(prefab);
        }

        RegisterJackie();
        _registered = true;
    }

    private static void MakePassive(GameObject prefab)
    {
        var vanillaTrader = prefab.GetComponent<Trader>();
        if (vanillaTrader != null) Object.DestroyImmediate(vanillaTrader);

        var npcTalk = prefab.GetComponent<NpcTalk>();
        if (npcTalk != null) Object.DestroyImmediate(npcTalk);

        // Creature-derived trader prefabs must keep their native AI component.
        // Valheim's Character/Humanoid, animation and EnemyHud paths expect a valid
        // creature layout. Stripping MonsterAI/AnimalAI left half-creature prefabs
        // and caused EnemyHud.UpdateHuds NullReferenceExceptions at runtime.
        // Aggression is disabled instead of removing AI.
        var monsterAi = prefab.GetComponent<MonsterAI>();
        if (monsterAi != null)
        {
            monsterAi.m_viewRange = 0f;
            monsterAi.m_viewAngle = 0f;
            monsterAi.m_hearRange = 0f;
            monsterAi.m_alertRange = 0f;
            monsterAi.m_fleeIfNotAlerted = false;
        }

        var animalAi = prefab.GetComponent<AnimalAI>();
        if (animalAi != null)
        {
            animalAi.m_viewRange = 0f;
            animalAi.m_viewAngle = 0f;
            animalAi.m_hearRange = 0f;
        }

        var character = prefab.GetComponent<Character>();
        if (character != null)
        {
            character.m_faction = Character.Faction.Players;
            character.m_name = string.Empty;
        }

        var tameable = prefab.GetComponent<Tameable>();
        if (tameable != null) Object.DestroyImmediate(tameable);
    }

    private static void PrepareCreatureTrader(GameObject prefab, TraderNpc owner)
    {
        // Do not put Interactable/Hoverable proxies on native creature hitboxes.
        // Valheim resolves those hitboxes through Character and that competes with the
        // normal NPC hover path. One explicit interaction volume is deterministic.
        foreach (var proxy in prefab.GetComponentsInChildren<TraderInteractionProxy>(true))
            Object.DestroyImmediate(proxy);

        var anchor = new GameObject("ImmersiveTrader_NpcInteraction");
        anchor.transform.SetParent(prefab.transform, false);

        float height = owner.TraderId == "troldad" ? 1.45f : 1.15f;
        float radius = owner.TraderId == "troldad" ? 1.05f : 0.75f;
        anchor.transform.localPosition = new Vector3(0f, height, 0f);

        var collider = anchor.AddComponent<SphereCollider>();
        collider.radius = radius;
        collider.isTrigger = false;

        var proxy = anchor.AddComponent<TraderInteractionProxy>();
        proxy.Owner = owner;

        var character = prefab.GetComponent<Character>();
        if (character != null)
            character.m_name = string.Empty;
    }

    private static void RegisterJackie()
    {
        if (PrefabManager.Instance.GetPrefab("ImmersiveTrader_Jackie") != null) return;

        var wolf = PrefabManager.Instance.CreateClonedPrefab("ImmersiveTrader_Jackie", "Wolf");
        if (wolf == null) return;

        // Jackie must use the same neutral/player faction as our traders.
        // Keeping native Wolf MonsterAI is fine, but leaving the cloned wolf on its
        // original ForestMonsters faction makes it attack both the player and Troldad.
        var character = wolf.GetComponent<Character>();
        if (character != null)
        {
            character.m_name = "Jackie";
            character.m_faction = Character.Faction.Players;
        }

        var monsterAi = wolf.GetComponent<MonsterAI>();
        if (monsterAi != null)
        {
            monsterAi.m_viewRange = 0f;
            monsterAi.m_viewAngle = 0f;
            monsterAi.m_hearRange = 0f;
            monsterAi.m_alertRange = 0f;
            monsterAi.m_fleeIfNotAlerted = false;
        }

        var tameable = wolf.GetComponent<Tameable>();
        if (tameable != null)
            Object.DestroyImmediate(tameable);

        wolf.transform.localScale = Vector3.one * 0.9f;
        PrefabManager.Instance.AddPrefab(wolf);
    }
}
