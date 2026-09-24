using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

/// <summary>70 cargo goods x 13 other regular traders = 910 delivery routes.</summary>
public static class RewardRegistry
{
    public static readonly List<RewardDefinition> Rewards = new();
    public static void Initialize()
    {
        Rewards.Clear();

        Plugin.Log.LogInfo($"Cargo routes initialized: {Rewards.Count}/910");
    }

    public static void Validate()
    {
        int missing = 0;
        var seen = new HashSet<string>();
        foreach (var reward in Rewards)
        {
            string key = reward.TraderId + "|" + reward.TreasureId;
            if (!seen.Add(key)) Plugin.Log.LogWarning($"Duplicate cargo route: {key}");
            if (ObjectDB.instance?.GetItemPrefab(reward.ItemPrefab) != null) continue;
            missing++;
            Plugin.Log.LogWarning($"Cargo reward prefab missing: {reward.ItemPrefab} ({reward.TreasureId} -> {reward.TraderId})");
        }
        Plugin.Log.LogInfo($"Cargo routes ready: {Rewards.Count}; unique: {seen.Count}; missing reward prefabs: {missing}");
    }

    private static void Add(string traderId, string treasureId, string itemPrefab, int baseAmount)
        => Rewards.Add(new RewardDefinition(traderId, treasureId, itemPrefab, baseAmount));
}
