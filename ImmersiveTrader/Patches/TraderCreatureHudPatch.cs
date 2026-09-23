using HarmonyLib;
using ImmersiveTrader.Components;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Creature visuals still need Character/MonsterAI for their native animation graph,
/// but they are traders, not combat targets. Keep the creature shell intact and stop
/// EnemyHud from creating/retaining an enemy health bar for ImmersiveTrader NPCs.
/// </summary>
[HarmonyPatch(typeof(EnemyHud), "TestShow")]
internal static class TraderCreatureHudPatch
{
    private static void Postfix(Character c, ref bool __result)
    {
        if (!__result || c == null)
            return;

        if (c.GetComponent<TraderNpc>() != null ||
            c.GetComponentInParent<TraderNpc>() != null ||
            c.GetComponentInChildren<TraderNpc>(true) != null)
        {
            __result = false;
        }
    }
}
