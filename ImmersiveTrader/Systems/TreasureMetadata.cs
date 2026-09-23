using System;
using System.Globalization;

namespace ImmersiveTrader;

public static class TreasureMetadata
{
    private const string SourceTraderKey = "ImmersiveTrader.SourceTrader";
    private const string SourceTierKey = "ImmersiveTrader.SourceTier";
    private const string ShipmentIdKey = "ImmersiveTrader.ShipmentId";
    private const string SourceXKey = "ImmersiveTrader.SourceX";
    private const string SourceZKey = "ImmersiveTrader.SourceZ";

    public static void Stamp(ItemDrop.ItemData item, string sourceTraderId, int sourceBiomeTier, float sourceX = 0f, float sourceZ = 0f)
    {
        item.m_customData[SourceTraderKey] = sourceTraderId;
        item.m_customData[SourceTierKey] = sourceBiomeTier.ToString(CultureInfo.InvariantCulture);
        item.m_customData[ShipmentIdKey] = Guid.NewGuid().ToString("N");
        item.m_customData[SourceXKey] = sourceX.ToString(CultureInfo.InvariantCulture);
        item.m_customData[SourceZKey] = sourceZ.ToString(CultureInfo.InvariantCulture);
    }

    public static bool TryGetSourcePosition(ItemDrop.ItemData item, out float x, out float z)
    {
        x = 0f; z = 0f;
        return item.m_customData.TryGetValue(SourceXKey, out var sx)
            && item.m_customData.TryGetValue(SourceZKey, out var sz)
            && float.TryParse(sx, NumberStyles.Float, CultureInfo.InvariantCulture, out x)
            && float.TryParse(sz, NumberStyles.Float, CultureInfo.InvariantCulture, out z);
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
