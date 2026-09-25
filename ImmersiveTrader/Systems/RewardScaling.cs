using System;

namespace ImmersiveTrader;

public static class RewardScaling
{
    public static int GetBiomeMultiplier(int sourceBiomeTier, int targetBiomeTier)
    {
        int tierDistance = Math.Abs(sourceBiomeTier - targetBiomeTier);
        return tierDistance switch
        {
            0 => 1,
            1 => 2,
            2 => 4,
            _ => 6
        };
    }

    public static float GetDistanceMultiplier(float metres)
    {
        if (metres < Plugin.DistanceTier1Metres.Value) return 1f;
        if (metres < Plugin.DistanceTier2Metres.Value) return 1.25f;
        if (metres < Plugin.DistanceTier3Metres.Value) return 1.5f;
        if (metres < Plugin.DistanceTier4Metres.Value) return 1.75f;
        return 2f;
    }

    public static int GetRewardAmount(int baseAmount, int sourceBiomeTier, int targetBiomeTier, float metres)
        => Math.Max(1, (int)Math.Round(baseAmount * GetBiomeMultiplier(sourceBiomeTier, targetBiomeTier) * GetDistanceMultiplier(metres)));

    public static int GetMultiplier(int sourceBiomeTier, int targetBiomeTier)
        => GetBiomeMultiplier(sourceBiomeTier, targetBiomeTier);
}
