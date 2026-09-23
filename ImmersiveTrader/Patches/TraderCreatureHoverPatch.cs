using HarmonyLib;
using ImmersiveTrader.Components;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Current Valheim keeps Player hover targets private. Harmony field injection lets
/// us alter those private fields without compiling against inaccessible members.
/// Creature-derived traders then follow the same Hoverable path as native traders.
/// </summary>
[HarmonyPatch(typeof(Player), "UpdateHover")]
internal static class TraderCreatureHoverPatch
{
    private static void Postfix(Player __instance, ref GameObject ___m_hovering, ref Character ___m_hoveringCreature)
    {
        if (__instance != Player.m_localPlayer || ___m_hovering == null)
            return;

        var trader = ___m_hovering.GetComponentInParent<TraderNpc>();
        if (trader == null)
            trader = ___m_hovering.GetComponentInChildren<TraderNpc>(true);

        if (trader == null && ___m_hoveringCreature != null)
            trader = ___m_hoveringCreature.GetComponentInParent<TraderNpc>();

        if (trader == null)
            return;

        ___m_hovering = trader.gameObject;
        ___m_hoveringCreature = null;
    }
}
