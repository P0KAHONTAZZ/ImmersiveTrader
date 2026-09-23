using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderInteraction
{
    public static void Handle(Player player, TraderDefinition trader, bool alternateUse, Vector3 traderPosition)
    {
        if (alternateUse)
        {
            var offers = TraderShop.GetAvailableOffers(trader.Id);
            if (offers.Length > 0)
            {
                // Temporary interaction path until the native-style shop panel is wired:
                // alternate-use buys the first thematic offer through the same backend.
                TraderShop.TryBuy(player, offers[0]);
                return;
            }
        }

        if (QuestDelivery.TryDeliverAny(player, trader, traderPosition))
            return;

        // Existing task takes priority over issuing another courier shipment.
        if (TraderActivityService.TryTurnIn(player, trader.Id))
            return;

        if (QuestIssuing.TryGiveTreasure(player, trader, traderPosition))
            return;

        // If the player cannot take another shipment, the same trader can still offer
        // their local hunt/gather job instead of becoming a dead interaction.
        if (TraderActivityService.TryAccept(player, trader.Id))
            return;

        var stock = TraderShop.GetAvailableOffers(trader.Id);
        if (stock.Length > 0)
        {
            var offer = stock[0];
            player.Message(MessageHud.MessageType.Center,
                $"{trader.Name}: {offer.Label} - {offer.Price} coins. Use alternate interact to buy.");
            return;
        }

        player.Message(MessageHud.MessageType.Center, $"{trader.Name}: Come back when you can carry another shipment.");
    }
}
