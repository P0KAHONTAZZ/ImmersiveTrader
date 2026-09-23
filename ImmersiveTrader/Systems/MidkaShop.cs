using UnityEngine;

namespace ImmersiveTrader;

public static class MidkaShop
{
    public const string HealingMeadPrefab = "MeadHealthMinor";

    public static bool TryBuyMinorHealingMead(Player player)
    {
        int price = Plugin.MidkaHealingMeadPrice.Value;
        var inventory = player.GetInventory();

        if (inventory.CountItems("Coins") < price)
        {
            player.Message(MessageHud.MessageType.Center, $"Midka: You need {price} coins.");
            return false;
        }

        var itemPrefab = ObjectDB.instance.GetItemPrefab(HealingMeadPrefab);
        if (itemPrefab == null)
        {
            player.Message(MessageHud.MessageType.Center, "Midka: I'm out of healing mead.");
            return false;
        }

        inventory.RemoveItem("Coins", price);
        inventory.AddItem(itemPrefab, 1);
        player.Message(MessageHud.MessageType.Center, $"Bought Minor Healing Mead for {price} coins.");
        return true;
    }
}