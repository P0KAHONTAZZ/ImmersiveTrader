using System.Collections.Generic;

namespace ImmersiveTrader;

public static class QuestState
{
    public sealed record ActiveTreasure(string TreasureId, string SourceTraderId, int SourceBiomeTier);

    private static readonly Dictionary<long, List<ActiveTreasure>> ActiveByPlayer = new();

    public static IReadOnlyList<ActiveTreasure> GetActive(long playerId)
        => ActiveByPlayer.TryGetValue(playerId, out var list) ? list : System.Array.Empty<ActiveTreasure>();

    public static bool CanTake(long playerId)
        => GetActive(playerId).Count < Plugin.MaxCarriedTreasures.Value;

    public static void Add(long playerId, ActiveTreasure treasure)
    {
        if (!ActiveByPlayer.TryGetValue(playerId, out var list))
        {
            list = new List<ActiveTreasure>();
            ActiveByPlayer[playerId] = list;
        }

        list.Add(treasure);
    }

    public static bool TryRemove(long playerId, string treasureId, out ActiveTreasure? removed)
    {
        removed = null;
        if (!ActiveByPlayer.TryGetValue(playerId, out var list))
            return false;

        int index = list.FindIndex(x => x.TreasureId == treasureId);
        if (index < 0) return false;

        removed = list[index];
        list.RemoveAt(index);
        return true;
    }
}