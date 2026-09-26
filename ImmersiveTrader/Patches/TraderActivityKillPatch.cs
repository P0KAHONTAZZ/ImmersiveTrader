using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Contract participation follows the same proximity model as personal boss progression:
/// when a valid contract target dies, every player within 100 m who carries a matching
/// contract receives one kill. The killer does not need to be the contract holder.
/// </summary>
[HarmonyPatch(typeof(Character), "OnDeath")]
internal static class TraderActivityKillPatch
{
    private const float ParticipationRadius = 100f;

    [HarmonyPrefix]
    private static void Prefix(Character __instance)
    {
        if (__instance == null || __instance.IsPlayer() || __instance.IsBoss())
            return;

        // Each client owns its own physical contract scroll. When that client observes
        // the network death, it only evaluates its local character. This gives the same
        // 100 m participation semantics as boss progression without ever editing another
        // player's inventory from the wrong peer.
        var player = Player.m_localPlayer;
        if (player == null || Vector3.Distance(player.transform.position, __instance.transform.position) > ParticipationRadius)
            return;

        string prefabName = Utils.GetPrefabName(__instance.gameObject);
        TraderActivityService.RegisterKillOnPhysicalContracts(player, prefabName);
    }
}
