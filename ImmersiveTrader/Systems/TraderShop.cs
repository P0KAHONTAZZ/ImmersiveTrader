using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Shop backend shared by the future native-style trader window.
/// Keeping purchases here means UI code never owns inventory/progression rules.
/// </summary>
public static class TraderShop
{
    public static TraderOfferDefinition[] GetAvailableOffers(string traderId) =>
        TraderShopRegistry.Offers
            .Where(x => x.TraderId == traderId && ProgressionGate.IsRewardTierUnlocked(x.RequiredTier))
            .ToArray();

    public static bool TryBuy(Player player, TraderOfferDefinition offer)
    {
        if (!ProgressionGate.IsRewardTierUnlocked(offer.RequiredTier))
        {
            player.Message(MessageHud.MessageType.Center, "This stock is not available yet.");
            return false;
        }

        var inventory = player.GetInventory();
        if (inventory.CountItems("Coins") < offer.Price)
        {
            player.Message(MessageHud.MessageType.Center, $"You need {offer.Price} coins.");
            return false;
        }

        var prefab = ObjectDB.instance?.GetItemPrefab(offer.ItemPrefab);
        if (prefab == null)
        {
            player.Message(MessageHud.MessageType.Center, $"{offer.Label} is currently unavailable.");
            return false;
        }

        inventory.RemoveItem("Coins", offer.Price);
        inventory.AddItem(prefab, offer.Stack);
        player.Message(MessageHud.MessageType.Center, $"Bought {offer.Stack}x {offer.Label}.");
        return true;
    }
}
