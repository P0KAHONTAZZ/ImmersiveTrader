using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderActivityService
{
    public static bool TryTurnInPhysical(Player player, string traderId)
    {
        foreach (var item in player.GetInventory().GetAllItems().ToArray())
        {
            if (!ContractMetadata.TryRead(item, out string contractId, out string issuer, out int progress) || issuer != traderId)
                continue;

            var definition = TraderActivityRegistry.Activities.FirstOrDefault(x => x.Id == contractId && x.TraderId == traderId);
            if (definition == null)
            {
                Plugin.Log.LogWarning($"Rejected physical contract with invalid metadata: {contractId}, issuer={issuer}");
                player.Message(MessageHud.MessageType.Center, $"{traderId}: This contract has invalid papers.");
                return true;
            }
            int required = ContractMetadata.GetRequiredAmount(item, definition.RequiredAmount);
            if (progress < required)
                continue;

            var rewardSkill = ContractMetadata.GetRewardSkill(item, contractId, definition.RewardSkill);
            if (!ContractSkillReward.TryGrant(player, rewardSkill, (int)definition.RewardSkillLevels, out float before, out float after))
            {
                player.Message(MessageHud.MessageType.Center, $"{traderId}: Cannot grant {rewardSkill} levels (skill unavailable or already at maximum). Contract retained.");
                return true;
            }
            player.GetInventory().RemoveItem(item);
            player.Message(MessageHud.MessageType.Center,
                $"Kontrakt ukończony: {rewardSkill} {before:0.##} → {after:0.##} (+{after - before:0.##} poziomy).");
            return true;
        }

        return false;
    }

    public static int CountPhysicalContracts(Player player, string traderId)
    {
        return player.GetInventory().GetAllItems().Count(item =>
            ContractMetadata.TryRead(item, out _, out string issuer, out _) && issuer == traderId);
    }

    public static void RegisterKillOnPhysicalContracts(Player player, string prefabName)
    {
        foreach (var item in player.GetInventory().GetAllItems())
        {
            if (!ContractMetadata.TryRead(item, out string contractId, out _, out int progress))
                continue;

            var definition = TraderActivityRegistry.Activities.FirstOrDefault(x => x.Id == contractId);
            if (definition == null || definition.TraderId != GetIssuer(item) ||
                progress >= ContractMetadata.GetRequiredAmount(item, definition.RequiredAmount) || !PrefabMatches(prefabName, definition.TargetPrefab))
                continue;

            int required = ContractMetadata.GetRequiredAmount(item, definition.RequiredAmount);
            progress = Mathf.Min(progress + 1, required);
            ContractMetadata.SetProgress(item, progress);
            player.Message(MessageHud.MessageType.TopLeft, $"{definition.Title}: {progress}/{required}");
        }
    }

    public static string GetStatus(Player player, string traderId)
    {
        var physical = player.GetInventory().GetAllItems()
            .Select(item =>
            {
                if (!ContractMetadata.TryRead(item, out string contractId, out string issuer, out int progress) || issuer != traderId)
                    return null;
                var definition = TraderActivityRegistry.Activities.FirstOrDefault(x => x.Id == contractId && x.TraderId == issuer);
                return definition == null ? null : $"{definition.Title}: {progress}/{ContractMetadata.GetRequiredAmount(item, definition.RequiredAmount)}";
            })
            .Where(x => x != null)
            .ToArray();

        return physical.Length == 0 ? "No active task." : string.Join(" | ", physical);
    }

    public static int GetWorldDayPublic() => GetWorldDay();

    private static int GetWorldDay()
    {
        if (ZNet.instance == null) return 0;
        return Mathf.FloorToInt((float)(ZNet.instance.GetTimeSeconds() / 1800.0));
    }

    private static string GetIssuer(ItemDrop.ItemData item)
    {
        ContractMetadata.TryRead(item, out _, out string issuer, out _);
        return issuer;
    }

    private static bool PrefabMatches(string actual, string expected) =>
        actual == expected || actual.StartsWith(expected + "(Clone)");
}
