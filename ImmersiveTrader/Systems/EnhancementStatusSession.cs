using System;
using System.Collections.Generic;
using HarmonyLib;

namespace ImmersiveTrader;

/// <summary>
/// Enhancement scroll buffs are session-only. Death snapshots and restores their remaining
/// duration; Game.Logout clears the snapshot, so nothing survives a relog.
/// </summary>
internal static class EnhancementStatusSession
{
    private static readonly Dictionary<string, float> Pending = new(StringComparer.OrdinalIgnoreCase);

    internal static void Snapshot(Player player)
    {
        Pending.Clear();
        foreach (string id in EnhancementStatusRegistry.Ids)
        {
            if (id == "rested")
            {
                if (!EnhancementStatusRegistry.RestedGrantedByEnhancement) continue;
                var restedTemplate = ObjectDB.instance?.GetStatusEffect("Rested".GetStableHashCode());
                var rested = restedTemplate == null ? null : player.GetSEMan().GetStatusEffect(restedTemplate);
                if (rested != null) Pending[id] = Math.Max(0.1f, EnhancementStatusTime.Remaining(rested));
                continue;
            }

            var template = EnhancementStatusRegistry.Template(id);
            var active = template == null ? null : player.GetSEMan().GetStatusEffect(template);
            if (active != null) Pending[id] = Math.Max(0.1f, EnhancementStatusTime.Remaining(active));
        }
    }

    internal static void Restore(Player player)
    {
        if (Pending.Count == 0) return;
        foreach (var pair in Pending)
        {
            EnhancementStatusRegistry.Apply(player, pair.Key, pair.Value, out _);
            EnhancementStatusRegistry.SetRemaining(player, pair.Key, pair.Value);
        }
        Pending.Clear();
    }

    internal static void Logout()
    {
        Pending.Clear();
        EnhancementStatusRegistry.MarkRestedEnhancement(false);
    }
}

[HarmonyPatch(typeof(Character), "OnDeath")]
internal static class EnhancementDeathPersistencePatch
{
    [HarmonyPrefix]
    private static void Prefix(Character __instance)
    {
        if (__instance is Player player && ReferenceEquals(player, Player.m_localPlayer))
            EnhancementStatusSession.Snapshot(player);
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
internal static class EnhancementRespawnPersistencePatch
{
    [HarmonyPostfix]
    private static void Postfix(Player __instance)
    {
        if (ReferenceEquals(__instance, Player.m_localPlayer))
            EnhancementStatusSession.Restore(__instance);
    }
}

[HarmonyPatch(typeof(Game), nameof(Game.Logout))]
internal static class EnhancementLogoutPatch
{
    [HarmonyPrefix]
    private static void Prefix() => EnhancementStatusSession.Logout();
}
