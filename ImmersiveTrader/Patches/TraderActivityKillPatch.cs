using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Counts only kills credited by Valheim to the local Player. This is a prototype hook;
/// persistence/server reconciliation remains separate from task rules.
/// </summary>
[HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
internal static class TraderActivityKillPatch
{
    private static void Prefix(Character __instance)
    {
        var local = Player.m_localPlayer;
        if (local == null || __instance == null || __instance.IsPlayer()) return;

        var attacker = __instance.GetLastAttacker();
        if (attacker != local) return;

        TraderActivityService.RegisterKill(local, Utils.GetPrefabName(__instance.gameObject));
    }
}
