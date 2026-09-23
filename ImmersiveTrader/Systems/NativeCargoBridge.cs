using System.Collections.Generic;
using HarmonyLib;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>Handles only cargo rows added to Valheim's native StoreGui.</summary>
public static class NativeCargoBridge
{
    private sealed record CargoSale(TraderDefinition Source, Vector3 Position, string TreasureId);
    private static readonly Dictionary<Trader.TradeItem, CargoSale> Sales = new();

    public static void Clear() => Sales.Clear();

    public static void Register(Trader.TradeItem item, TraderDefinition source, Vector3 position, string treasureId)
        => Sales[item] = new CargoSale(source, position, treasureId);

    public static bool TryHandle(Player player, Trader.TradeItem item)
    {
        if (!Sales.TryGetValue(item, out var sale)) return false;
        return QuestIssuing.TryBuyTreasure(player, sale.Source, sale.Position, sale.TreasureId, 10);
    }
}

[HarmonyPatch(typeof(StoreGui), "BuySelectedItem")]
internal static class NativeCargoBuyPatch
{
    private static bool Prefix(StoreGui __instance)
    {
        var player = Player.m_localPlayer;
        if (player == null) return true;
        var selected = AccessTools.Field(typeof(StoreGui), "m_selectedItem")?.GetValue(__instance) as Trader.TradeItem;
        if (selected == null) return true;
        return !NativeCargoBridge.TryHandle(player, selected);
    }
}
