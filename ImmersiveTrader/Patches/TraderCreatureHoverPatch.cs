using HarmonyLib;
using ImmersiveTrader.Components;
using UnityEngine;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Valheim treats creature prefabs as creatures before it evaluates normal Hoverable
/// components. Creature-derived traders therefore get the enemy-health hover path.
/// Redirect only our trader creatures back to the normal hover/interact path after
/// Valheim has resolved the raycast.
/// </summary>
[HarmonyPatch(typeof(Player), "UpdateHover")]
internal static class TraderCreatureHoverPatch
{
    private static void Postfix(Player __instance)
    {
        if (__instance != Player.m_localPlayer || __instance.m_hovering == null)
            return;

        var trader = __instance.m_hovering.GetComponentInParent<TraderNpc>();
        if (trader == null)
            trader = __instance.m_hovering.GetComponentInChildren<TraderNpc>(true);

        if (trader == null)
            return;

        __instance.m_hovering = trader.gameObject;
        __instance.m_hoveringCreature = null;
    }
}
