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
            if (definition == null) continue;
            if (progress < definition.RequiredAmount)
            {
                player.Message(MessageHud.MessageType.Center, $"{definition.Title}: {progress}/{definition.RequiredAmount}");
                return true;
            }

            player.GetInventory().RemoveItem(item);
            player.RaiseSkill(definition.RewardSkill, definition.RewardSkillLevels);
            player.Message(MessageHud.MessageType.Center,
                $"Contract complete: +{definition.RewardSkillLevels:0} {definition.RewardSkill}");
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
                progress >= definition.RequiredAmount || !PrefabMatches(prefabName, definition.TargetPrefab))
                continue;

            progress = Mathf.Min(progress + 1, definition.RequiredAmount);
            ContractMetadata.SetProgress(item, progress);
            player.Message(MessageHud.MessageType.TopLeft, $"{definition.Title}: {progress}/{definition.RequiredAmount}");
        }
    }

    public static string GetStatus(Player player, string traderId)
    {
        var physical = player.GetInventory().GetAllItems()
            .Select(item =>
            {
                if (!ContractMetadata.TryRead(item, out string contractId, out string issuer, out int progress) || issuer != traderId)
                    return null;
                var definition = TraderActivityRegistry.Activities.FirstOrDefault(x => x.Id == contractId);
                return definition == null ? null : $"{definition.Title}: {progress}/{definition.RequiredAmount}";
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
