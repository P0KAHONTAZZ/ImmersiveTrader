using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using BepInEx;

namespace ImmersiveTrader;

/// <summary>Reputation belongs to the character and world, separately for each issuing trader.</summary>
public static class TraderReputation
{
    // Three delivered shipments earn as much reputation as one finished contract.
    // Three shipments and two contracts per seven-day cycle yield nine points;
    // four cycles reach the 36-point cap.
    public const int CargoPoints = 1;
    public const int ContractPoints = 3;
    public const int Maximum = 36;

    private static readonly object Sync = new();
    private static readonly Dictionary<string, int> Points = new();
    private static readonly Dictionary<string, List<double>> Events = new();
    private static bool loaded;
    private static string FilePath => Path.Combine(Paths.ConfigPath, "ImmersiveTrader-reputation.txt");

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
        throw new InvalidOperationException("Cannot identify the current world for reputation.");
    }

    private static string Key(Player player, string trader)
    {
        string plain = $"{WorldId()}|{player.GetPlayerID()}|{trader}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plain));
    }

    private static void Load()
    {
        if (loaded) return;
        if (File.Exists(FilePath))
            foreach (string line in File.ReadAllLines(FilePath))
            {
                int tab = line.IndexOf('\t');
                if (tab <= 0) continue;
                if (line[0] == 'E')
                {
                    if (double.TryParse(line.Substring(tab + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out double stamp))
                    {
                        string eventKey = line.Substring(1, tab - 1);
                        if (!Events.TryGetValue(eventKey, out var list)) Events[eventKey] = list = new List<double>();
                        list.Add(stamp);
                    }
                    continue;
                }
                if (!int.TryParse(line.Substring(tab + 1), NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int value)) continue;
                Points[line.Substring(0, tab)] = Math.Clamp(value, 0, Maximum);
            }
        loaded = true;
    }

    public static int Get(Player player, string trader)
    {
        lock (Sync)
        {
            Load();
            return Points.TryGetValue(Key(player, trader), out int value) ? value : 0;
        }
    }

    public static int Add(Player player, string trader, int amount)
    {
        if (amount <= 0) return Get(player, trader);
        lock (Sync)
        {
            Load();
            string key = Key(player, trader);
            int current = Points.TryGetValue(key, out int value) ? value : 0;
            string eventKey = key + "|" + amount;
            double now = ZNet.instance.GetTimeSeconds();
            int quota = amount == CargoPoints ? 3 : amount == ContractPoints ? 2 : 0;
            if (quota == 0) return current;
            if (!Events.TryGetValue(eventKey, out var history)) Events[eventKey] = history = new List<double>();
            history.RemoveAll(stamp => stamp > now || now - stamp >= 7d * 1800d);
            if (history.Count >= quota) return current;
            int next = Math.Min(Maximum, current + amount);
            if (next == current) return next;
            Directory.CreateDirectory(Paths.ConfigPath);
            File.AppendAllText(FilePath,
                "E" + eventKey + "\t" + now.ToString("R", CultureInfo.InvariantCulture) + Environment.NewLine +
                key + "\t" + next.ToString(CultureInfo.InvariantCulture) + Environment.NewLine);
            history.Add(now);
            Points[key] = next;
            return next;
        }
    }

    public static string Describe(Player player, string trader)
    {
        int points = Get(player, trader);
        int level = Math.Min(4, points / 9);
        return $"{points}/{Maximum} (poziom {level}/4)";
    }
}
