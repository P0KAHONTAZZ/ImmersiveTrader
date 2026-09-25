using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class ProgressionGate
{
    // Boss kills are stored as unique keys on each character by Valheim.
    // A different character on the same server must not inherit world progression.
    private static readonly Dictionary<int, string> RequiredPreviousBossKey = new()
    {
        { 1, "defeated_eikthyr" },
        { 2, "defeated_gdking" },
        { 3, "defeated_bonemass" },
        { 4, "defeated_dragon" },
        { 5, "defeated_goblinking" },
        { 6, "defeated_queen" }
    };

    private static string AccessKey(string traderId) => "ImmersiveTrader_access_" + traderId;

    public static bool HasAccess(Player player, string traderId)
        => player != null && player.HaveUniqueKey(AccessKey(traderId));

    public static void GrantAccess(Player player, string traderId)
        => player.AddUniqueKey(AccessKey(traderId));

    public static void RevokeAccess(Player player, string traderId)
        => player.RemoveUniqueKey(AccessKey(traderId));

    public static bool IsRewardTierUnlocked(Player player, int rewardTier)
    {
        if (rewardTier <= 0 || !Plugin.ProgressionLock.Value) return true;
        return player != null && RequiredPreviousBossKey.TryGetValue(rewardTier, out var key)
            && player.HaveUniqueKey(key);
    }

    public static bool CanAccessTrader(Player player, TraderDefinition trader)
        => HasAccess(player, trader.Id) || IsRewardTierUnlocked(player, trader.BiomeTier);

    public static bool CanAccessOffer(Player player, TraderOfferDefinition offer)
        => HasAccess(player, offer.TraderId) || IsRewardTierUnlocked(player, offer.RequiredTier);

    // Legacy callers use the local character; never fall back to the world key.
    public static bool IsRewardTierUnlocked(int rewardTier)
        => IsRewardTierUnlocked(Player.m_localPlayer, rewardTier);

    public static bool CanReceiveTier(int targetTier)
        => IsRewardTierUnlocked(Player.m_localPlayer, targetTier);
}
