using System;
using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Opens Valheim's native StoreGui using a temporary Trader component populated
/// from ImmersiveTrader's curated stock. The NPC shell itself stays custom.
/// </summary>
public static class NativeTraderWindow
{
    public static bool TryOpen(Player player, TraderDefinition definition, GameObject npc)
    {
        var gui = StoreGui.instance;
        if (gui == null)
        {
            player.Message(MessageHud.MessageType.Center, "Trader window is not ready.");
            return false;
        }

        var offers = TraderShop.GetAvailableOffers(definition.Id);
        if (offers.Length == 0)
        {
            player.Message(MessageHud.MessageType.Center, $"{definition.Name}: Nothing for sale right now.");
            return false;
        }

        var trader = npc.GetComponent<Trader>() ?? npc.AddComponent<Trader>();
        trader.m_name = definition.Name;
        trader.m_items.Clear();

        foreach (var offer in offers)
        {
            var prefab = ObjectDB.instance?.GetItemPrefab(offer.ItemPrefab);
            var itemDrop = prefab?.GetComponent<ItemDrop>();
            if (itemDrop == null)
            {
                Plugin.Log.LogWarning($"Store item prefab missing for {definition.Id}: {offer.ItemPrefab}");
                continue;
            }

            var tradeItem = new Trader.TradeItem
            {
                m_prefab = itemDrop,
                m_price = offer.Price,
                m_stack = offer.Stack
            };

            trader.m_items.Add(tradeItem);
            Plugin.Log.LogInfo($"Native shop {definition.Id}: {offer.ItemPrefab}, configured={offer.Price}, native={tradeItem.m_price}, stack={tradeItem.m_stack}");
        }

        if (trader.m_items.Count == 0)
            return false;

        gui.Show(trader);
        return true;
    }
}
