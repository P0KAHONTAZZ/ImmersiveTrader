using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

/// <summary>One permanent stock unlock at each reputation level 2–5 for every trader.</summary>
public static class TraderShopRegistry
{
    public static readonly IReadOnlyList<TraderOfferDefinition> Offers = new List<TraderOfferDefinition>
    {
        // Midka's 70-coin potion is her existing level-one exception.
        new("midka", "MeadHealthMinor", 70, 1, 0, "Minor Healing Mead"),
        new("midka", "Honey", 90, 1, 0, "Honey", 2),
        new("midka", "RawMeat", 150, 1, 0, "Boar Meat", 3),
        new("midka", "FineWood", 300, 1, 0, "Fine Wood", 4),
        new("midka", "MeadStaminaMinor", 350, 1, 0, "Minor Stamina Mead", 5),

        new("troldad", "Raspberry", 75, 1, 0, "Raspberries", 2),
        new("troldad", "LeatherScraps", 75, 1, 0, "Leather Scraps", 3),
        new("troldad", "DeerHide", 140, 1, 0, "Deer Hide", 4),
        new("troldad", "DeerStew", 450, 1, 0, "Deer Stew", 5),

        new("grimvald", "GreydwarfEye", 110, 1, 1, "Greydwarf Eye", 2),
        new("grimvald", "Coal", 180, 1, 1, "Coal", 3),
        new("grimvald", "TinOre", 500, 1, 1, "Tin Ore", 4),
        new("grimvald", "BronzeNails", 700, 1, 1, "Bronze Nails", 5),

        new("rudy_warg", "TrollHide", 220, 1, 1, "Troll Hide", 2),
        new("rudy_warg", "SurtlingCore", 300, 1, 1, "Surtling Core", 3),
        new("rudy_warg", "CopperOre", 550, 1, 1, "Copper Ore", 4),
        new("rudy_warg", "FineWood", 700, 1, 1, "Fine Wood", 5),

        new("mokra_dzika", "TurnipSeeds", 300, 1, 2, "Turnip Seeds", 2),
        new("mokra_dzika", "Root", 380, 1, 2, "Root", 3),
        new("mokra_dzika", "Chain", 500, 1, 2, "Chain", 4),
        new("mokra_dzika", "Bloodbag", 720, 1, 2, "Bloodbag", 5),

        new("encek", "MeadPoisonResist", 300, 1, 2, "Poison Resistance Mead", 2),
        new("encek", "Chain", 380, 1, 2, "Chain", 3),
        new("encek", "IronScrap", 650, 1, 2, "Scrap Iron", 4),
        new("encek", "Root", 800, 1, 2, "Root", 5),

        new("hrothgar", "MeadFrostResist", 420, 1, 3, "Frost Resistance Mead", 2),
        new("hrothgar", "TrophyWolf", 500, 1, 3, "Wolf Trophy", 3),
        new("hrothgar", "SilverOre", 750, 1, 3, "Silver Ore", 4),
        new("hrothgar", "Obsidian", 850, 1, 3, "Obsidian", 5),

        new("ylva_frost", "OnionSeeds", 420, 1, 3, "Onion Seeds", 2),
        new("ylva_frost", "FreezeGland", 480, 1, 3, "Freeze Gland", 3),
        new("ylva_frost", "Crystal", 680, 1, 3, "Crystal", 4),
        new("ylva_frost", "MeadStaminaMedium", 900, 1, 3, "Medium Stamina Mead", 5),

        new("bjarki_goldtooth", "Cloudberry", 480, 1, 4, "Cloudberries", 2),
        new("bjarki_goldtooth", "Needle", 550, 1, 4, "Needle", 3),
        new("bjarki_goldtooth", "BlackMetalScrap", 850, 1, 4, "Black Metal Scrap", 4),
        new("bjarki_goldtooth", "LoxPelt", 1100, 1, 4, "Lox Pelt", 5),

        new("ragnar_turnipson", "Barley", 480, 1, 4, "Barley", 2),
        new("ragnar_turnipson", "Flax", 600, 1, 4, "Flax", 3),
        new("ragnar_turnipson", "LoxPelt", 800, 1, 4, "Lox Pelt", 4),
        new("ragnar_turnipson", "Wisp", 1300, 1, 4, "Wisp", 5),

        new("cmok", "BlackMarble", 800, 1, 5, "Black Marble", 2),
        new("cmok", "YggdrasilWood", 980, 1, 5, "Yggdrasil Wood", 3),
        new("cmok", "Sap", 1000, 1, 5, "Sap", 4),
        new("cmok", "Carapace", 1400, 1, 5, "Carapace", 5),

        new("grelka", "RoyalJelly", 700, 1, 5, "Royal Jelly", 2),
        new("grelka", "Bilebag", 850, 1, 5, "Bilebag", 3),
        new("grelka", "Softtissue", 1000, 1, 5, "Soft Tissue", 4),
        new("grelka", "Eitr", 1500, 1, 5, "Refined Eitr", 5),

        new("spalony_zenek", "CharredBone", 700, 1, 6, "Charred Bone", 2),
        new("spalony_zenek", "SulfurStone", 850, 1, 6, "Sulfur", 3),
        new("spalony_zenek", "FlametalOreNew", 1500, 1, 6, "Flametal Ore", 4),
        new("spalony_zenek", "MorgenSinew", 1800, 1, 6, "Morgen Sinew", 5),

        new("skjold_cinderborn", "Blackwood", 750, 1, 6, "Ashwood", 2),
        new("skjold_cinderborn", "CharredBone", 850, 1, 6, "Charred Bone", 3),
        new("skjold_cinderborn", "MoltenCore", 1000, 1, 6, "Molten Core", 4),
        new("skjold_cinderborn", "MorgenSinew", 1900, 1, 6, "Morgen Sinew", 5)
    };
}
