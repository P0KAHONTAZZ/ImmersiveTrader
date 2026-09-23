using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class QuestDelivery
{
    public static bool TryDeliverAny(Player player, TraderDefinition target)
    {
        long playerId = player.GetPlayerID();
        var active = QuestState.GetActive(playerId)
            .FirstOrDefault(x => x.SourceTraderId != target.Id && HasTreasure(player, x.TreasureId));

        if (active == null)
            return false;

        var reward = RewardRegistry.Rewards.FirstOrDefault(x =>
            x.TraderId == target.Id && x.TreasureId == active.TreasureId);

        if (reward == null)
            return false;

        var rewardPrefab = ObjectDB.instance.GetItemPrefab(reward.ItemPrefab);
        if (rewardPrefab == null)
        {
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: I cannot prepare your payment.");
            return false;
        }

        int amount = RewardScaling.GetRewardAmount(reward.BaseAmount, active.SourceBiomeTier, target.BiomeTier);
        if (!player.GetInventory().CanAddItem(rewardPrefab, amount))
        {
            player.Message(MessageHud.MessageType.Center, $"{target.Name}: Make room for your payment first.");
            return false;
        }

        string treasurePrefab = $"ImmersiveTrader_{active.TreasureId}";
        player.GetInventory().RemoveItem(treasurePrefab, 1);
        GiveReward(player, rewardPrefab, amount);
        QuestState.TryRemove(playerId, active.TreasureId, out _);

        string message = target.LiesAboutRewards
            ? TroldadDialogue.GetLie(reward.ItemPrefab, amount)
            : $"{target.Name}: Deal. Your payment: {amount} {reward.ItemPrefab}.";

        player.Message(MessageHud.MessageType.Center, message);
        return true;
    }

    private static bool HasTreasure(Player player, string treasureId)
        => player.GetInventory().CountItems($"ImmersiveTrader_{treasureId}") > 0;

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
