using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class NativeContractBridge
{
    private sealed record ContractSale(TraderDefinition Source, TraderActivityDefinition Contract);
    private static readonly Dictionary<Trader.TradeItem, ContractSale> Sales = new();

    public static void Clear() => Sales.Clear();
    public static bool IsContract(Trader.TradeItem item) => Sales.ContainsKey(item);
    public static void Register(Trader.TradeItem item, TraderDefinition source, TraderActivityDefinition contract)
        => Sales[item] = new ContractSale(source, contract);

    public static bool TryHandle(Player player, Trader.TradeItem item)
    {
        if (!Sales.TryGetValue(item, out var sale)) return false;

        if (sale.Contract.TraderId != sale.Source.Id)
        {
            Plugin.Log.LogWarning($"Rejected contract sale with mismatched issuer: {sale.Contract.Id}, contract={sale.Contract.TraderId}, shop={sale.Source.Id}");
            player.Message(MessageHud.MessageType.Center, $"{sale.Source.Name}: This contract is not mine to issue.");
            return true;
        }

        try
        {
            if (!ItemPurchaseCooldown.CanBuy(player, sale.Source.Id, "contract", sale.Contract.Id, out int days))
            {
                player.Message(MessageHud.MessageType.Center, $"{sale.Source.Name}: {sale.Contract.Title} available in {days} Valheim day(s).");
                return true;
            }
        }
        catch (System.Exception ex)
        {
            Plugin.Log.LogError($"Contract cooldown check failed: {ex}");
            player.Message(MessageHud.MessageType.Center, "Contract unavailable: cooldown data could not be read.");
            return true;
        }

        if (TraderActivityService.CountPhysicalContracts(player, sale.Source.Id) >= 2)
        {
            player.Message(MessageHud.MessageType.Center, $"{sale.Source.Name}: Finish one of my two contracts first.");
            return true;
        }

        bool alreadyCarried = player.GetInventory().GetAllItems().Any(x =>
            ContractMetadata.TryRead(x, out string id, out _, out _) && id == sale.Contract.Id);
        if (alreadyCarried)
        {
            player.Message(MessageHud.MessageType.Center, $"{sale.Source.Name}: You already carry this contract.");
            return true;
        }

        var prefab = ObjectDB.instance?.GetItemPrefab(ContractRegistry.GetPrefabName(sale.Contract.Id));
        if (prefab == null || !player.GetInventory().CanAddItem(prefab, 1))
        {
            player.Message(MessageHud.MessageType.Center, $"{sale.Source.Name}: Make room for the contract first.");
            return true;
        }

        var before = player.GetInventory().GetAllItems().ToArray();
        if (!player.GetInventory().AddItem(prefab, 1))
        {
            player.Message(MessageHud.MessageType.Center, $"{sale.Source.Name}: Could not issue the contract.");
            return true;
        }

        var created = player.GetInventory().GetAllItems().FirstOrDefault(x =>
            x.m_dropPrefab != null && x.m_dropPrefab.name.StartsWith(prefab.name) && !before.Contains(x));
        if (created == null)
        {
            // Never leave an untracked generic scroll behind if inventory hooks changed
            // the insertion semantics unexpectedly.
            var stray = player.GetInventory().GetAllItems().FirstOrDefault(x =>
                x.m_dropPrefab != null && x.m_dropPrefab.name.StartsWith(prefab.name) &&
                !ContractMetadata.TryRead(x, out _, out _, out _));
            if (stray != null) player.GetInventory().RemoveItem(stray);
            player.Message(MessageHud.MessageType.Center, $"{sale.Source.Name}: Contract creation failed.");
            return true;
        }

        // Re-check the per-issuer limit immediately before stamping. Inventory hooks may
        // have inserted another contract while AddItem was running.
        if (TraderActivityService.CountPhysicalContracts(player, sale.Source.Id) >= 2)
        {
            player.GetInventory().RemoveItem(created);
            player.Message(MessageHud.MessageType.Center, $"{sale.Source.Name}: Contract limit changed; issuance cancelled.");
            return true;
        }

        bool rare = false;
        int reputationLevel = TraderReputation.GetLevel(player, sale.Source.Id);
        int required = ContractRequirementScaling.Scale(sale.Contract.RequiredAmount, reputationLevel);
        ContractMetadata.Stamp(created, sale.Contract.Id, sale.Source.Id, TraderActivityService.GetWorldDayPublic(), required, rare, sale.Contract.RewardSkill);
        try { ItemPurchaseCooldown.MarkBought(player, sale.Source.Id, "contract", sale.Contract.Id); }
        catch (System.Exception ex)
        {
            Plugin.Log.LogError($"Contract cooldown save failed: {ex}");
            player.GetInventory().RemoveItem(created);
            player.Message(MessageHud.MessageType.Center, "Contract issuance cancelled: cooldown data could not be saved.");
            return true;
        }
        created.m_crafterName = sale.Contract.Title;
        Plugin.Log.LogInfo($"Contract scroll issued: {sale.Contract.Id}, issuer={sale.Source.Id}, " +
            $"inventoryItems={player.GetInventory().GetAllItems().Count}, " +
            $"validScrolls={TraderActivityService.CountPhysicalContracts(player, sale.Source.Id)}");
        player.Message(MessageHud.MessageType.Center,
            $"Contract added to your inventory: {sale.Contract.Title}. Target: {required} x {sale.Contract.TargetPrefab}. Reward: +{sale.Contract.RewardSkillLevels:0} levels ({sale.Contract.RewardSkill}).");
        Components.TraderBanter.PurchaseAtShop();
        return true;
    }
}

[HarmonyPatch(typeof(StoreGui), "BuySelectedItem")]
internal static class NativeContractBuyPatch
{
    private static bool Prefix(StoreGui __instance)
    {
        var player = Player.m_localPlayer;
        if (player == null) return true;
        var selected = AccessTools.Field(typeof(StoreGui), "m_selectedItem")?.GetValue(__instance) as Trader.TradeItem;
        if (selected == null) return true;
        return !NativeContractBridge.TryHandle(player, selected);
    }
}
