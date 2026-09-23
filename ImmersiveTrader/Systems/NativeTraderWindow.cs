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

        // Never AddComponent<Trader>() to our NPC: Trader.Awake/Update expects a fully
        // authored vanilla trader hierarchy (talk points, dialogue lists, effects, etc.).
        // Clone Haldor's initialized Trader component data onto a disabled helper instead.
        var haldor = ZNetScene.instance?.GetPrefab("Haldor");
        var template = haldor?.GetComponent<Trader>();
        if (template == null)
        {
            player.Message(MessageHud.MessageType.Center, "Native trader template is unavailable.");
            return false;
        }

        var helper = UnityEngine.Object.Instantiate(haldor, npc.transform.position + Vector3.down * 1000f, npc.transform.rotation);
        helper.name = $"ImmersiveTrader_Store_{definition.Id}";
        helper.transform.SetParent(npc.transform, true);
        helper.transform.localScale = Vector3.zero;

        // Hide the helper completely; it exists only to provide a valid vanilla Trader
        // object to StoreGui. The visible/interactable NPC remains our custom shell.
        foreach (var renderer in helper.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = false;
        foreach (var collider in helper.GetComponentsInChildren<Collider>(true))
            collider.enabled = false;

        var trader = helper.GetComponent<Trader>();
        if (trader == null)
        {
            UnityEngine.Object.Destroy(helper);
            return false;
        }

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
                m_stack = offer.Stack,
                m_requiredGlobalKey = string.Empty,
                m_levelUpEffect = false,
                m_buyPlayerEffects = new EffectList(),
                m_icon = null,
                m_name = itemDrop.m_itemData?.m_shared?.m_name ?? string.Empty,
                m_tooltip = itemDrop.m_itemData?.m_shared?.m_description ?? string.Empty,
                m_buyKey = string.Empty,
                m_incrementKey = string.Empty,
                m_incrementAmount = 0
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
