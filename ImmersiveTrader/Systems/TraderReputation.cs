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
    // Five cargo offers and five contracts can be completed per trader in each seven-day purchase cycle.
    // Carrying two shipments at a time does not limit the number of deliveries or reputation awards.
    // Typical pace: three routes one biome apart and two contracts = 16 points per cycle.
    // Five routes one biome apart and five contracts = 30 points per cycle.
    public const int ContractPoints = 2;
    public const int Maximum = 64;

    public static int CargoPoints(int sourceBiomeTier, int targetBiomeTier)
        => 2 * (Math.Min(4, Math.Abs(sourceBiomeTier - targetBiomeTier)) + 1);

    private static readonly object Sync = new();
    private static readonly Dictionary<string, int> Points = new();
    private static bool loaded;
    private static string FilePath => Path.Combine(Paths.ConfigPath, "ImmersiveTrader-reputation.txt");

    private static string Key(Player player, string trader)
        => MultiplayerPlayerState.Key(player, trader);

    private static void Load()
    {
        if (loaded) return;
        if (File.Exists(FilePath))
            foreach (string line in File.ReadAllLines(FilePath))
            {
                int tab = line.IndexOf('\t');
                if (tab <= 0) continue;
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

    public static int GetLevel(Player player, string trader) => Level(Get(player, trader));

    internal static string RewardIdentity(Player player, string trader) => Key(player, trader);

    public static int Add(Player player, string trader, int amount)
    {
        MultiplayerAuthority.RequireServer("reputation mutation");
        if (amount <= 0) return Get(player, trader);
        int next;
        lock (Sync)
        {
            Load();
            string key = Key(player, trader);
            int current = Points.TryGetValue(key, out int value) ? value : 0;
            next = Math.Min(Maximum, current + amount);
            if (next != current)
            {
                Directory.CreateDirectory(Paths.ConfigPath);
                File.AppendAllText(FilePath, key + "\t" + next.ToString(CultureInfo.InvariantCulture) + Environment.NewLine);
                Points[key] = next;
            }
        }
        return next;
    }

    public static int Reset(Player player, string trader)
    {
        MultiplayerAuthority.RequireServer("reputation reset");
        lock (Sync)
        {
            Load();
            string key = Key(player, trader);
            int current = Points.TryGetValue(key, out int value) ? value : 0;
            if (current == 0) return 0;
            Directory.CreateDirectory(Paths.ConfigPath);
            File.AppendAllText(FilePath, key + "\t0" + Environment.NewLine);
            Points[key] = 0;
            return current;
        }
    }

    public static string DescribeStanding(Player player, string trader)
    {
        int level = Level(Get(player, trader));
        string stars = new string('★', level);
        string emptyStars = new string('☆', 5 - level);
        return $"<color=yellow>{stars}</color><color=#888888>{emptyStars}</color> <color=yellow>{LevelName(level)}</color>";
    }

    public static string Describe(Player player, string trader)
    {
        int points = Get(player, trader);
        int level = Level(points);
        return $"{points}/{Maximum} (level {level}/5: {LevelName(level)})";
    }

    private static int Level(int points)
        => points >= 64 ? 5 : points >= 44 ? 4 : points >= 28 ? 3 : points >= 12 ? 2 : 1;

    private static string LevelName(int level) => level switch
    {
        1 => "Skeptical",
        2 => "Neutral",
        3 => "Favorable",
        4 => "Friendly",
        _ => "Trusted"
    };
}
