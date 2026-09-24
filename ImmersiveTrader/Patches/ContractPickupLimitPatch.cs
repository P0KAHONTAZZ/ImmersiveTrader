using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader.Patches;

/// <summary>Keep the two-contract-per-issuer limit when picking up a dropped scroll.</summary>
[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.Pickup), new[] { typeof(GameObject), typeof(bool), typeof(bool) })]
internal static class ContractPickupLimitPatch
{
    private static bool Prefix(Humanoid __instance, GameObject go, ref bool __result)
    {
        if (__instance is not Player player || go == null) return true;
        var drop = go.GetComponent<ItemDrop>();
        if (drop == null) return true;

        // A loose item's metadata may still reside in its network object until Load.
        // Call the game's loader only for our own scrolls.
        if (!go.name.StartsWith(ContractRegistry.PrefabName, StringComparison.Ordinal)) return true;
        try { AccessTools.Method(typeof(ItemDrop), "Load")?.Invoke(drop, null); }
        catch (Exception ex)
        {
            Plugin.Log.LogWarning($"Could not load dropped contract metadata: {ex.Message}");
            return true;
        }
        if (drop.m_itemData == null) return true;
        if (!ContractMetadata.TryRead(drop.m_itemData, out _, out string issuer, out _))
        {
            var data = drop.m_itemData.m_customData;
            if (!data.TryGetValue("ImmersiveTrader.ContractId", out string contractId) ||
                !data.TryGetValue("ImmersiveTrader.ContractTrader", out issuer) ||
                !string.Equals(go.name.Replace("(Clone)", string.Empty),
                    ContractRegistry.GetPrefabName(contractId), StringComparison.Ordinal) ||
                !TraderActivityRegistry.Activities.Any(x => x.Id == contractId && x.TraderId == issuer))
                return true;
        }

        if (TraderActivityService.CountPhysicalContracts(player, issuer) < 2) return true;
        var trader = TraderRegistry.Traders.FirstOrDefault(x => x.Id == issuer);
        player.Message(MessageHud.MessageType.Center,
            $"Masz już dwa kontrakty od {trader?.Name ?? issuer}. Oddaj jeden, zanim podniesiesz kolejny.");
        __result = false;
        return false;
    }
}
