using HarmonyLib;

namespace ImmersiveTrader.Patches;

/// <summary>Adds live contract progress to the vanilla item tooltip without custom UI.</summary>
[HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip))]
internal static class ContractTooltipPatch
{
    private static void Postfix(ItemDrop.ItemData __instance, ref string __result)
    {
        string progress = ContractMetadata.GetProgressText(__instance);
        if (string.IsNullOrEmpty(progress)) return;
        __result += $"\n\n<color=orange>{progress}</color>";
    }
}
