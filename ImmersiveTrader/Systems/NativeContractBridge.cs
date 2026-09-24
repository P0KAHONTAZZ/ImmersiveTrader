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

        var prefab = ObjectDB.instance?.GetItemPrefab(ContractRegistry.PrefabName);
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
            x.m_dropPrefab != null && x.m_dropPrefab.name.StartsWith(ContractRegistry.PrefabName) && !before.Contains(x));
        if (created == null)
        {
            player.Message(MessageHud.MessageType.Center, $"{sale.Source.Name}: Contract creation failed.");
            return true;
        }

        ContractMetadata.Stamp(created, sale.Contract.Id, sale.Source.Id, TraderActivityService.GetWorldDayPublic());
        created.m_crafterName = sale.Contract.Title;
        player.Message(MessageHud.MessageType.Center,
            $"Contract accepted: {sale.Contract.Title} - {sale.Contract.RequiredAmount} targets | reward +{sale.Contract.RewardSkillLevels:0} {sale.Contract.RewardSkill}");
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
