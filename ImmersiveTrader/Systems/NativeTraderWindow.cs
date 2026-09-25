using System;
using System.Linq;
using System.Collections.Generic;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Opens Valheim's native StoreGui using a temporary Trader component populated
/// from ImmersiveTrader's curated stock. The NPC shell itself stays custom.
/// </summary>
public static class NativeTraderWindow
{
    private static GameObject? activeHelper;
    private static readonly Dictionary<Trader.TradeItem, TraderOfferDefinition> OrdinarySales = new();
    public static bool TryGetOrdinarySale(Trader.TradeItem row, out TraderOfferDefinition offer)
    {
        if (OrdinarySales.TryGetValue(row, out var found))
        {
            offer = found;
            return true;
        }
        offer = null!;
        return false;
    }
    public static GameObject? ActiveNpc => activeHelper == null ? null : activeHelper.transform.parent?.gameObject;

    public static void Close()
    {
        OrdinarySales.Clear();
        NativeCargoBridge.Clear();
        NativeContractBridge.Clear();
        if (activeHelper != null)
            UnityEngine.Object.Destroy(activeHelper);
        activeHelper = null;
    }

    public static bool TryOpen(Player player, TraderDefinition definition, GameObject npc)
    {
        // StoreGui may have been closed by another UI path; remove stale helpers before
        // registering rows for the new window.
        Close();
        var gui = StoreGui.instance;
        if (gui == null)
        {
            player.Message(MessageHud.MessageType.Center, "Trader window is not ready.");
            return false;
        }

        int reputationLevel = TraderReputation.GetLevel(player, definition.Id);
        bool tierUnlocked = ProgressionGate.CanAccessTrader(player, definition);
        var offers = TraderShop.GetAvailableOffers(player, definition.Id);
        Plugin.Log.LogInfo($"Shop access {definition.Id}: reputationLevel={reputationLevel}, tierUnlocked={tierUnlocked}, offers={offers.Length}.");

        // Never AddComponent<Trader>() to our NPC: Trader.Awake/Update expects a fully
        // authored vanilla trader hierarchy (talk points, dialogue lists, effects, etc.).
        // Clone an initialized vanilla Trader onto a disabled helper instead.
        // Hildir provides the female trader interaction audio for our three women;
        // the other traders keep Haldor's existing interaction audio.
        string templateName = definition.Id is "mokra_dzika" or "ylva_frost" or "grelka"
            ? "Hildir" : "Haldor";
        var source = ZNetScene.instance?.GetPrefab(templateName);
        var template = source?.GetComponent<Trader>();
        if (template == null)
        {
            player.Message(MessageHud.MessageType.Center, "Native trader template is unavailable.");
            return false;
        }

        // Instantiate the helper while its vanilla source prefab is disabled. Otherwise its
        // ZNetView.Awake can create a persistent vanilla trader ZDO before we hide it,
        // which then reappears after the next world load.
        bool sourceWasActive = source.activeSelf;
        GameObject helper;
        try
        {
            source.SetActive(false);
            helper = UnityEngine.Object.Instantiate(source, npc.transform.position + Vector3.down * 1000f, npc.transform.rotation);
        }
        finally
        {
            source.SetActive(sourceWasActive);
        }

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
        foreach (var cargoDef in TreasureRegistry.GetForTrader(definition.Id))
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
                m_tooltip = $"Trade shipment. Deliver it to another trader.\nOrigin: <color=yellow>{definition.Name}</color>\nPrice: <color=yellow>10 coins</color>",
                m_buyKey = string.Empty, m_incrementKey = string.Empty, m_incrementAmount = 0
            };
            trader.m_items.Add(cargoItem);
            NativeCargoBridge.Register(cargoItem, definition, npc.transform.position, cargoDef.Id);
        }

        // Five free hunting contracts share the same native list. Selecting one invokes
        // our bridge instead of vanilla's coin purchase and creates a persistent scroll.
        NativeContractBridge.Clear();
        if (ObjectDB.instance != null)
        {
            foreach (var contract in TraderActivityRegistry.Activities.Where(x => x.TraderId == definition.Id))
            {
                var contractPrefab = ObjectDB.instance.GetItemPrefab(ContractRegistry.GetPrefabName(contract.Id))?.GetComponent<ItemDrop>();
                if (contractPrefab?.m_itemData?.m_shared == null) continue;
                var icon = ContractIconRegistry.ForSkill(contract.RewardSkill);
                if (icon != null) contractPrefab.m_itemData.m_shared.m_icons = new[] { icon };
                if (contractPrefab.m_itemData.m_shared.m_icons == null ||
                    contractPrefab.m_itemData.m_shared.m_icons.Length == 0 ||
                    contractPrefab.m_itemData.m_shared.m_icons[0] == null) continue;

                var contractItem = new Trader.TradeItem
                {
                    m_prefab = contractPrefab, m_price = 0, m_stack = 1,
                    m_requiredGlobalKey = string.Empty, m_levelUpEffect = false,
                    m_buyPlayerEffects = new EffectList(), m_icon = icon ?? contractPrefab.m_itemData.m_shared.m_icons[0],
                    m_name = contract.Title,
                    m_tooltip = $"Hunting contract. Carry this scroll while hunting and return it to the issuing trader.\nTarget: <color=yellow>{contract.RequiredAmount} x {contract.TargetPrefab}</color>\nReward: <color=yellow>+{contract.RewardSkillLevels:0} levels ({contract.RewardSkill})</color>\nIssued by: <color=yellow>{definition.Name}</color>",
                    m_buyKey = string.Empty, m_incrementKey = string.Empty, m_incrementAmount = 0
                };
                trader.m_items.Add(contractItem);
                NativeContractBridge.Register(contractItem, definition, contract);
            }
        }
        else
            Plugin.Log.LogWarning("Contract shop rows skipped: ObjectDB is unavailable.");

        if (trader.m_items.Count == 0)
        {
            UnityEngine.Object.Destroy(helper);
            NativeCargoBridge.Clear();
            NativeContractBridge.Clear();
            return false;
        }

        int cargoRows = trader.m_items.Count(x => NativeCargoBridge.IsCargo(x));
        int contractRows = trader.m_items.Count(x => NativeContractBridge.IsContract(x));
        Plugin.Log.LogInfo($"Native shop {definition.Id}: {offers.Length} ordinary configured, {cargoRows} cargo rows, {contractRows} contract rows, {trader.m_items.Count} total rows.");

        activeHelper = helper;
        try
        {
            gui.Show(trader);
        }
        catch
        {
            Close();
            throw;
        }
        // The gift is delivered only after the issuer's shop opens successfully.
        // Delivery or contract turn-in at another NPC never triggers a gift.
        try { TraderTrustReward.TryGrant(player, definition.Id); }
        catch (Exception error) { Plugin.Log.LogWarning($"Trust gift delivery deferred for {definition.Id}: {error}"); }
        return true;
    }
}
