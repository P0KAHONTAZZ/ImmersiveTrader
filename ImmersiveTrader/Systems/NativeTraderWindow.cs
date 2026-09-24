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

        // Instantiate the helper while the Haldor prefab is disabled. Otherwise its
        // ZNetView.Awake can create a persistent vanilla Haldor ZDO before we hide it,
        // which then reappears as a real Haldor after the next world load.
        bool haldorWasActive = haldor.activeSelf;
        haldor.SetActive(false);
        var helper = UnityEngine.Object.Instantiate(haldor, npc.transform.position + Vector3.down * 1000f, npc.transform.rotation);
        haldor.SetActive(haldorWasActive);

        helper.name = $"ImmersiveTrader_Store_{definition.Id}";
        helper.transform.SetParent(npc.transform, true);
        helper.transform.localScale = Vector3.zero;

        var helperView = helper.GetComponent<ZNetView>();
        if (helperView != null)
            UnityEngine.Object.DestroyImmediate(helperView);

        helper.SetActive(true);

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

        // Five physical cargo goods belonging to this trader. Every one can later be
        // delivered to any of the other 13 regular traders; destination is not baked
        // into the item, only issuer + origin are stamped at purchase time.
        NativeCargoBridge.Clear();
        foreach (var cargoDef in TreasureRegistry.Treasures.Where(x => x.Id.StartsWith(definition.Id switch
        {
            "rudy_warg" => "rudy_",
            "mokra_dzika" => "mokra_",
            "ylva_frost" => "ylva_",
            "bjarki_goldtooth" => "bjarki_",
            "ragnar_turnipson" => "ragnar_",
            "spalony_zenek" => "zenek_",
            "skjold_cinderborn" => "skjold_",
            _ => definition.Id + "_"
        })))
        {
            var cargoPrefab = ObjectDB.instance?.GetItemPrefab($"ImmersiveTrader_{cargoDef.Id}");
            var cargoDrop = cargoPrefab?.GetComponent<ItemDrop>();
            if (cargoDrop == null) continue;

            var cargoItem = new Trader.TradeItem
            {
                m_prefab = cargoDrop, m_price = 10, m_stack = 1,
                m_requiredGlobalKey = string.Empty, m_levelUpEffect = false,
                m_buyPlayerEffects = new EffectList(), m_icon = null,
                m_name = cargoDrop.m_itemData?.m_shared?.m_name ?? cargoDef.DisplayName,
                m_tooltip = "Towar transportowy - 10 monet. Dostarcz dowolnemu innemu handlarzowi.",
                m_buyKey = string.Empty, m_incrementKey = string.Empty, m_incrementAmount = 0
            };
            trader.m_items.Add(cargoItem);
            NativeCargoBridge.Register(cargoItem, definition, npc.transform.position, cargoDef.Id);
        }

        if (trader.m_items.Count == 0)
            return false;

        gui.Show(trader);
        return true;
    }
}
