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
    private static readonly Dictionary<(long PlayerId, string TraderId), int> LastAcceptedDay = new();
    private static readonly HashSet<(long PlayerId, string ContractId)> Completed = new();

    public static TraderActivityDefinition? GetOffer(string traderId) =>
        TraderActivityRegistry.Activities.FirstOrDefault(x => x.TraderId == traderId);

    public static TraderActivityDefinition[] GetOffers(Player player, string traderId)
    {
        long id = player.GetPlayerID();
        return TraderActivityRegistry.Activities
            .Where(x => x.TraderId == traderId && !Completed.Contains((id, x.Id)))
            .ToArray();
    }

    public static bool TryAccept(Player player, string traderId)
    {
        long playerId = player.GetPlayerID();
        int activeForTrader = Active.Values.Count(x => x.Definition.TraderId == traderId &&
            Active.Any(a => a.Key.PlayerId == playerId && ReferenceEquals(a.Value, x)));
        if (activeForTrader >= 2)
        {
            player.Message(MessageHud.MessageType.Center, "You already have 2 active contracts from this trader.");
            return false;
        }

        int today = GetWorldDay();
        if (LastAcceptedDay.TryGetValue((playerId, traderId), out int lastDay) && today - lastDay < 7)
        {
            player.Message(MessageHud.MessageType.Center, $"Next contract available in {7 - (today - lastDay)} world day(s).");
            return false;
        }

        var offer = GetOffers(player, traderId)
            .FirstOrDefault(x => !Active.ContainsKey((playerId, x.Id)));
        if (offer == null)
        {
            player.Message(MessageHud.MessageType.Center, "All contracts from this trader are complete.");
            return false;
        }
        Active[(playerId, offer.Id)] = new State { Definition = offer };
        LastAcceptedDay[(playerId, traderId)] = today;
        player.Message(MessageHud.MessageType.Center, $"Contract accepted: {offer.Title} (0/{offer.RequiredAmount}) | reward +{offer.RewardSkillLevels:0} {offer.RewardSkill}");
        return true;
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
        Completed.Add((playerId, state.Definition.Id));
        Active.Remove(entry.Key);

        int done = TraderActivityRegistry.Activities.Count(x => x.TraderId == traderId &&
            Completed.Contains((playerId, x.Id)));
        player.Message(MessageHud.MessageType.Center,
            $"Contract complete: +{state.Definition.RewardSkillLevels:0} {state.Definition.RewardSkill} | trader contracts {done}/5");
        return true;
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
        int done = TraderActivityRegistry.Activities.Count(x => x.TraderId == traderId && Completed.Contains((playerId, x.Id)));
        if (active.Length == 0)
            return done >= 5 ? "All contracts complete (5/5)." : $"No active task. Contracts complete: {done}/5.";

        return string.Join(" | ", active.Select(x =>
            $"{x.Definition.Title}: {x.Progress}/{x.Definition.RequiredAmount} (+{x.Definition.RewardSkillLevels:0} {x.Definition.RewardSkill})")) +
            $" | complete {done}/5";
    }

    private static int GetWorldDay()
    {
        if (ZNet.instance == null) return 0;
        return Mathf.FloorToInt((float)(ZNet.instance.GetTimeSeconds() / 1800.0));
    }

    private static bool PrefabMatches(string actual, string expected) =>
        actual == expected || actual.StartsWith(expected + "(Clone)");
}
