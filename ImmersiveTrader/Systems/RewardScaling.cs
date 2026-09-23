using System;

namespace ImmersiveTrader;

public static class RewardScaling
{
    public static int GetMultiplier(int sourceBiomeTier, int targetBiomeTier)
    {
        int distance = Math.Abs(sourceBiomeTier - targetBiomeTier);
        return distance switch
        {
            0 => 1,
            1 => 2,
            2 => 4,
            _ => 6
        };
    }

    public static int GetRewardAmount(int baseAmount, int sourceBiomeTier, int targetBiomeTier)
        => baseAmount * GetMultiplier(sourceBiomeTier, targetBiomeTier);
}