using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderInteraction
{
    public static void Handle(Player player, TraderDefinition trader, bool alternateUse, Vector3 traderPosition)
    {
        if (trader.Id == "midka" && alternateUse)
        {
            MidkaShop.TryBuyMinorHealingMead(player);
            return;
        }

        if (QuestDelivery.TryDeliverAny(player, trader, traderPosition))
            return;

        if (QuestIssuing.TryGiveTreasure(player, trader, traderPosition))
            return;

        if (trader.Id == "midka")
        {
            player.Message(MessageHud.MessageType.Center,
                $"Midka: I can sell you Minor Healing Mead for {Plugin.MidkaHealingMeadPrice.Value} coins. Use alternate interact to buy.");
            return;
        }

        player.Message(MessageHud.MessageType.Center, $"{trader.Name}: Come back when you can carry another shipment.");
    }
}
