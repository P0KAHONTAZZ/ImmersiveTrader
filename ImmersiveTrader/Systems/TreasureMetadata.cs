using System;
using System.Globalization;

namespace ImmersiveTrader;

public static class TreasureMetadata
{
    private const string SourceTraderKey = "ImmersiveTrader.SourceTrader";
    private const string SourceTierKey = "ImmersiveTrader.SourceTier";
    private const string ShipmentIdKey = "ImmersiveTrader.ShipmentId";

    public static void Stamp(ItemDrop.ItemData item, string sourceTraderId, int sourceBiomeTier)
    {
        item.m_customData[SourceTraderKey] = sourceTraderId;
        item.m_customData[SourceTierKey] = sourceBiomeTier.ToString(CultureInfo.InvariantCulture);
        item.m_customData[ShipmentIdKey] = Guid.NewGuid().ToString("N");
    }

    public static string GetShipmentId(ItemDrop.ItemData item)
        => item.m_customData.TryGetValue(ShipmentIdKey, out var id) ? id : string.Empty;

    public static bool TryRead(ItemDrop.ItemData item, out string sourceTraderId, out int sourceBiomeTier)
    {
        sourceTraderId = string.Empty;
        sourceBiomeTier = 0;

        return item.m_customData.TryGetValue(SourceTraderKey, out sourceTraderId)
            && item.m_customData.TryGetValue(SourceTierKey, out var tier)
            && int.TryParse(tier, NumberStyles.Integer, CultureInfo.InvariantCulture, out sourceBiomeTier);
    }
}
