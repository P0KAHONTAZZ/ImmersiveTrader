using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

/// <summary>
/// Reputation stock per trader (all sold 1 piece per purchase, unlimited purchases):
///  L1-L3: tedious-to-farm materials of the trader's biome (no duplicates between traders),
///  L4:    the biome's key metal/alloy/resource at a deliberately high price (buy or go farm it),
///  L5:    one enhancement scroll, different for every trader. Rested and Endurance are never sold.
/// </summary>
public static class TraderShopRegistry
{
    /// <summary>Scroll sold at reputation 5 by each trader (excluded from that trader's random trust gift).</summary>
    public static readonly IReadOnlyDictionary<string, string> ScrollOffer = new Dictionary<string, string>
    {
        ["midka"] = "vitality",
        ["troldad"] = "burden",
        ["grimvald"] = "lumberjack",
        ["rudy_warg"] = "hunter",
        ["mokra_dzika"] = "venom",
        ["encek"] = "spirit",
        ["hrothgar"] = "pathfinder",
        ["ylva_frost"] = "frost",
        ["bjarki_goldtooth"] = "wanderer",
        ["ragnar_turnipson"] = "craftsman",
        ["cmok"] = "storm",
        ["grelka"] = "focus",
        ["spalony_zenek"] = "embers",
        ["skjold_cinderborn"] = "miner",
    };

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

            new("midka", "Honey", 15, 1, 0, "Honey", 1),
            new("midka", "Resin", 8, 1, 0, "Resin", 2),
            new("midka", "Feathers", 20, 1, 0, "Feathers", 3),
            new("midka", "HardAntler", 300, 1, 0, "Hard Antler", 4),
            new("midka", EnhancementScrollItems.PrefabName("vitality"), 200, 1, 0, "Scroll of Vitality", 5),

            new("troldad", "LeatherScraps", 10, 1, 0, "Leather Scraps", 1),
            new("troldad", "DeerHide", 15, 1, 0, "Deer Hide", 2),
            new("troldad", "NeckTail", 12, 1, 0, "Neck Tail", 3),
            new("troldad", "FineWood", 50, 1, 0, "Fine Wood", 4),
            new("troldad", EnhancementScrollItems.PrefabName("burden"), 200, 1, 0, "Scroll of Burden", 5),

            new("grimvald", "GreydwarfEye", 12, 1, 1, "Greydwarf Eye", 1),
            new("grimvald", "BoneFragments", 15, 1, 1, "Bone Fragments", 2),
            new("grimvald", "Thistle", 20, 1, 1, "Thistle", 3),
            new("grimvald", "Bronze", 250, 1, 1, "Bronze", 4),
            new("grimvald", EnhancementScrollItems.PrefabName("lumberjack"), 300, 1, 1, "Scroll of Lumberjack", 5),

            new("rudy_warg", "TrollHide", 35, 1, 1, "Troll Hide", 1),
            new("rudy_warg", "MushroomYellow", 20, 1, 1, "Yellow Mushroom", 2),
            new("rudy_warg", "Blueberries", 12, 1, 1, "Blueberries", 3),
            new("rudy_warg", "Copper", 150, 1, 1, "Copper", 4),
            new("rudy_warg", EnhancementScrollItems.PrefabName("hunter"), 300, 1, 1, "Scroll of Hunter", 5),

            new("mokra_dzika", "Entrails", 20, 1, 2, "Entrails", 1),
            new("mokra_dzika", "TurnipSeeds", 25, 1, 2, "Turnip Seeds", 2),
            new("mokra_dzika", "Root", 35, 1, 2, "Root", 3),
            new("mokra_dzika", "IronScrap", 200, 1, 2, "Scrap Iron", 4),
            new("mokra_dzika", EnhancementScrollItems.PrefabName("venom"), 400, 1, 2, "Scroll of Venom", 5),

            new("encek", "Guck", 25, 1, 2, "Guck", 1),
            new("encek", "Bloodbag", 30, 1, 2, "Bloodbag", 2),
            new("encek", "Chain", 45, 1, 2, "Chain", 3),
            new("encek", "Iron", 300, 1, 2, "Iron", 4),
            new("encek", EnhancementScrollItems.PrefabName("spirit"), 400, 1, 2, "Scroll of Spirit", 5),

            new("hrothgar", "WolfPelt", 35, 1, 3, "Wolf Pelt", 1),
            new("hrothgar", "WolfFang", 30, 1, 3, "Wolf Fang", 2),
            new("hrothgar", "WolfMeat", 30, 1, 3, "Wolf Meat", 3),
            new("hrothgar", "Silver", 400, 1, 3, "Silver", 4),
            new("hrothgar", EnhancementScrollItems.PrefabName("pathfinder"), 500, 1, 3, "Scroll of Pathfinder", 5),

            new("ylva_frost", "FreezeGland", 35, 1, 3, "Freeze Gland", 1),
            new("ylva_frost", "Onion", 25, 1, 3, "Onion", 2),
            new("ylva_frost", "OnionSeeds", 40, 1, 3, "Onion Seeds", 3),
            new("ylva_frost", "SilverOre", 300, 1, 3, "Silver Ore", 4),
            new("ylva_frost", EnhancementScrollItems.PrefabName("frost"), 500, 1, 3, "Scroll of Frost", 5),

            new("bjarki_goldtooth", "Needle", 40, 1, 4, "Needle", 1),
            new("bjarki_goldtooth", "LoxPelt", 55, 1, 4, "Lox Pelt", 2),
            new("bjarki_goldtooth", "Tar", 40, 1, 4, "Tar", 3),
            new("bjarki_goldtooth", "BlackMetal", 500, 1, 4, "Black Metal", 4),
            new("bjarki_goldtooth", EnhancementScrollItems.PrefabName("wanderer"), 600, 1, 4, "Scroll of Wanderer", 5),

            new("ragnar_turnipson", "Cloudberry", 30, 1, 4, "Cloudberries", 1),
            new("ragnar_turnipson", "BarleyFlour", 40, 1, 4, "Barley Flour", 2),
            new("ragnar_turnipson", "Flax", 45, 1, 4, "Flax", 3),
            new("ragnar_turnipson", "BlackMetalScrap", 350, 1, 4, "Black Metal Scrap", 4),
            new("ragnar_turnipson", EnhancementScrollItems.PrefabName("craftsman"), 600, 1, 4, "Scroll of Craftsman", 5),

            new("cmok", "Bilebag", 50, 1, 5, "Bilebag", 1),
            new("cmok", "Carapace", 70, 1, 5, "Carapace", 2),
            new("cmok", "Mandible", 80, 1, 5, "Mandible", 3),
            new("cmok", "Softtissue", 400, 1, 5, "Soft Tissue", 4),
            new("cmok", EnhancementScrollItems.PrefabName("storm"), 750, 1, 5, "Scroll of Storm", 5),

            new("grelka", "MushroomJotunPuffs", 45, 1, 5, "Jotun Puffs", 1),
            new("grelka", "MushroomMagecap", 50, 1, 5, "Magecap", 2),
            new("grelka", "RoyalJelly", 55, 1, 5, "Royal Jelly", 3),
            new("grelka", "Sap", 400, 1, 5, "Sap", 4),
            new("grelka", EnhancementScrollItems.PrefabName("focus"), 750, 1, 5, "Scroll of Focus", 5),

            new("spalony_zenek", "CharredBone", 55, 1, 6, "Charred Bone", 1),
            new("spalony_zenek", "SulfurStone", 70, 1, 6, "Sulfur", 2),
            new("spalony_zenek", "MorgenSinew", 110, 1, 6, "Morgen Sinew", 3),
            new("spalony_zenek", "FlametalNew", 700, 1, 6, "Flametal", 4),
            new("spalony_zenek", EnhancementScrollItems.PrefabName("embers"), 900, 1, 6, "Scroll of Embers", 5),

            new("skjold_cinderborn", "Blackwood", 50, 1, 6, "Ashwood", 1),
            new("skjold_cinderborn", "AsksvinHide", 75, 1, 6, "Asksvin Hide", 2),
            new("skjold_cinderborn", "CeramicPlate", 90, 1, 6, "Ceramic Plate", 3),
            new("skjold_cinderborn", "MoltenCore", 3000, 1, 6, "Molten Core", 4),
            new("skjold_cinderborn", EnhancementScrollItems.PrefabName("miner"), 900, 1, 6, "Scroll of Miner", 5),
        };
        return list;
    }
}
