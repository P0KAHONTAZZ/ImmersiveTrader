using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderInteraction
{
    public static void Handle(Player player, TraderDefinition trader, bool alternateUse, Vector3 traderPosition)
    {
        // Alternate interaction remains the quick shop action while the native StoreGui
        // integration is developed. Normal interaction is now deterministic: an existing
        // delivery/task is handled before a new courier shipment is issued.
        if (alternateUse)
        {
            ShowOrBuyStock(player, trader);
            return;
        }

        if (QuestDelivery.TryDeliverAny(player, trader, traderPosition))
            return;

        if (TraderActivityService.TryTurnInPhysical(player, trader.Id))
            return;

        ShowStock(player, trader);
    }

    private static void ShowOrBuyStock(Player player, TraderDefinition trader)
    {
        var offers = TraderShop.GetAvailableOffers(trader.Id);
        if (offers.Length == 0)
        {
            player.Message(MessageHud.MessageType.Center, $"{trader.Name}: Nothing for sale right now.");
            return;
        }

        TraderShop.TryBuy(player, offers[0]);
    }

    private static void ShowStock(Player player, TraderDefinition trader)
    {
        var stock = TraderShop.GetAvailableOffers(trader.Id);
        if (stock.Length > 0)
        {
            var offer = stock[0];
            player.Message(MessageHud.MessageType.Center,
                $"{trader.Name}: {offer.Label} - {offer.Price} coins. Use alternate interact to buy.");
            return;
        }

        player.Message(MessageHud.MessageType.Center, $"{trader.Name}: Nothing available right now.");
    }
}
