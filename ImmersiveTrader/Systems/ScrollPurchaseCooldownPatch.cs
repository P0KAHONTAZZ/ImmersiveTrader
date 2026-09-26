using System;
using System.Linq;
using HarmonyLib;

namespace ImmersiveTrader;

/// <summary>
/// Enhancement scrolls sold at reputation 5 can be bought once per 3 in-game days
/// (per world, character, trader and scroll). The vanilla purchase itself is untouched:
/// the prefix blocks it while on cooldown, the postfix records a successful purchase.
/// </summary>
[HarmonyPatch(typeof(StoreGui), "BuySelectedItem")]
internal static class ScrollPurchaseCooldownPatch
{
    private static bool Prefix(StoreGui __instance, out int __state)
    {
        __state = -1;
        var player = Player.m_localPlayer;
        var row = AccessTools.Field(typeof(StoreGui), "m_selectedItem")?.GetValue(__instance) as Trader.TradeItem;
        if (player == null || row == null || !NativeTraderWindow.TryGetOrdinarySale(row, out var offer)) return true;
        if (!offer.ItemPrefab.StartsWith(EnhancementScrollItems.PrefabPrefix, StringComparison.Ordinal)) return true;
        try
        {
            if (!ItemPurchaseCooldown.CanBuy(player, offer.TraderId, "scroll", offer.ItemPrefab,
                    TraderShopRegistry.ScrollCooldownDays, out int days))
            {
                player.Message(MessageHud.MessageType.Center, $"{offer.Label} available again in {days} Valheim day(s).");
                return false;
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Scroll cooldown check failed: {ex}");
            player.Message(MessageHud.MessageType.Center, "Scroll unavailable: cooldown data could not be read.");
            return false;
        }
        __state = Count(player, offer.ItemPrefab);
        return true;
    }

    private static void Postfix(StoreGui __instance, int __state)
    {
        if (__state < 0) return;
        var player = Player.m_localPlayer;
        var row = AccessTools.Field(typeof(StoreGui), "m_selectedItem")?.GetValue(__instance) as Trader.TradeItem;
        if (player == null || row == null || !NativeTraderWindow.TryGetOrdinarySale(row, out var offer)) return;
        if (Count(player, offer.ItemPrefab) <= __state) return; // purchase did not happen
        try { ItemPurchaseCooldown.MarkBought(player, offer.TraderId, "scroll", offer.ItemPrefab); }
        catch (Exception ex) { Plugin.Log.LogError($"Scroll cooldown save failed: {ex}"); }
    }

    private static int Count(Player player, string prefab) =>
        player.GetInventory().GetAllItems()
            .Where(i => i.m_dropPrefab != null && i.m_dropPrefab.name.StartsWith(prefab, StringComparison.Ordinal))
            .Sum(i => i.m_stack);
}
