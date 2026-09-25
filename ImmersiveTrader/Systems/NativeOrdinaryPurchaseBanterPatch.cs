using System.Linq;
using HarmonyLib;
using ImmersiveTrader.Components;

namespace ImmersiveTrader;

[HarmonyPatch(typeof(StoreGui), "BuySelectedItem")]
internal static class NativeOrdinaryPurchaseBanterPatch
{
    private static bool Prefix(StoreGui __instance, out int __state)
    {
        __state = -1;
        var row = AccessTools.Field(typeof(StoreGui), "m_selectedItem")?.GetValue(__instance) as Trader.TradeItem;
        if (row == null || NativeCargoBridge.IsCargo(row) || NativeContractBridge.IsContract(row) ||
            NativeTraderWindow.ActiveNpc == null) return true;
        if (!NativeTraderWindow.TryGetOrdinarySale(row, out var offer)) return true;
        var player = Player.m_localPlayer;
        if (player == null || TraderReputation.GetLevel(player, offer.TraderId) < offer.RequiredReputationLevel ||
            !ProgressionGate.IsRewardTierUnlocked(offer.RequiredTier))
        {
            player?.Message(MessageHud.MessageType.Center,
                $"Requires {offer.RequiredReputationLevel} reputation stars and biome progression.");
            return false;
        }
        __state = CountCarried(row);
        return true;
    }

    private static void Postfix(StoreGui __instance, int __state)
    {
        if (__state < 0) return;
        var row = AccessTools.Field(typeof(StoreGui), "m_selectedItem")?.GetValue(__instance) as Trader.TradeItem;
        if (row != null && !NativeCargoBridge.IsCargo(row) && !NativeContractBridge.IsContract(row) &&
            CountCarried(row) > __state)
            TraderBanter.PurchaseAtShop();
    }

    private static int CountCarried(Trader.TradeItem row)
    {
        var player = Player.m_localPlayer;
        var prefabName = row.m_prefab?.gameObject.name;
        if (player == null || prefabName == null) return 0;
        return player.GetInventory().GetAllItems().Where(item =>
            item.m_dropPrefab != null && item.m_dropPrefab.name.StartsWith(prefabName))
            .Sum(item => item.m_stack);
    }
}
