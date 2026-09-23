using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class LegendaryRewardRegistry
{
    // Intentionally conservative until final balancing. These are high-value utility
    // materials, not raw metal, and still obey progression checks.
    public static readonly IReadOnlyList<RewardDefinition> Rewards = new List<RewardDefinition>
    {
        new("legendary_mieteg", "ancient_parcel", "Feathers", 100),
        new("legendary_mieteg", "carved_idol", "FineWood", 80),
        new("legendary_mieteg", "sealed_mead_cask", "RoyalJelly", 30),
        new("legendary_mieteg", "merchants_gem", "YggdrasilWood", 50),
        new("legendary_mieteg", "runic_ledger", "BlackMarble", 60)
    };
}
