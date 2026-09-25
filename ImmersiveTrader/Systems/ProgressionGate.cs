using System.Collections.Generic;
using UnityEngine;
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

    // Vanilla characters from older worlds often have no personal boss defeat keys.
    // Known materials are saved on the character; a later-biome material proves
    // access to all earlier traders without reading this server's world keys.
    private static readonly Dictionary<int, string[]> MaterialEvidence = new()
    {
        { 1, new[] { "CopperOre", "TinOre", "Bronze" } },
        { 2, new[] { "IronScrap", "Iron" } },
        { 3, new[] { "SilverOre", "Silver" } },
        { 4, new[] { "BlackMetalScrap", "BlackMetal" } },
        { 5, new[] { "Carapace", "Sap", "BlackMarble" } },
        { 6, new[] { "FlametalOreNew", "Flametal", "Blackwood" } }
    };

    private static Player _cachedPlayer;
    private static float _nextMaterialCheck;
    private static int _cachedMaterialTier;

    public static int HighestKnownMaterialTier(Player player)
    {
        if (player == null || ObjectDB.instance == null) return 0;
        if (_cachedPlayer == player && Time.time < _nextMaterialCheck) return _cachedMaterialTier;
        _cachedPlayer = player;
        _nextMaterialCheck = Time.time + 5f;
        _cachedMaterialTier = 0;
        for (int tier = 6; tier >= 1; tier--)
        {
            if (!MaterialEvidence.TryGetValue(tier, out var prefabs)) continue;
            foreach (var prefabName in prefabs)
            {
                var item = ObjectDB.instance.GetItemPrefab(prefabName)?.GetComponent<ItemDrop>();
                var materialName = item?.m_itemData?.m_shared?.m_name;
                if (!string.IsNullOrEmpty(materialName) && player.IsKnownMaterial(materialName))
                {
                    _cachedMaterialTier = tier;
                    return tier;
                }
            }
        }
        return 0;
    }

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
            && (player.HaveUniqueKey(key) || HighestKnownMaterialTier(player) >= rewardTier);
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
