using System;
using System.Collections.Generic;

namespace ImmersiveTrader;

public static class TraderCooldown
{
    private static readonly Dictionary<(long Player, string Trader), int> LastIssueDay = new();

    public static int CurrentWorldDay()
    {
        if (ZNet.instance == null) return 0;
        // Valheim day length is 1800 seconds. Network time keeps clients aligned.
        return Math.Max(0, (int)(ZNet.instance.GetTimeSeconds() / 1800d));
    }

    public static bool CanIssue(long playerId, string traderId, out int daysRemaining)
    {
        daysRemaining = 0;
        if (!LastIssueDay.TryGetValue((playerId, traderId), out int issued))
            return true;

        int elapsed = CurrentWorldDay() - issued;
        daysRemaining = Math.Max(0, Plugin.TraderCooldownWorldDays.Value - elapsed);
        return daysRemaining <= 0;
    }

    public static void MarkIssued(long playerId, string traderId)
        => LastIssueDay[(playerId, traderId)] = CurrentWorldDay();
}
