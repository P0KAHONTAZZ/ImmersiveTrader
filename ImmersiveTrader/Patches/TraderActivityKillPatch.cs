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

        // The peer that owns the dead network character is the single source of the
        // death event. Each nearby player's own client updates its physical contract
        // when it observes that same network death; server-side transaction validation
        // remains responsible for final turn-in/rewards.
        string prefabName = Utils.GetPrefabName(__instance.gameObject);
        foreach (Player player in Player.GetAllPlayers())
        {
            if (player == null || Vector3.Distance(player.transform.position, __instance.transform.position) > ParticipationRadius)
                continue;

            // A remote player's inventory is not authoritative on this client. Only
            // mutate the locally owned character's physical scroll; all peers observe
            // the same death and therefore independently evaluate their own holder.
            if (player != Player.m_localPlayer)
                continue;

            TraderActivityService.RegisterKillOnPhysicalContracts(player, prefabName);
        }
    }
}
