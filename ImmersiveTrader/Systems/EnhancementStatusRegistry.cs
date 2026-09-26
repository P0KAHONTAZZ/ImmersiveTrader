using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveTrader;

internal static class EnhancementStatusRegistry
{
    internal const float DefaultDuration = 1800f;
    private static readonly Dictionary<string, StatusEffect> Effects = new(StringComparer.OrdinalIgnoreCase);

    internal static readonly string[] Ids =
    {
        "embers","frost","storm","venom","spirit","lumberjack","miner","burden",
        "vitality","endurance","focus","craftsman","wanderer","pathfinder","hunter","rested"
    };

    internal static void Register()
    {
        if (ObjectDB.instance == null) return;
        Add("embers", "+5-10 Fire do trafienia");
        Add("frost", "+5-10 Frost do trafienia");
        Add("storm", "+5-10 Lightning do trafienia");
        Add("venom", "+5-10 Poison do trafienia");
        Add("spirit", "+5-10 Spirit do trafienia");

        AddStats("lumberjack", "+20% Chop");
        AddStats("miner", "+20% Pickaxe");

        var burden = AddStats("burden", "+10% maks. udźwigu");
        burden.m_addMaxCarryWeight = 30f; // patched below to be true 10% dynamically

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
        wanderer.m_runStaminaUseModifier = -0.15f;

        var pathfinder = AddStats("pathfinder", "-15% staminy podczas skoku");
        pathfinder.m_jumpStaminaUseModifier = -0.15f;

        AddStats("hunter", "-10% staminy przy użyciu łuku");
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
            active.m_time = 0f;
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
        applied.m_time = 0f;
        message = $"{DisplayName(id)} applied for {Mathf.RoundToInt(applied.m_ttl)} seconds.";
        return true;
    }

    internal static void Clear(Player player)
    {
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

    internal static string DisplayName(string id) => char.ToUpperInvariant(id[0]) + id.Substring(1);
}
