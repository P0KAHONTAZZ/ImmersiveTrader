using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace ImmersiveTrader.Patches;

/// <summary>Adds live contract progress to the vanilla item tooltip without custom UI.</summary>
[HarmonyPatch]
internal static class ContractTooltipPatch
{
    // Valheim exposes several GetTooltip overloads. An unqualified HarmonyPatch
    // fails during Plugin.Awake and prevents all trader NPCs from registering.
    private static IEnumerable<MethodBase> TargetMethods() =>
        typeof(ItemDrop.ItemData)
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(method => method.Name == nameof(ItemDrop.ItemData.GetTooltip) && method.ReturnType == typeof(string));

    private static void Postfix(ItemDrop.ItemData __instance, ref string __result)
    {
        string progress = ContractMetadata.GetProgressText(__instance);
        if (string.IsNullOrEmpty(progress)) return;
        __result += $"\n\n<color=orange>{progress}</color>";
    }
}
