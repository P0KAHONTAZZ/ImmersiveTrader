using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class TraderInteraction
{
    public static void Handle(Player player, TraderDefinition trader, bool alternateUse)
    {
        if (trader.Id == "midka" && alternateUse)
        {
            MidkaShop.TryBuyMinorHealingMead(player);
            return;
        }

        if (QuestDelivery.TryDeliverAny(player, trader))
            return;

        if (QuestIssuing.TryGiveTreasure(player, trader))
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
