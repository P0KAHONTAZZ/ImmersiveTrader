using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderShop
{
    public static TraderOfferDefinition[] GetAvailableOffers(Player player, string traderId) =>
        TraderShopRegistry.Offers
            .Where(x => x.TraderId == traderId &&
                TraderReputation.GetLevel(player, traderId) >= x.RequiredReputationLevel &&
                ProgressionGate.IsRewardTierUnlocked(x.RequiredTier))
            .ToArray();

    public static bool TryBuy(Player player, TraderOfferDefinition offer)
    {
        if (!ProgressionGate.IsRewardTierUnlocked(offer.RequiredTier) ||
            TraderReputation.GetLevel(player, offer.TraderId) < offer.RequiredReputationLevel)
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

        // Validate capacity before taking money. AddItem can fail when inventory slots
        // are full even if the player has enough coins.
        if (!inventory.CanAddItem(prefab, offer.Stack))
        {
            player.Message(MessageHud.MessageType.Center, "Make room in your inventory first.");
            return false;
        }

        inventory.RemoveItem("Coins", offer.Price);
        if (!inventory.AddItem(prefab, offer.Stack))
        {
            // Defensive rollback: never eat the player's coins if Valheim rejects the item.
            var coins = ObjectDB.instance?.GetItemPrefab("Coins");
            if (coins != null) inventory.AddItem(coins, offer.Price);
            player.Message(MessageHud.MessageType.Center, "Purchase failed; your coins were returned.");
            return false;
        }

        player.Message(MessageHud.MessageType.Center, $"Bought {offer.Stack}x {offer.Label}.");
        return true;
    }
}
