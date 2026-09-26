using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Contract-style tooltip lines for physical enhancement scrolls (Effect / Duration / Use / Stacking).
/// Separate from ContractTooltipPatch; it only touches items whose consume effect is one of ours.
/// </summary>
[HarmonyPatch]
internal static class EnhancementScrollTooltipPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        typeof(ItemDrop.ItemData)
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(method => method.Name == nameof(ItemDrop.ItemData.GetTooltip) && method.ReturnType == typeof(string));

    private static void Postfix(ItemDrop.ItemData __instance, ref string __result)
    {
        try
        {
            string? id = EnhancementScrollItems.IdFor(__instance);
            if (id == null) return;
            __result += "\n\n" + EnhancementScrollItems.TooltipLines(id);
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning("Enhancement scroll tooltip failed: " + e.Message);
        }
    }
}
