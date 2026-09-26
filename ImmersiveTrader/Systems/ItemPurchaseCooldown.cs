using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using BepInEx;

namespace ImmersiveTrader;

/// <summary>Purchase cooldown per world, character, merchant and item (7 Valheim days by default; scrolls 3).</summary>
public static class ItemPurchaseCooldown
{
    private const double DaySeconds = 1800d;
    private static readonly object Sync = new();
    private static readonly Dictionary<string, double> LastPurchase = new();
    private static bool loaded;
    private static string FilePath => Path.Combine(Paths.ConfigPath, "ImmersiveTrader-item-cooldowns.txt");

    private static string Key(Player player, string trader, string kind, string item)
        => MultiplayerPlayerState.Key(player, trader, kind, item);

    private static void Load()
    {
        if (loaded) return;
        if (File.Exists(FilePath))
            foreach (string line in File.ReadAllLines(FilePath))
            {
                int tab = line.IndexOf('\t');
                if (tab <= 0 || !double.TryParse(line.Substring(tab + 1), NumberStyles.Float,
                        CultureInfo.InvariantCulture, out double seconds) || double.IsNaN(seconds) || double.IsInfinity(seconds)) continue;
                LastPurchase[line.Substring(0, tab)] = seconds;
            }
        loaded = true;
    }

    public static bool CanBuy(Player player, string trader, string kind, string item, out int daysRemaining)
        => CanBuy(player, trader, kind, item, 7d, out daysRemaining);

    public static bool CanBuy(Player player, string trader, string kind, string item, double cooldownDays, out int daysRemaining)
    {
        if (ZNet.instance != null && !ZNet.instance.IsServer())
        {
            if (MultiplayerNetwork.TryGetCachedCooldown(player, trader, kind, item, out daysRemaining))
                return daysRemaining <= 0;
            MultiplayerNetwork.RequestCooldown(player, trader, kind, item, cooldownDays);
            // Unknown state is fail-closed until the server answers. This prevents a
            // remote client from treating missing local config as permission to buy.
            daysRemaining = 1;
            return false;
        }
        return CanBuyAuthoritative(player, trader, kind, item, cooldownDays, out daysRemaining);
    }

    internal static bool CanBuyAuthoritative(Player player, string trader, string kind, string item, double cooldownDays, out int daysRemaining)
    {
        double cooldownSeconds = cooldownDays * DaySeconds;
        lock (Sync)
        {
            Load();
            string key = Key(player, trader, kind, item);
            double now = ZNet.instance.GetTimeSeconds();
            if (LastPurchase.TryGetValue(key, out double issued) && now - issued < cooldownSeconds)
            {
                daysRemaining = Math.Max(1, (int)Math.Ceiling((cooldownSeconds - (now - issued)) / DaySeconds));
                return false;
            }
            daysRemaining = 0;
            return true;
        }
    }

    public static void MarkBought(Player player, string trader, string kind, string item)
    {
        MultiplayerAuthority.RequireServer("purchase cooldown mutation");
        lock (Sync)
        {
            Load();
            string key = Key(player, trader, kind, item);
            double now = ZNet.instance.GetTimeSeconds();
            Directory.CreateDirectory(Paths.ConfigPath);
            File.AppendAllText(FilePath, key + "\t" + now.ToString("R", CultureInfo.InvariantCulture) + Environment.NewLine);
            LastPurchase[key] = now;
            MultiplayerNetwork.InvalidateCooldown(player, trader, kind, item);
        }
    }
}
