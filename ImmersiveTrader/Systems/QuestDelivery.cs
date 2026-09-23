using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class QuestDelivery
{
    public static bool TryDeliverAny(Player player, TraderDefinition target)
    {
        var carried = InventoryTreasureService.GetCarried(player)
            .FirstOrDefault(x => x.SourceTraderId != target.Id);

        if (carried == null) return false;

        if (!ProgressionGate.CanReceiveTier(target.BiomeTier))
        {
            player.Message(MessageHud.MessageType.Center,
                $"{target.Name}: You are not ready for goods from this region yet.");
            return true;
        }

        var reward = RewardRegistry.Rewards.FirstOrDefault(x =>
            x.TraderId == target.Id && x.TreasureId == carried.TreasureId);
        if (reward == null) return false;

        var rewardPrefab = ObjectDB.instance.GetItemPrefab(reward.ItemPrefab);
        if (rewardPrefab == null)
        {
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: I cannot prepare your payment.");
            return true;
        }

        int amount = RewardScaling.GetRewardAmount(reward.BaseAmount, carried.SourceBiomeTier, target.BiomeTier);
        if (!player.GetInventory().CanAddItem(rewardPrefab, amount))
        {
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: Make room for your payment first.");
            return true;
        }

        player.GetInventory().RemoveItem(carried.Item);
        GiveReward(player, rewardPrefab, amount);

        string message = target.LiesAboutRewards
            ? TroldadDialogue.GetLie(reward.ItemPrefab, amount)
            : $"{target.Name}: Deal. Your payment: {amount} {reward.ItemPrefab}.";
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