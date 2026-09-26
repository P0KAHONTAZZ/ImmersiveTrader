using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ImmersiveTrader;

internal static class EnhancementStatusRegistry
{
    internal const float DefaultDuration = 1800f;
    private static readonly Dictionary<string, StatusEffect> Effects = new(StringComparer.OrdinalIgnoreCase);
    private static Texture2D? IconAtlas;
    internal static bool RestedGrantedByEnhancement { get; private set; }
    private static readonly Dictionary<string, int> IconIndex = new(StringComparer.OrdinalIgnoreCase)
    {
        ["embers"]=0, ["frost"]=1, ["storm"]=2, ["venom"]=3, ["spirit"]=4,
        ["lumberjack"]=5, ["miner"]=6, ["burden"]=7, ["vitality"]=8, ["endurance"]=9,
        ["focus"]=10, ["craftsman"]=11, ["wanderer"]=12, ["pathfinder"]=13, ["hunter"]=14
    };

    internal static readonly string[] Ids =
    {
        "embers","frost","storm","venom","spirit","lumberjack","miner","burden",
        "vitality","endurance","focus","craftsman","wanderer","pathfinder","hunter","rested"
    };

    internal static void Register()
    {
        if (ObjectDB.instance == null) return;
        AddDamage("embers", "+5-10 Fire do trafienia", SE_EnhancementDamage.Kind.Fire);
        AddDamage("frost", "+5-10 Frost do trafienia", SE_EnhancementDamage.Kind.Frost);
        AddDamage("storm", "+5-10 Lightning do trafienia", SE_EnhancementDamage.Kind.Lightning);
        AddDamage("venom", "+5-10 Poison do trafienia", SE_EnhancementDamage.Kind.Poison);
        AddDamage("spirit", "+5-10 Spirit do trafienia", SE_EnhancementDamage.Kind.Spirit);

        AddDamage("lumberjack", "+20% Chop", SE_EnhancementDamage.Kind.Chop);
        AddDamage("miner", "+20% Pickaxe", SE_EnhancementDamage.Kind.Pickaxe);

        var burden = AddStats("burden", "+10% maks. udźwigu");
        burden.m_addMaxCarryWeight = 0f; // true 10% is calculated dynamically by EnhancementCarryPatch

        var vitality = AddStats("vitality", "+10% regeneracji HP");
        vitality.m_healthRegenMultiplier = 1.10f;

        var endurance = AddStats("endurance", "+20% regeneracji staminy; -10% staminy na ataki melee");
        endurance.m_staminaRegenMultiplier = 1.20f;

        var focus = AddStats("focus", "+15% regeneracji Eitr");
        focus.m_eitrRegenMultiplier = 1.15f;

        var craftsman = AddStats("craftsman", "-20% staminy przy używaniu narzędzi");
        craftsman.m_homeItemStaminaUseModifier = -0.20f;

        var wanderer = AddStats("wanderer", "-15% staminy podczas biegu");
        wanderer.m_runStaminaDrainModifier = -0.15f;

        var pathfinder = AddStats("pathfinder", "-15% staminy podczas skoku");
        pathfinder.m_jumpStaminaUseModifier = -0.15f;

        AddStats("hunter", "-10% staminy przy użyciu łuku");
    }

    private static SE_EnhancementDamage AddDamage(string id, string tooltip, SE_EnhancementDamage.Kind kind)
    {
        var se = ScriptableObject.CreateInstance<SE_EnhancementDamage>();
        se.DamageKind = kind;
        Setup(se, id, tooltip);
        ObjectDB.instance.m_StatusEffects.Add(se);
        Effects[id] = se;
        return se;
    }

    private static StatusEffect Add(string id, string tooltip)
    {
        var se = ScriptableObject.CreateInstance<StatusEffect>();
        Setup(se, id, tooltip);
        ObjectDB.instance.m_StatusEffects.Add(se);
        Effects[id] = se;
        return se;
    }

    private static SE_Stats AddStats(string id, string tooltip)
    {
        var se = ScriptableObject.CreateInstance<SE_Stats>();
        Setup(se, id, tooltip);
        ObjectDB.instance.m_StatusEffects.Add(se);
        Effects[id] = se;
        return se;
    }

    private static void Setup(StatusEffect se, string id, string tooltip)
    {
        se.name = "ImmersiveTrader_" + id;
        se.m_name = DisplayName(id);
        se.m_tooltip = tooltip;
        se.m_icon = LoadIcon(id);
        se.m_ttl = DefaultDuration;
        se.m_flashIcon = false;
        se.m_cooldownIcon = true;
    }

    internal static bool Apply(Player player, string id, float seconds, out string message)
    {
        id = id.ToLowerInvariant();
        if (id == "rested")
        {
            var rested = ObjectDB.instance?.GetStatusEffect("Rested".GetStableHashCode());
            if (rested == null) { message = "Vanilla Rested status was not found."; return false; }
            var active = player.GetSEMan().AddStatusEffect(rested, true);
            if (active == null) { message = "Could not apply Rested."; return false; }
            active.m_ttl = 1200f;
            EnhancementStatusTime.Reset(active);
            RestedGrantedByEnhancement = true;
            message = "Rested applied for 20 minutes.";
            return true;
        }

        if (!Effects.TryGetValue(id, out var template))
        {
            message = "Unknown buff. Use: it buff list";
            return false;
        }

        var applied = player.GetSEMan().AddStatusEffect(template, true);
        if (applied == null) { message = "Could not apply buff."; return false; }
        applied.m_ttl = seconds > 0f ? seconds : DefaultDuration;
        EnhancementStatusTime.Reset(applied);
        message = $"{DisplayName(id)} applied for {Mathf.RoundToInt(applied.m_ttl)} seconds.";
        return true;
    }

    internal static void Clear(Player player)
    {
        if (RestedGrantedByEnhancement)
            player.GetSEMan().RemoveStatusEffect("Rested".GetStableHashCode(), true);
        RestedGrantedByEnhancement = false;
        foreach (var id in Ids)
        {
            if (id == "rested") continue;
            if (Effects.TryGetValue(id, out var se))
                player.GetSEMan().RemoveStatusEffect(se.NameHash(), true);
        }
    }

    internal static bool Has(Character character, string id) =>
        Effects.TryGetValue(id, out var se) && character.GetSEMan().HaveStatusEffect(se.NameHash());

    internal static StatusEffect? Template(string id) =>
        Effects.TryGetValue(id, out var se) ? se : null;

    private static Sprite? LoadIcon(string id)
    {
        if (!IconIndex.TryGetValue(id, out int index)) return null;
        IconAtlas ??= LoadTexture("ImmersiveTrader.Assets.EnhancementStatusIcons.png");
        if (IconAtlas == null || IconAtlas.width < (index + 1) * 128 || IconAtlas.height < 128) return null;
        var sprite = Sprite.Create(IconAtlas, new Rect(index * 128, 0, 128, 128), new Vector2(0.5f, 0.5f));
        sprite.name = "ImmersiveTrader_StatusIcon_" + id;
        return sprite;
    }

    private static Texture2D? LoadTexture(string resource)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
        if (stream == null) { Plugin.Log.LogWarning("Enhancement status icon atlas missing."); return null; }
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(memory.ToArray())) return null;
        texture.filterMode = FilterMode.Bilinear;
        return texture;
    }

    internal static void SetRemaining(Player player, string id, float seconds)
    {
        if (id.Equals("rested", StringComparison.OrdinalIgnoreCase))
        {
            var template = ObjectDB.instance?.GetStatusEffect("Rested".GetStableHashCode());
            var active = template == null ? null : player.GetSEMan().GetStatusEffect(template);
            if (active != null) { active.m_ttl = seconds; EnhancementStatusTime.Reset(active); }
            return;
        }
        var own = Template(id);
        var effect = own == null ? null : player.GetSEMan().GetStatusEffect(own);
        if (effect != null) { effect.m_ttl = seconds; EnhancementStatusTime.Reset(effect); }
    }

    internal static void MarkRestedEnhancement(bool value) => RestedGrantedByEnhancement = value;

    internal static string DisplayName(string id) => char.ToUpperInvariant(id[0]) + id.Substring(1);
}
