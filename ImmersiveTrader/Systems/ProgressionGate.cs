using System.Collections.Generic;

namespace ImmersiveTrader;

public static class ProgressionGate
{
    // Vanilla global keys from defeated bosses. Ashlands uses Fader.
    private static readonly Dictionary<int, string> RequiredBossKey = new()
    {
        { 1, "defeated_eikthyr" },
        { 2, "defeated_gdking" },
        { 3, "defeated_bonemass" },
        { 4, "defeated_dragon" },
        { 5, "defeated_goblinking" },
        { 6, "defeated_queen" }
    };

    public static bool CanReceiveTier(int targetTier)
    {
        if (targetTier <= 0 || !Plugin.ProgressionLock.Value)
            return true;

        return RequiredBossKey.TryGetValue(targetTier, out var key)
            && ZoneSystem.instance != null
            && ZoneSystem.instance.GetGlobalKey(key);
    }
}
