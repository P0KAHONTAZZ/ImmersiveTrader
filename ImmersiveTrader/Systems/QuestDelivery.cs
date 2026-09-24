using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class QuestDelivery
{
    public static bool TryDeliverAny(Player player, TraderDefinition target, Vector3 targetPosition)
    {
        // A shipment may go to any of the other 13 regular traders, never back to its issuer.
        var carried = InventoryTreasureService.GetCarried(player).FirstOrDefault(x => x.SourceTraderId != target.Id);
        if (carried == null) return false;

        bool targetTierUnlocked = target.IsLegendary || ProgressionGate.IsRewardTierUnlocked(target.BiomeTier);

        var table = target.IsLegendary ? LegendaryRewardRegistry.Rewards : RewardRegistry.Rewards;
        var reward = table.FirstOrDefault(x => x.TraderId == target.Id && x.TreasureId == carried.TreasureId);
        if (reward == null) return false;

        if (!targetTierUnlocked)
        {
            var sourceTrader = TraderRegistry.Traders.FirstOrDefault(x => x.Id == carried.SourceTraderId);
            var fallbackTrader = sourceTrader != null && ProgressionGate.IsRewardTierUnlocked(sourceTrader.BiomeTier)
                ? sourceTrader
                : TraderRegistry.Traders.FirstOrDefault(x => !x.IsLegendary && x.BiomeTier == 0);

            var fallback = fallbackTrader == null
                ? null
                : RewardRegistry.Rewards.FirstOrDefault(x => x.TraderId == fallbackTrader.Id && x.TreasureId == carried.TreasureId);

            if (fallback != null)
                reward = fallback;
        }

        var rewardPrefab = ObjectDB.instance.GetItemPrefab(reward.ItemPrefab);
        if (rewardPrefab == null)
        {
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: I cannot prepare your payment.");
            return true;
        }

        int biomeMultiplier = target.IsLegendary ? 1 : RewardScaling.GetBiomeMultiplier(carried.SourceBiomeTier, target.BiomeTier);
        float routeMetres = 0f;
        if (!target.IsLegendary && TreasureMetadata.TryGetSourcePosition(carried.Item, out float sourceX, out float sourceZ))
        {
            routeMetres = Vector2.Distance(new Vector2(sourceX, sourceZ), new Vector2(targetPosition.x, targetPosition.z));
        }
        float distanceMultiplier = target.IsLegendary ? 1f : RewardScaling.GetDistanceMultiplier(routeMetres);
        int amount = target.IsLegendary
            ? reward.BaseAmount
            : RewardScaling.GetRewardAmount(reward.BaseAmount, carried.SourceBiomeTier, target.BiomeTier, routeMetres);

        if (!player.GetInventory().CanAddItem(rewardPrefab, amount))
        {
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: Make room for your payment first.");
            return true;
        }

        player.GetInventory().RemoveItem(carried.Item);
        GiveReward(player, rewardPrefab, amount);

        string message = target.IsLegendary
            ? $"???: Those who trade in gold count coins. Those who trade in favors count roads. Received: {amount} {reward.ItemPrefab}"
            : target.LiesAboutRewards
                ? TroldadDialogue.GetLie(reward.ItemPrefab, amount)
                : $"{target.Name}: Deal. Biome x{biomeMultiplier}, distance {routeMetres:0}m x{distanceMultiplier:0.##}. Your payment: {amount} {reward.ItemPrefab}."
                + (targetTierUnlocked ? "" : " Better local stock unlocks after the previous biome boss.");

        player.Message(MessageHud.MessageType.Center, message);
        return true;
    }

    private static void GiveReward(Player player, GameObject prefab, int amount)
    {
        int left = amount;
        int stack = Mathf.Max(1, prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_maxStackSize);
        while (left > 0)
        {
            int give = Mathf.Min(stack, left);
            player.GetInventory().AddItem(prefab, give);
            left -= give;
        }
    }
}