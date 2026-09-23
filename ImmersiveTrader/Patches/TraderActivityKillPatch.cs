using HarmonyLib;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Hunt tracking hook. Damage attribution is captured when Character.Damage still has
/// the attacking Player available; OnDeath then consumes that attribution. This avoids
/// relying on a GetLastAttacker API that is not exposed by the current Valheim assembly.
/// </summary>
[HarmonyPatch]
internal static class TraderActivityKillPatch
{
    private static readonly System.Collections.Generic.Dictionary<Character, long> LastPlayerHit = new();

    [HarmonyPatch(typeof(Character), nameof(Character.Damage))]
    [HarmonyPrefix]
    private static void DamagePrefix(Character __instance, HitData hit)
    {
        if (__instance == null || hit == null || __instance.IsPlayer()) return;
        var attacker = hit.GetAttacker();
        if (attacker is Player player)
            LastPlayerHit[__instance] = player.GetPlayerID();
    }

    [HarmonyPatch(typeof(Character), "OnDeath")]
    [HarmonyPrefix]
    private static void DeathPrefix(Character __instance)
    {
        var local = Player.m_localPlayer;
        if (local == null || __instance == null || __instance.IsPlayer()) return;

        if (!LastPlayerHit.TryGetValue(__instance, out long playerId) || playerId != local.GetPlayerID())
            return;

        LastPlayerHit.Remove(__instance);
        TraderActivityService.RegisterKill(local, Utils.GetPrefabName(__instance.gameObject));
    }
}
