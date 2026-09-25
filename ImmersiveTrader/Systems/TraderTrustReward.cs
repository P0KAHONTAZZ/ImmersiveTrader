using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;

namespace ImmersiveTrader;

/// <summary>One trust gift per character, world, and trader; retried when inventory is full.</summary>
public static class TraderTrustReward
{
    public sealed record Gift(string Prefab, string Name, int Quantity = 1);
    public static readonly IReadOnlyDictionary<string, Gift> Gifts = new Dictionary<string, Gift>
    {
        ["midka"] = new("HelmetDverger", "Dverger Circlet"),
        ["troldad"] = new("MeadTamer", "Brew of Animal Whispers"),
        ["grimvald"] = new("IronScrap", "Scrap Iron", 30),
        ["rudy_warg"] = new("HelmetRoot", "Root Mask"),
        ["mokra_dzika"] = new("SilverOre", "Silver Ore", 30),
        ["encek"] = new("SilverOre", "Silver Ore", 30),
        ["hrothgar"] = new("SaddleLox", "Lox Saddle"),
        ["ylva_frost"] = new("BlackMetalScrap", "Black Metal Scrap", 30),
        ["bjarki_goldtooth"] = new("GrapplingHook", "Grappling Hook"),
        ["ragnar_turnipson"] = new("SaddleLox", "Lox Saddle"),
        ["cmok"] = new("MoltenCore", "Molten Core", 5),
        ["grelka"] = new("FlametalOreNew", "Flametal Ore", 10),
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
        // The gift is earned from this trader's reputation, including cargo delivered
        // before the next boss falls. The boss gate still applies to store stock.
        lock (Sync)
        {
            Load();
            string key = TraderReputation.RewardIdentity(player, trader);
            if (trader == "cmok" || trader == "grelka")
                key += "|trust-gift-v3";
            else if (gift.Quantity == 30)
                key += "|metal-gift-v2";
            if (Claimed.Contains(key)) return;
            var prefab = ObjectDB.instance?.GetItemPrefab(gift.Prefab);
            if (prefab == null)
            {
                Plugin.Log.LogWarning($"Trust gift item missing for {trader}: {gift.Prefab}");
                return;
            }
            // Prior versions of these two Mistlands gifts stay claimed; grant the
            // updated reward exactly once even to players who claimed an older one.
            var inventory = player.GetInventory();
            if (!inventory.CanAddItem(prefab, gift.Quantity))
            {
                player.Message(MessageHud.MessageType.Center,
                    "Trusted reward waiting: make room for the full gift (30 ore needs a free inventory slot). Reopen this trader's shop.");
                return;
            }

            int remaining = gift.Quantity;
            int granted = 0;
            int stackSize = Math.Max(1, prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_maxStackSize);
            while (remaining > 0)
            {
                int take = Math.Min(remaining, stackSize);
                if (!inventory.AddItem(prefab, take)) break;
                remaining -= take;
                granted += take;
            }
            var itemName = prefab.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_name;
            if (remaining > 0)
            {
                if (granted > 0 && !string.IsNullOrEmpty(itemName))
                    inventory.RemoveItem(itemName, granted);
                player.Message(MessageHud.MessageType.Center,
                    "Trusted reward could not fit in your inventory; make space and reopen the shop.");
                return;
            }

            try
            {
                Directory.CreateDirectory(Paths.ConfigPath);
                File.AppendAllText(FilePath, key + Environment.NewLine);
                Claimed.Add(key);
                player.Message(MessageHud.MessageType.Center,
                    $"Trusted reward: {gift.Quantity} x {gift.Name} added to your inventory.");
            }
            catch (Exception error)
            {
                Plugin.Log.LogError($"Could not record trust gift for {trader}: {error}");
                if (!string.IsNullOrEmpty(itemName)) inventory.RemoveItem(itemName, granted);
                player.Message(MessageHud.MessageType.Center,
                    "Trusted reward could not be saved; reopen this trader's shop later.");
            }
        }
    }
}
