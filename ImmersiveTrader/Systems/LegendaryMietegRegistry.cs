using ImmersiveTrader.Components;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

public static class LegendaryMietegRegistry
{
    private static bool _prefabRegistered;
    private static bool _locationsRegistered;

    public static void RegisterPrefab()
    {
        if (_prefabRegistered) return;

        var prefab = PrefabManager.Instance.CreateClonedPrefab("ImmersiveTrader_NPC_legendary_mieteg", "Odin");
        if (prefab == null) return;

        // Mieteg deliberately uses Odin's hooded one-eyed presentation. Strip Odin's
        // encounter controller so he cannot vanish or run the vanilla apparition logic;
        // ImmersiveTrader controls his 1-2 day presence instead.
        foreach (var behaviour in prefab.GetComponents<MonoBehaviour>())
        {
            if (behaviour != null && behaviour.GetType().Name == "Odin")
                Object.DestroyImmediate(behaviour);
        }

        var vanillaTrader = prefab.GetComponent<Trader>();
        if (vanillaTrader != null) Object.DestroyImmediate(vanillaTrader);

        var npcTalk = prefab.GetComponent<NpcTalk>();
        if (npcTalk != null) Object.DestroyImmediate(npcTalk);

        // Odin is primarily an apparition and may not expose a normal trader hit volume.
        // Give Mieteg a dedicated interaction surface without touching his visuals.
        var interaction = new GameObject("ImmersiveTrader_MietegInteraction");
        interaction.transform.SetParent(prefab.transform, false);
        interaction.transform.localPosition = new Vector3(0f, 1.15f, 0f);
        var collider = interaction.AddComponent<SphereCollider>();
        collider.radius = 0.85f;
        collider.isTrigger = false;

        var npc = prefab.GetComponent<TraderNpc>() ?? prefab.AddComponent<TraderNpc>();
        npc.TraderId = "legendary_mieteg";
        var proxy = interaction.AddComponent<TraderInteractionProxy>();
        proxy.Owner = npc;

        PrefabManager.Instance.AddPrefab(prefab);
        _prefabRegistered = true;
    }

    public static void RegisterLocations()
    {
        if (_locationsRegistered) return;

        // Prepared rare sites. Runtime activation/deactivation is deliberately separated
        // from world generation so one site can be selected for a 1-2 day appearance.
        int siteIndex = 0;
        const int totalSites = 8;
        var biomes = new[]
        {
            Heightmap.Biome.BlackForest, Heightmap.Biome.BlackForest,
            Heightmap.Biome.Swamp, Heightmap.Biome.Swamp,
            Heightmap.Biome.Mountain, Heightmap.Biome.Mountain,
            Heightmap.Biome.Plains, Heightmap.Biome.Plains
        };

        foreach (var biome in biomes)
        {
            int currentSite = siteIndex++;
            var container = ZoneManager.Instance.CreateLocationContainer($"ImmersiveTrader_MietegSite_{currentSite}_{biome}");
            var prefab = PrefabManager.Instance.GetPrefab("ImmersiveTrader_NPC_legendary_mieteg");
            if (prefab == null) continue;

            var npc = Object.Instantiate(prefab, container.transform);
            npc.name = prefab.name;
            npc.transform.localPosition = Vector3.zero;
            var presence = npc.GetComponent<LegendaryMietegPresence>() ?? npc.AddComponent<LegendaryMietegPresence>();
            presence.SiteIndex = currentSite;
            presence.SiteCount = totalSites;

            var config = new LocationConfig
            {
                Biome = biome,
                Quantity = 1,
                Unique = false,
                Priotized = false,
                ClearArea = false,
                ExteriorRadius = 5f,
                MinDistance = 2500f,
                MaxDistance = 8000f,
                MinDistanceFromSimilar = 2500f,
                Group = "ImmersiveTrader_LegendaryMieteg",
                IconPlaced = false,
                IconAlways = false
            };

            ZoneManager.Instance.AddCustomLocation(new CustomLocation(container, false, config));
        }

        _locationsRegistered = true;
    }
}
