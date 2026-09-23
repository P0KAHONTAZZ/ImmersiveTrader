using System;
using System.Collections.Generic;

namespace ImmersiveTrader;

public static class DeliveryReputation
{
    private static readonly Dictionary<long, int> CompletedDeliveries = new();

    public static int GetCompleted(long playerId)
        => CompletedDeliveries.TryGetValue(playerId, out var value) ? value : 0;

    public static int GetBonusPercent(long playerId)
        => GetCompleted(playerId) * Math.Max(0, Plugin.ReputationBonusPerDeliveryPercent.Value);

    public static float GetMultiplier(long playerId)
        => 1f + GetBonusPercent(playerId) / 100f;

    public static void MarkCompleted(long playerId)
        => CompletedDeliveries[playerId] = GetCompleted(playerId) + 1;
}
