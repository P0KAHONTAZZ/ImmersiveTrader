using System.Collections.Generic;
using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderActivityService
{
    private sealed class State
    {
        public TraderActivityDefinition Definition = null!;
        public int Progress;
        public bool Completed => Progress >= Definition.RequiredAmount;
    }

    private static readonly Dictionary<(long PlayerId, string ContractId), State> Active = new();
    private static readonly Dictionary<(long PlayerId, string ContractId), int> LastAcceptedDay = new();
    private static readonly HashSet<(long PlayerId, string ContractId)> CompletedOnce = new();

    public static TraderActivityDefinition? GetOffer(string traderId) =>
        TraderActivityRegistry.Activities.FirstOrDefault(x => x.TraderId == traderId);

    public static TraderActivityDefinition[] GetOffers(Player player, string traderId)
    {
        long id = player.GetPlayerID();
        return TraderActivityRegistry.Activities
            .Where(x => x.TraderId == traderId)
            .ToArray();
    }

    public static bool TryAccept(Player player, string traderId)
    {
        long playerId = player.GetPlayerID();
        int activeTotal = Active.Count(x => x.Key.PlayerId == playerId);
        if (activeTotal >= 5)
        {
            player.Message(MessageHud.MessageType.Center, "You already have 5 active contracts in total.");
            return false;
        }

        int activeForTrader = Active.Count(x => x.Key.PlayerId == playerId && x.Value.Definition.TraderId == traderId);
        if (activeForTrader >= 2)
        {
            player.Message(MessageHud.MessageType.Center, "You already have 2 active contracts from this trader.");
            return false;
        }

        int today = GetWorldDay();
        var offer = GetOffers(player, traderId)
            .Where(x => !Active.ContainsKey((playerId, x.Id)))
            .FirstOrDefault(x => !LastAcceptedDay.TryGetValue((playerId, x.Id), out int lastDay) || today - lastDay >= 7);
        if (offer == null)
        {
            player.Message(MessageHud.MessageType.Center, "No contract is currently available; active contracts or weekly cooldowns are blocking the remaining offers.");
            return false;
        }
        Active[(playerId, offer.Id)] = new State { Definition = offer };
        LastAcceptedDay[(playerId, offer.Id)] = today;
        player.Message(MessageHud.MessageType.Center, $"Contract accepted: {offer.Title} (0/{offer.RequiredAmount}) | reward +{offer.RewardSkillLevels:0} {offer.RewardSkill}");
        return true;
    }


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

    public static bool TryTurnIn(Player player, string traderId)
    {
        long playerId = player.GetPlayerID();
        var entry = Active.FirstOrDefault(x => x.Key.PlayerId == playerId &&
            x.Value.Definition.TraderId == traderId && x.Value.Completed);
        if (entry.Value == null)
        {
            var running = Active.FirstOrDefault(x => x.Key.PlayerId == playerId && x.Value.Definition.TraderId == traderId);
            if (running.Value == null) return false;
            player.Message(MessageHud.MessageType.Center,
                $"{running.Value.Definition.Title}: {running.Value.Progress}/{running.Value.Definition.RequiredAmount}");
            return true;
        }

        var state = entry.Value;
        player.RaiseSkill(state.Definition.RewardSkill, state.Definition.RewardSkillLevels);
        CompletedOnce.Add((playerId, state.Definition.Id));
        Active.Remove(entry.Key);

        int activeTotal = Active.Count(x => x.Key.PlayerId == playerId);
        player.Message(MessageHud.MessageType.Center,
            $"Contract complete: +{state.Definition.RewardSkillLevels:0} {state.Definition.RewardSkill} | active contracts {activeTotal}/5");
        return true;
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
            if (definition == null || progress >= definition.RequiredAmount || !PrefabMatches(prefabName, definition.TargetPrefab))
                continue;

            progress = Mathf.Min(progress + 1, definition.RequiredAmount);
            ContractMetadata.SetProgress(item, progress);
            player.Message(MessageHud.MessageType.TopLeft, $"{definition.Title}: {progress}/{definition.RequiredAmount}");
        }
    }

    public static void RegisterKill(Player player, string prefabName)
    {
        long playerId = player.GetPlayerID();
        foreach (var state in Active.Where(x => x.Key.PlayerId == playerId).Select(x => x.Value))
        {
            if (state.Completed || !PrefabMatches(prefabName, state.Definition.TargetPrefab)) continue;
            state.Progress = Mathf.Min(state.Progress + 1, state.Definition.RequiredAmount);
            player.Message(MessageHud.MessageType.TopLeft, $"{state.Definition.Title}: {state.Progress}/{state.Definition.RequiredAmount}");
        }
    }

    public static string GetStatus(Player player, string traderId)
    {
        long playerId = player.GetPlayerID();
        var active = Active.Where(x => x.Key.PlayerId == playerId && x.Value.Definition.TraderId == traderId)
            .Select(x => x.Value).ToArray();
        int activeTotal = Active.Count(x => x.Key.PlayerId == playerId);

        if (active.Length == 0)
            return $"No active contract from this trader. Active contracts overall: {activeTotal}/5.";

        return string.Join(" | ", active.Select(x =>
            $"{x.Definition.Title}: {x.Progress}/{x.Definition.RequiredAmount} (+{x.Definition.RewardSkillLevels:0} {x.Definition.RewardSkill})")) +
            $" | active overall {activeTotal}/5";
    }

    public static int GetWorldDayPublic() => GetWorldDay();

    private static int GetWorldDay()
    {
        if (ZNet.instance == null) return 0;
        return Mathf.FloorToInt((float)(ZNet.instance.GetTimeSeconds() / 1800.0));
    }

    private static bool PrefabMatches(string actual, string expected) =>
        actual == expected || actual.StartsWith(expected + "(Clone)");
}
