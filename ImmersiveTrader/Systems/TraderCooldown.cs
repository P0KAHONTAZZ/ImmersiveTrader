using System;

namespace ImmersiveTrader;

public static class TraderCooldown
{
    private const string KeyPrefix = "ImmersiveTrader_Cooldown_";

    public static int CurrentWorldDay()
    {
        if (ZNet.instance == null) return 0;
        return Math.Max(0, (int)(ZNet.instance.GetTimeSeconds() / 1800d));
    }

    public static bool CanIssue(Player player, string traderId, out int daysRemaining)
    {
        daysRemaining = 0;
        string value = player.GetCustomData(KeyPrefix + traderId);
        if (string.IsNullOrEmpty(value) || !int.TryParse(value, out int issued))
            return true;

        int elapsed = CurrentWorldDay() - issued;
        daysRemaining = Math.Max(0, Plugin.TraderCooldownWorldDays.Value - elapsed);
        return daysRemaining <= 0;
    }

    public static void MarkIssued(Player player, string traderId)
        => player.SetCustomData(KeyPrefix + traderId, CurrentWorldDay().ToString());
}
