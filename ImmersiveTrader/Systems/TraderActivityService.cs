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

    private static readonly Dictionary<(long PlayerId, string TraderId), State> Active = new();
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
        var key = (player.GetPlayerID(), traderId);
        if (Active.ContainsKey(key)) return false;
        var offer = GetOffers(player, traderId).FirstOrDefault();
        if (offer == null)
        {
            player.Message(MessageHud.MessageType.Center, "All contracts from this trader are complete.");
            return false;
        }
        Active[key] = new State { Definition = offer };
        player.Message(MessageHud.MessageType.Center, $"Contract accepted: {offer.Title} (0/{offer.RequiredAmount}) | reward +{offer.RewardSkillLevels:0} {offer.RewardSkill}");
        return true;
    }

    public static bool TryTurnIn(Player player, string traderId)
    {
        var key = (player.GetPlayerID(), traderId);
        if (!Active.TryGetValue(key, out var state)) return false;
        if (!state.Completed)
        {
            player.Message(MessageHud.MessageType.Center, $"{state.Definition.Title}: {state.Progress}/{state.Definition.RequiredAmount}");
            return true;
        }

        player.RaiseSkill(state.Definition.RewardSkill, state.Definition.RewardSkillLevels);
        Completed.Add((player.GetPlayerID(), state.Definition.Id));
        Active.Remove(key);

        int done = TraderActivityRegistry.Activities.Count(x => x.TraderId == traderId &&
            Completed.Contains((player.GetPlayerID(), x.Id)));
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
        if (Active.TryGetValue((playerId, traderId), out var state))
            return $"{state.Definition.Title}: {state.Progress}/{state.Definition.RequiredAmount} | +{state.Definition.RewardSkillLevels:0} {state.Definition.RewardSkill}";

        int done = TraderActivityRegistry.Activities.Count(x => x.TraderId == traderId && Completed.Contains((playerId, x.Id)));
        return done >= 5 ? "All contracts complete (5/5)." : $"No active task. Contracts complete: {done}/5.";
    }

    private static bool PrefabMatches(string actual, string expected) =>
        actual == expected || actual.StartsWith(expected + "(Clone)");
}
