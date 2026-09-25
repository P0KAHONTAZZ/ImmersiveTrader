using System.Collections.Generic;

namespace ImmersiveTrader;

public static class ProgressionGate
{
    // A biome's normal reward pool becomes available after the previous biome boss is defeated.
    private static readonly Dictionary<int, string> RequiredPreviousBossKey = new()
    {
        { 1, "defeated_eikthyr" },
        { 2, "defeated_gdking" },
        { 3, "defeated_bonemass" },
        { 4, "defeated_dragon" },
        { 5, "defeated_goblinking" },
        { 6, "defeated_queen" }
    };

    public static bool IsRewardTierUnlocked(int rewardTier)
    {
        if (rewardTier <= 0 || !Plugin.ProgressionLock.Value)
            return true;

        return RequiredPreviousBossKey.TryGetValue(rewardTier, out var key)
            && ZoneSystem.instance != null
            && ZoneSystem.instance.GetGlobalKey(key);
    }

    // Kept for compatibility with older call sites.
    public static bool CanReceiveTier(int targetTier) => IsRewardTierUnlocked(targetTier);
}
