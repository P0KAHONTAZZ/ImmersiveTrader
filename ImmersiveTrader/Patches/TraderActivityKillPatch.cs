using HarmonyLib;
using System.Runtime.CompilerServices;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Hunt tracking hook. Damage attribution is captured when Character.Damage still has
/// the attacking Player available; OnDeath then consumes that attribution. This avoids
/// relying on a GetLastAttacker API that is not exposed by the current Valheim assembly.
/// </summary>
[HarmonyPatch]
internal static class TraderActivityKillPatch
{
    // Destroyed or despawned creatures must not be kept alive by the tracking table.
    private sealed class Attribution { internal long PlayerId; }
    private static readonly ConditionalWeakTable<Character, Attribution> LastPlayerHit = new();

    [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
    [HarmonyPrefix]
    private static void DamagePrefix(Character __instance, HitData hit)
    {
        if (__instance == null || hit == null || __instance.IsPlayer()) return;
        var attacker = hit.GetAttacker();
        if (attacker is Player player)
            LastPlayerHit.GetValue(__instance, _ => new Attribution()).PlayerId = player.GetPlayerID();
    }

    [HarmonyPatch(typeof(Character), "OnDeath")]
    [HarmonyPrefix]
    private static void DeathPrefix(Character __instance)
    {
        if (__instance == null || __instance.IsPlayer()) return;

        if (!LastPlayerHit.TryGetValue(__instance, out var attribution))
            return;

        // Explicitly clear dead creatures even while no local player exists.
        LastPlayerHit.Remove(__instance);
        var local = Player.m_localPlayer;
        if (local == null || attribution.PlayerId != local.GetPlayerID())
            return;
        string prefabName = Utils.GetPrefabName(__instance.gameObject);
        TraderActivityService.RegisterKillOnPhysicalContracts(local, prefabName);
    }
}
