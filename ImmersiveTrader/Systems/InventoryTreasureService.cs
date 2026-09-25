using System.Collections.Generic;
using System.Linq;

namespace ImmersiveTrader;

public static class InventoryTreasureService
{
    public sealed record CarriedTreasure(
        ItemDrop.ItemData Item,
        string TreasureId,
        string SourceTraderId,
        int SourceBiomeTier,
        string ShipmentId);

    public static IReadOnlyList<CarriedTreasure> GetCarried(Player player)
    {
        var result = new List<CarriedTreasure>();

        foreach (var item in player.GetInventory().GetAllItems())
        {
            if (!TryGetTreasureId(item, out var treasureId))
                continue;

            if (!TreasureMetadata.TryRead(item, out var sourceTrader, out var sourceTier))
                continue;

            result.Add(new CarriedTreasure(
                item,
                treasureId,
                sourceTrader,
                sourceTier,
                TreasureMetadata.GetShipmentId(item)));
        }

        return result;
    }

    public static int Count(Player player) => GetCarried(player).Count;

    public static bool TryGetTreasureId(ItemDrop.ItemData item, out string treasureId)
    {
        treasureId = string.Empty;
        string prefab = item.m_dropPrefab != null ? item.m_dropPrefab.name : string.Empty;
        const string prefix = "ImmersiveTrader_";
        if (!prefab.StartsWith(prefix))
            return false;

        string candidate = prefab.Substring(prefix.Length).Replace("(Clone)", string.Empty);
        bool known = TreasureRegistry.Treasures.Any(x => x.Id == candidate);
        if (known) treasureId = candidate;
        return known;
    }
}
