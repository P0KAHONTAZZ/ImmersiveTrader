using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Reputation level 5 ("Trusted") gift bundle, once per character, world and trader:
/// 1x Scroll of Rested + 1x random enhancement scroll (never Rested, never the scroll this
/// trader sells at level 5) + the trader's own items below. Retried when inventory is full.
/// </summary>
public static class TraderTrustReward
{
    public sealed record Gift(string Prefab, string Name, int Quantity = 1);
    private const string GiftVersion = "|trust-gift-v4";

    public static readonly IReadOnlyDictionary<string, Gift[]> Gifts = new Dictionary<string, Gift[]>
    {
        ["midka"] = new[] { new Gift("MeadHealthMajor", "Major Healing Mead", 5) },
        ["troldad"] = new[] { new Gift("MeadTamer", "Brew of Animal Whispers") },
        ["grimvald"] = new[] { new Gift("IronScrap", "Scrap Iron", 30) },
        ["rudy_warg"] = Array.Empty<Gift>(), // scrolls only
        ["mokra_dzika"] = new[] { new Gift("SilverOre", "Silver Ore", 30) },
        ["encek"] = new[] { new Gift("SilverOre", "Silver Ore", 30),
                            new Gift(EnhancementScrollItems.PrefabName("endurance"), "Scroll of Endurance") },
        ["hrothgar"] = new[] { new Gift("SaddleLox", "Lox Saddle") },
        ["ylva_frost"] = new[] { new Gift("BlackMetalScrap", "Black Metal Scrap", 30) },
        ["bjarki_goldtooth"] = new[] { new Gift("GrapplingHook", "Grappling Hook") },
        ["ragnar_turnipson"] = new[] { new Gift("SaddleLox", "Lox Saddle") },
        ["cmok"] = new[] { new Gift("MoltenCore", "Molten Core", 5) },
        ["grelka"] = new[] { new Gift("FlametalOreNew", "Flametal Ore", 10) },
        ["spalony_zenek"] = new[] { new Gift("SaddleAsksvin", "Asksvin Saddle") },
        ["skjold_cinderborn"] = new[] { new Gift("SaddleMoose", "Moose Saddle") }
    };

    /// <summary>Full bundle for this trader, with the random scroll resolved now.</summary>
    private static List<Gift> Bundle(string trader)
    {
        var bundle = new List<Gift> { new(EnhancementScrollItems.PrefabName("rested"), "Scroll of Rested") };
        TraderShopRegistry.ScrollOffer.TryGetValue(trader, out var sold);
        var pool = EnhancementScrollItems.RegisteredIds
            .Where(id => id != "rested" && id != sold).ToList();
        if (pool.Count > 0)
        {
            string pick = pool[UnityEngine.Random.Range(0, pool.Count)];
            bundle.Add(new Gift(EnhancementScrollItems.PrefabName(pick), "Scroll of " + EnhancementStatusRegistry.DisplayName(pick)));
        }
        if (Gifts.TryGetValue(trader, out var own)) bundle.AddRange(own);
        return bundle;
    }

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
        if (TraderReputation.GetLevel(player, trader) < 5 || !Gifts.ContainsKey(trader))
            return;
        // Gift access follows the same character-based gate as this trader's shop.
        var definition = TraderRegistry.Traders.FirstOrDefault(x => x.Id == trader);
        if (definition == null || !ProgressionGate.CanAccessTrader(player, definition)) return;
        lock (Sync)
        {
            Load();
            string key = TraderReputation.RewardIdentity(player, trader) + GiftVersion;
            if (Claimed.Contains(key)) return;

            var bundle = Bundle(trader);
            var resolved = new List<(Gift gift, GameObject prefab, int stack)>();
            int slotsNeeded = 0;
            foreach (var gift in bundle)
            {
                var prefab = ObjectDB.instance?.GetItemPrefab(gift.Prefab);
                if (prefab == null) { Plugin.Log.LogWarning($"Trust gift item missing for {trader}: {gift.Prefab}"); continue; }
                int stack = Math.Max(1, prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_maxStackSize);
                slotsNeeded += (gift.Quantity + stack - 1) / stack;
                resolved.Add((gift, prefab, stack));
            }
            if (resolved.Count == 0) return;

            var inventory = player.GetInventory();
            if (inventory.GetEmptySlots() < slotsNeeded)
            {
                player.Message(MessageHud.MessageType.Center,
                    $"Trusted reward waiting: free {slotsNeeded} inventory slots, then reopen this trader's shop.");
                return;
            }

            var added = new List<(string name, int amount)>();
            bool failed = false;
            foreach (var (gift, prefab, stack) in resolved)
            {
                int remaining = gift.Quantity;
                string itemName = prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_name;
                while (remaining > 0)
                {
                    int take = Math.Min(remaining, stack);
                    if (!inventory.AddItem(prefab, take)) { failed = true; break; }
                    remaining -= take;
                    added.Add((itemName, take));
                }
                if (failed) break;
            }
            if (failed)
            {
                foreach (var (name, amount) in added) inventory.RemoveItem(name, amount);
                player.Message(MessageHud.MessageType.Center,
                    "Trusted reward could not fit in your inventory; make space and reopen the shop.");
                return;
            }

            try
            {
                Directory.CreateDirectory(Paths.ConfigPath);
                File.AppendAllText(FilePath, key + Environment.NewLine);
                Claimed.Add(key);
                string list = string.Join(", ", resolved.Select(r => $"{r.gift.Quantity} x {r.gift.Name}"));
                player.Message(MessageHud.MessageType.Center, $"Trusted reward: {list}.");
                Plugin.Log.LogInfo($"Trust gift granted by {trader}: {list}");
            }
            catch (Exception error)
            {
                Plugin.Log.LogError($"Could not record trust gift for {trader}: {error}");
                foreach (var (name, amount) in added) inventory.RemoveItem(name, amount);
                player.Message(MessageHud.MessageType.Center,
                    "Trusted reward could not be saved; reopen this trader's shop later.");
            }
        }
    }
}
