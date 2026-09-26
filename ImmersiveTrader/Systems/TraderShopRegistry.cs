using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

/// <summary>
/// Reputation stock per trader (1 piece per purchase except arrows, unlimited purchases except scrolls):
///  L1-L3: tedious-to-farm materials/arrows of the trader's biome (no duplicates between traders;
///         arrows are sold in bundles of 20),
///  L4:    the biome's key metal/alloy/resource at a deliberately high price (buy or go farm it),
///  L5:    one enhancement scroll, different for every trader, matched to the next biome's boss
///         (buyable once per 3 in-game days). Rested and Endurance are never sold.
/// </summary>
public static class TraderShopRegistry
{
    /// <summary>Scroll sold at reputation 5 by each trader (excluded from that trader's random trust gift).</summary>
    public static readonly IReadOnlyDictionary<string, string> ScrollOffer = new Dictionary<string, string>
    {
        ["midka"] = "vitality",
        ["troldad"] = "embers",
        ["grimvald"] = "spirit",
        ["rudy_warg"] = "embers",
        ["mokra_dzika"] = "hunter",
        ["encek"] = "pathfinder",
        ["hrothgar"] = "storm",
        ["ylva_frost"] = "wanderer",
        ["bjarki_goldtooth"] = "venom",
        ["ragnar_turnipson"] = "burden",
        ["cmok"] = "frost",
        ["grelka"] = "focus",
        ["spalony_zenek"] = "storm",
        ["skjold_cinderborn"] = "hunter",
    };

    public const double ScrollCooldownDays = 3d;

    public static readonly IReadOnlyList<TraderOfferDefinition> Offers = Build();

    /// <summary>Logs every offer or trust-gift prefab this game build does not know.</summary>
    public static void Validate()
    {
        var missing = new List<string>();
        foreach (var offer in Offers)
            if (ObjectDB.instance.GetItemPrefab(offer.ItemPrefab) == null) missing.Add($"offer {offer.TraderId}:{offer.ItemPrefab}");
        foreach (var pair in TraderTrustReward.Gifts)
            foreach (var gift in pair.Value)
                if (ObjectDB.instance.GetItemPrefab(gift.Prefab) == null) missing.Add($"gift {pair.Key}:{gift.Prefab}");
        if (missing.Count == 0) Plugin.Log.LogInfo($"Trader stock ready: {Offers.Count} offers, all prefabs found.");
        else Plugin.Log.LogWarning("Trader stock prefabs missing: " + string.Join(", ", missing));
    }

    private static List<TraderOfferDefinition> Build()
    {
        var list = new List<TraderOfferDefinition>
        {
            // Midka's field-medic potion stays her level-one exception.
            new("midka", "MeadHealthMinor", 70, 1, 0, "Minor Healing Mead", 1),

            new("midka", "Honey", 30, 1, 0, "Honey", 1),
            new("midka", "Dandelion", 20, 1, 0, "Dandelion", 2),
            new("midka", "Feathers", 40, 1, 0, "Feathers", 3),
            new("midka", "HardAntler", 500, 1, 0, "Hard Antler", 4),
            new("midka", EnhancementScrollItems.PrefabName("vitality"), 500, 1, 0, "Scroll of Vitality", 5),

            new("troldad", "LeatherScraps", 20, 1, 0, "Leather Scraps", 1),
            new("troldad", "DeerHide", 30, 1, 0, "Deer Hide", 2),
            new("troldad", "NeckTail", 24, 1, 0, "Neck Tail", 3),
            new("troldad", "FineWood", 100, 1, 0, "Fine Wood", 4),
            new("troldad", EnhancementScrollItems.PrefabName("embers"), 500, 1, 0, "Scroll of Embers", 5),

            new("grimvald", "GreydwarfEye", 24, 1, 1, "Greydwarf Eye", 1),
            new("grimvald", "Thistle", 40, 1, 1, "Thistle", 2),
            new("grimvald", "TrophyBjorn", 200, 1, 1, "Bear Trophy", 3),
            new("grimvald", "Bronze", 650, 1, 1, "Bronze", 4),
            new("grimvald", EnhancementScrollItems.PrefabName("spirit"), 580, 1, 1, "Scroll of Spirit", 5),

            new("rudy_warg", "TrollHide", 70, 1, 1, "Troll Hide", 1),
            new("rudy_warg", "Blueberries", 24, 1, 1, "Blueberries", 2),
            new("rudy_warg", "BjornHide", 120, 1, 1, "Bear Hide", 3),
            new("rudy_warg", "Copper", 550, 1, 1, "Copper", 4),
            new("rudy_warg", EnhancementScrollItems.PrefabName("embers"), 580, 1, 1, "Scroll of Embers", 5),

            new("mokra_dzika", "Entrails", 40, 1, 2, "Entrails", 1),
            new("mokra_dzika", "ArrowPoison", 140, 20, 2, "Poison Arrows", 2),
            new("mokra_dzika", "Root", 140, 1, 2, "Root", 3),
            new("mokra_dzika", "IronScrap", 650, 1, 2, "Scrap Iron", 4),
            new("mokra_dzika", EnhancementScrollItems.PrefabName("hunter"), 660, 1, 2, "Scroll of Hunter", 5),

            new("encek", "Guck", 100, 1, 2, "Guck", 1),
            new("encek", "Bloodbag", 60, 1, 2, "Bloodbag", 2),
            new("encek", "Chain", 180, 1, 2, "Chain", 3),
            new("encek", "Iron", 750, 1, 2, "Iron", 4),
            new("encek", EnhancementScrollItems.PrefabName("pathfinder"), 660, 1, 2, "Scroll of Pathfinder", 5),

            new("hrothgar", "WolfPelt", 70, 1, 3, "Wolf Pelt", 1),
            new("hrothgar", "WolfFang", 60, 1, 3, "Wolf Fang", 2),
            new("hrothgar", "TrophyWolf", 400, 1, 3, "Wolf Trophy", 3),
            new("hrothgar", "Silver", 850, 1, 3, "Silver", 4),
            new("hrothgar", EnhancementScrollItems.PrefabName("storm"), 750, 1, 3, "Scroll of Storm", 5),

            new("ylva_frost", "FreezeGland", 70, 1, 3, "Freeze Gland", 1),
            new("ylva_frost", "WolfHairBundle", 60, 1, 3, "Wolf Hair Bundle", 2),
            new("ylva_frost", "Obsidian", 60, 1, 3, "Obsidian", 3),
            new("ylva_frost", "SilverOre", 750, 1, 3, "Silver Ore", 4),
            new("ylva_frost", EnhancementScrollItems.PrefabName("wanderer"), 750, 1, 3, "Scroll of Wanderer", 5),

            new("bjarki_goldtooth", "Needle", 80, 1, 4, "Needle", 1),
            new("bjarki_goldtooth", "LoxPelt", 110, 1, 4, "Lox Pelt", 2),
            new("bjarki_goldtooth", "Tar", 80, 1, 4, "Tar", 3),
            new("bjarki_goldtooth", "BlackMetal", 950, 1, 4, "Black Metal", 4),
            new("bjarki_goldtooth", EnhancementScrollItems.PrefabName("venom"), 830, 1, 4, "Scroll of Venom", 5),

            new("ragnar_turnipson", "Cloudberry", 60, 1, 4, "Cloudberries", 1),
            new("ragnar_turnipson", "ArrowNeedle", 200, 20, 4, "Needle Arrows", 2),
            new("ragnar_turnipson", "LoxMeat", 80, 1, 4, "Lox Meat", 3),
            new("ragnar_turnipson", "BlackMetalScrap", 850, 1, 4, "Black Metal Scrap", 4),
            new("ragnar_turnipson", EnhancementScrollItems.PrefabName("burden"), 830, 1, 4, "Scroll of Burden", 5),

            new("cmok", "Bilebag", 100, 1, 5, "Bilebag", 1),
            new("cmok", "Carapace", 140, 1, 5, "Carapace", 2),
            new("cmok", "Mandible", 160, 1, 5, "Mandible", 3),
            new("cmok", "Softtissue", 950, 1, 5, "Soft Tissue", 4),
            new("cmok", EnhancementScrollItems.PrefabName("frost"), 920, 1, 5, "Scroll of Frost", 5),

            new("grelka", "MushroomJotunPuffs", 90, 1, 5, "Jotun Puffs", 1),
            new("grelka", "MushroomMagecap", 100, 1, 5, "Magecap", 2),
            new("grelka", "RoyalJelly", 110, 1, 5, "Royal Jelly", 3),
            new("grelka", "Sap", 900, 1, 5, "Sap", 4),
            new("grelka", EnhancementScrollItems.PrefabName("focus"), 920, 1, 5, "Scroll of Focus", 5),

            new("spalony_zenek", "CharredBone", 110, 1, 6, "Charred Bone", 1),
            new("spalony_zenek", "SulfurStone", 140, 1, 6, "Sulfur", 2),
            new("spalony_zenek", "MorgenSinew", 220, 1, 6, "Morgen Sinew", 3),
            new("spalony_zenek", "FlametalNew", 1000, 1, 6, "Flametal", 4),
            new("spalony_zenek", EnhancementScrollItems.PrefabName("storm"), 1000, 1, 6, "Scroll of Storm", 5),

            new("skjold_cinderborn", "VoltureEgg", 100, 1, 6, "Volture Egg", 1),
            new("skjold_cinderborn", "AsksvinHide", 150, 1, 6, "Asksvin Hide", 2),
            new("skjold_cinderborn", "CeramicPlate", 180, 1, 6, "Ceramic Plate", 3),
            new("skjold_cinderborn", "MoltenCore", 3000, 1, 6, "Molten Core", 4),
            new("skjold_cinderborn", EnhancementScrollItems.PrefabName("hunter"), 1000, 1, 6, "Scroll of Hunter", 5),

            // Thematic profession scrolls are secondary level-5 offers. Nine traders therefore have two L5 items.
            new("troldad", EnhancementScrollItems.PrefabName("lumberjack"), 500, 1, 0, "Scroll of Lumberjack", 5),
            new("grimvald", EnhancementScrollItems.PrefabName("miner"), 580, 1, 1, "Scroll of Miner", 5),
            new("encek", EnhancementScrollItems.PrefabName("craftsman"), 660, 1, 2, "Scroll of Craftsman", 5),
            new("hrothgar", EnhancementScrollItems.PrefabName("miner"), 750, 1, 3, "Scroll of Miner", 5),
            new("ragnar_turnipson", EnhancementScrollItems.PrefabName("lumberjack"), 830, 1, 4, "Scroll of Lumberjack", 5),
            new("cmok", EnhancementScrollItems.PrefabName("craftsman"), 920, 1, 5, "Scroll of Craftsman", 5),
            new("grelka", EnhancementScrollItems.PrefabName("craftsman"), 920, 1, 5, "Scroll of Craftsman", 5),
            new("spalony_zenek", EnhancementScrollItems.PrefabName("lumberjack"), 1000, 1, 6, "Scroll of Lumberjack", 5),
            new("skjold_cinderborn", EnhancementScrollItems.PrefabName("miner"), 1000, 1, 6, "Scroll of Miner", 5),
        };
        return list;
    }
}
