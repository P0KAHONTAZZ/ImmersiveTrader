using System;

namespace ImmersiveTrader;

public static class RewardScaling
{
    public static float GetBiomeMultiplier(int sourceBiomeTier, int targetBiomeTier)
    {
        int tierDistance = Math.Abs(sourceBiomeTier - targetBiomeTier);
        return tierDistance switch
        {
            0 => 1f,
            1 => 1.25f,
            2 => 1.5f,
            3 => 1.75f,
            _ => 2f
        };
    }

    public static float GetDistanceMultiplier(float metres)
    {
        if (metres < 1000f) return 1f;
        if (metres < 2000f) return 1.3f;
        if (metres < 3000f) return 1.6f;
        if (metres < 4000f) return 1.9f;
        if (metres < 7500f) return 2.2f;
        return 2.5f;
    }

    public static int GetRewardAmount(int baseAmount, int sourceBiomeTier, int targetBiomeTier, float metres)
        => Math.Max(1, (int)Math.Round(baseAmount * GetBiomeMultiplier(sourceBiomeTier, targetBiomeTier) * GetDistanceMultiplier(metres)));

    public static float GetMultiplier(int sourceBiomeTier, int targetBiomeTier)
        => GetBiomeMultiplier(sourceBiomeTier, targetBiomeTier);
}
