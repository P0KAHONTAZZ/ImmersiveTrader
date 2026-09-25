using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class QuestDelivery
{
    public static bool TryDeliverAny(Player player, TraderDefinition target, Vector3 targetPosition)
    {
        // A shipment may go to any of the other 13 regular traders, never back to its issuer.
        var carried = InventoryTreasureService.GetCarried(player).FirstOrDefault(x =>
            x.SourceTraderId != target.Id &&
            RewardRegistry.Rewards.Any(r => r.TraderId == target.Id && r.TreasureId == x.TreasureId));
        if (carried == null) return false;

        string expectedSource = TreasureRegistry.GetOwnerTraderId(carried.TreasureId);
        if (string.IsNullOrEmpty(expectedSource) || expectedSource != carried.SourceTraderId)
        {
            Plugin.Log.LogWarning($"Rejected cargo with invalid issuer metadata: {carried.TreasureId}, stamped={carried.SourceTraderId}, expected={expectedSource}");
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: This shipment has invalid papers.");
            return true;
        }

        bool targetTierUnlocked = target.IsLegendary || ProgressionGate.IsRewardTierUnlocked(target.BiomeTier);

        var table = target.IsLegendary ? LegendaryRewardRegistry.Rewards : RewardRegistry.Rewards;
        var reward = table.FirstOrDefault(x => x.TraderId == target.Id && x.TreasureId == carried.TreasureId);
        if (reward == null)
        {
            Plugin.Log.LogWarning($"No cargo route: {carried.TreasureId} ({carried.SourceTraderId}) -> {target.Id}");
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: I have no contract for this shipment.");
            return true;
        }

        if (!targetTierUnlocked)
        {
            // There is intentionally no issuer -> issuer route. Pick the highest unlocked
            // regular destination that has a real route for this cargo instead.
            var fallback = TraderRegistry.Traders
                .Where(x => !x.IsLegendary &&
                            x.Id != carried.SourceTraderId &&
                            ProgressionGate.IsRewardTierUnlocked(x.BiomeTier))
                .OrderByDescending(x => x.BiomeTier)
                .ThenBy(x => x.Id)
                .Select(x => RewardRegistry.Rewards.FirstOrDefault(r =>
                    r.TraderId == x.Id && r.TreasureId == carried.TreasureId))
                .FirstOrDefault(x => x != null);

            if (fallback == null)
            {
                player.Message(MessageHud.MessageType.Center, $"{target.Name}: Your progression does not unlock a safe payment for this shipment yet.");
                return true;
            }

            reward = fallback;
        }

        var rewardPrefab = ObjectDB.instance.GetItemPrefab(reward.ItemPrefab);
        if (rewardPrefab == null)
        {
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: I cannot prepare your payment.");
            return true;
        }

        float biomeMultiplier = target.IsLegendary ? 1f : RewardScaling.GetBiomeMultiplier(carried.SourceBiomeTier, target.BiomeTier);
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

        // Reward capacity and prefab were verified above. Remove exactly the stamped
        // shipment selected for this delivery, then grant the route reward.
        var shipmentSnapshot = TreasureMetadata.Capture(carried.Item);
        player.GetInventory().RemoveItem(carried.Item);
        if (!GiveReward(player, rewardPrefab, amount))
        {
            // Defensive rollback: this should be unreachable after CanAddItem, but never
            // consume a shipment if the inventory API unexpectedly rejects the payment.
            if (player.GetInventory().AddItem(carried.Item.m_dropPrefab, 1))
            {
                var restored = player.GetInventory().GetAllItems().LastOrDefault(x =>
                    x.m_dropPrefab == carried.Item.m_dropPrefab &&
                    !TreasureMetadata.TryRead(x, out _, out _));
                if (restored != null)
                {
                    TreasureMetadata.Restore(restored, shipmentSnapshot);
                }
            }
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: Payment failed; shipment was returned.");
            return true;
        }

        int previousReputation = TraderReputation.Get(player, carried.SourceTraderId);
        int reputation = TraderReputation.Add(player, carried.SourceTraderId,
            TraderReputation.CargoPoints(carried.SourceBiomeTier, target.BiomeTier));

        string message = target.IsLegendary
            ? $"???: Those who trade in gold count coins. Those who trade in favors count roads. Received: {amount} {reward.ItemPrefab}"
            : target.LiesAboutRewards
                ? TroldadDialogue.GetLie(reward.ItemPrefab, amount)
                : $"{target.Name}: Deal. Biome x{biomeMultiplier:0.##}, distance {routeMetres:0}m x{distanceMultiplier:0.##}. Your payment: {amount} {reward.ItemPrefab}."
                + (targetTierUnlocked ? "" : " Better local stock unlocks after the previous biome boss.");

        player.Message(MessageHud.MessageType.Center, message + $" Sender reputation: +{reputation - previousReputation} ({reputation}/{TraderReputation.Maximum}).");
        return true;
    }

    private static bool GiveReward(Player player, GameObject prefab, int amount)
    {
        int left = amount;
        int stack = Mathf.Max(1, prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_maxStackSize);
        while (left > 0)
        {
            int give = Mathf.Min(stack, left);
            if (!player.GetInventory().AddItem(prefab, give))
                return false;
            left -= give;
        }
        return true;
    }
}