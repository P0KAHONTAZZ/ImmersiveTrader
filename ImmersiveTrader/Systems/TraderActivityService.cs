using System.Collections.Generic;
using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Runtime side-task state. Deliberately isolated behind this service so the current
/// in-memory prototype can later be replaced by ZDO/server persistence without changing UI.
/// </summary>
public static class TraderActivityService
{
    private sealed class State
    {
        public TraderActivityDefinition Definition = null!;
        public int Progress;
        public bool Completed => Progress >= Definition.RequiredAmount;
    }

    private static readonly Dictionary<(long PlayerId, string TraderId), State> Active = new();

    public static TraderActivityDefinition? GetOffer(string traderId) =>
        TraderActivityRegistry.Activities.FirstOrDefault(x => x.TraderId == traderId);

    public static bool TryAccept(Player player, string traderId)
    {
        var key = (player.GetPlayerID(), traderId);
        if (Active.ContainsKey(key)) return false;
        var offer = GetOffer(traderId);
        if (offer == null) return false;
        Active[key] = new State { Definition = offer };
        player.Message(MessageHud.MessageType.Center, $"Task accepted: {offer.Title} (0/{offer.RequiredAmount})");
        return true;
    }

    public static bool TryTurnIn(Player player, string traderId)
    {
        var key = (player.GetPlayerID(), traderId);
        if (!Active.TryGetValue(key, out var state)) return false;

        if (state.Definition.Type == TraderActivityType.Gather)
        {
            int have = player.GetInventory().CountItems(state.Definition.TargetPrefab);
            state.Progress = Mathf.Min(have, state.Definition.RequiredAmount);
        }

        if (!state.Completed)
        {
            player.Message(MessageHud.MessageType.Center,
                $"{state.Definition.Title}: {state.Progress}/{state.Definition.RequiredAmount}");
            return true;
        }

        if (state.Definition.Type == TraderActivityType.Gather)
            player.GetInventory().RemoveItem(state.Definition.TargetPrefab, state.Definition.RequiredAmount);

        var reward = ObjectDB.instance?.GetItemPrefab(state.Definition.RewardPrefab);
        if (reward == null) return true;
        if (!player.GetInventory().CanAddItem(reward, state.Definition.RewardAmount))
        {
            player.Message(MessageHud.MessageType.Center, "Make room for the task reward first.");
            return true;
        }

        player.GetInventory().AddItem(reward, state.Definition.RewardAmount);
        player.Message(MessageHud.MessageType.Center,
            $"Task complete: {state.Definition.RewardAmount}x {state.Definition.RewardPrefab}");
        Active.Remove(key);
        return true;
    }

    public static void RegisterKill(Player player, string prefabName)
    {
        long playerId = player.GetPlayerID();
        foreach (var state in Active.Where(x => x.Key.PlayerId == playerId).Select(x => x.Value))
        {
            if (state.Definition.Type != TraderActivityType.Hunt || state.Completed) continue;
            if (!PrefabMatches(prefabName, state.Definition.TargetPrefab)) continue;
            state.Progress = Mathf.Min(state.Progress + 1, state.Definition.RequiredAmount);
            player.Message(MessageHud.MessageType.TopLeft,
                $"{state.Definition.Title}: {state.Progress}/{state.Definition.RequiredAmount}");
        }
    }

    public static string GetStatus(Player player, string traderId)
    {
        if (!Active.TryGetValue((player.GetPlayerID(), traderId), out var state))
            return "No active task.";
        int progress = state.Definition.Type == TraderActivityType.Gather
            ? Mathf.Min(player.GetInventory().CountItems(state.Definition.TargetPrefab), state.Definition.RequiredAmount)
            : state.Progress;
        return $"{state.Definition.Title}: {progress}/{state.Definition.RequiredAmount}";
    }

    private static bool PrefabMatches(string actual, string expected) =>
        actual == expected || actual.StartsWith(expected + "(Clone)");
}
