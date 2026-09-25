using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;

namespace ImmersiveTrader;

/// <summary>One trust gift per character, world, and trader; retried when inventory is full.</summary>
public static class TraderTrustReward
{
    public sealed record Gift(string Prefab, string Name);
    public static readonly IReadOnlyDictionary<string, Gift> Gifts = new Dictionary<string, Gift>
    {
        ["midka"] = new("HelmetDverger", "Dverger Circlet"),
        ["troldad"] = new("MeadTamer", "Brew of Animal Whispers"),
        ["grimvald"] = new("BeltStrength", "Megingjord"),
        ["rudy_warg"] = new("HelmetRoot", "Root Mask"),
        ["mokra_dzika"] = new("CapeWolf", "Wolf Fur Cape"),
        ["encek"] = new("Wishbone", "Wishbone"),
        ["hrothgar"] = new("SaddleLox", "Lox Saddle"),
        ["ylva_frost"] = new("ArmorFenringChest", "Fenris Coat"),
        ["bjarki_goldtooth"] = new("GrapplingHook", "Grappling Hook"),
        ["ragnar_turnipson"] = new("SaddleLox", "Lox Saddle"),
        ["cmok"] = new("CapeFeather", "Feather Cape"),
        ["grelka"] = new("Demister", "Wisplight"),
        ["spalony_zenek"] = new("SaddleAsksvin", "Asksvin Saddle"),
        ["skjold_cinderborn"] = new("SaddleMoose", "Moose Saddle")
    };

    private static readonly object Sync = new();
    private static readonly HashSet<string> Claimed = new();
    private static bool loaded;
    private static string FilePath => Path.Combine(Paths.ConfigPath, "ImmersiveTrader-trust-gifts.txt");

    private static void Load()
    {
        if (loaded) return;
        if (File.Exists(FilePath))
            foreach (var key in File.ReadAllLines(FilePath))
                if (!string.IsNullOrWhiteSpace(key)) Claimed.Add(key.Trim());
        loaded = true;
    }

    public static void TryGrant(Player player, string trader)
    {
        if (TraderReputation.GetLevel(player, trader) < 5 || !Gifts.TryGetValue(trader, out var gift))
            return;
        if (!ProgressionGate.IsRewardTierUnlocked(
                TraderRegistry.Traders.First(x => x.Id == trader).BiomeTier)) return;
        lock (Sync)
        {
            Load();
            string key = TraderReputation.RewardIdentity(player, trader);
            if (Claimed.Contains(key)) return;
            var prefab = ObjectDB.instance?.GetItemPrefab(gift.Prefab);
            if (prefab == null)
            {
                Plugin.Log.LogWarning($"Trust gift item missing for {trader}: {gift.Prefab}");
                return;
            }
            var inventory = player.GetInventory();
            if (!inventory.CanAddItem(prefab, 1))
            {
                player.Message(MessageHud.MessageType.Center,
                    "Trusted reward waiting: clear one inventory slot, then speak to this trader again.");
                return;
            }
            if (!inventory.AddItem(prefab, 1)) return;
            try
            {
                Directory.CreateDirectory(Paths.ConfigPath);
                File.AppendAllText(FilePath, key + Environment.NewLine);
                Claimed.Add(key);
                player.Message(MessageHud.MessageType.Center,
                    $"Trusted reward: {gift.Name} added to your inventory.");
            }
            catch (Exception error)
            {
                Plugin.Log.LogError($"Could not record trust gift for {trader}: {error}");
                var itemName = prefab.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_name;
                if (!string.IsNullOrEmpty(itemName)) inventory.RemoveItem(itemName, 1);
                player.Message(MessageHud.MessageType.Center,
                    "Trusted reward could not be saved; speak to this trader again later.");
            }
        }
    }
}
