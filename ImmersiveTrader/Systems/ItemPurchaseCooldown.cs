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

    private static string WorldId()
    {
        var net = ZNet.instance ?? throw new InvalidOperationException("World is not loaded.");
        foreach (string method in new[] { "GetWorldUID", "GetWorldName" })
        {
            var info = net.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (info == null || info.GetParameters().Length != 0) continue;
            var value = info.Invoke(net, null)?.ToString();
            if (!string.IsNullOrEmpty(value)) return value;
        }
        throw new InvalidOperationException("Cannot identify the current world for purchase cooldowns.");
    }

    private static string Key(Player player, string trader, string kind, string item)
    {
        var plain = $"{WorldId()}|{player.GetPlayerID()}|{trader}|{kind}|{item}";
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plain));
    }

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
        lock (Sync)
        {
            Load();
            string key = Key(player, trader, kind, item);
            double now = ZNet.instance.GetTimeSeconds();
            Directory.CreateDirectory(Paths.ConfigPath);
            File.AppendAllText(FilePath, key + "\t" + now.ToString("R", CultureInfo.InvariantCulture) + Environment.NewLine);
            LastPurchase[key] = now;
        }
    }
}
