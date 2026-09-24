using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

/// <summary>
/// Phase 1 shop stock: five ordinary, known Valheim items per trader.
/// Cargo and contracts are deliberately NOT part of StoreGui yet; first we verify
/// that expanding native stock does not affect the stable trader lifecycle.
/// </summary>
public static class TraderShopRegistry
{
    public static readonly IReadOnlyList<TraderOfferDefinition> Offers = new List<TraderOfferDefinition>
    {
        new("midka","MeadHealthMinor",50,1,0,"Minor Healing Mead"),
        new("midka","Raspberry",12,5,0,"Raspberries"),
        new("midka","Blueberries",16,5,0,"Blueberries"),
        new("midka","Honey",20,5,0,"Honey"),
        new("midka","Mushroom",12,5,0,"Mushrooms"),

        new("troldad","ArrowFlint",18,20,0,"Flint Arrows"),
        new("troldad","Wood",12,20,0,"Wood"),
        new("troldad","LeatherScraps",18,10,0,"Leather Scraps"),
        new("troldad","DeerHide",25,5,0,"Deer Hide"),
        new("troldad","CookedDeerMeat",20,5,0,"Cooked Deer Meat"),

        new("grimvald","Resin",12,10,1,"Resin"),
        new("grimvald","Copper",35,5,1,"Copper"),
        new("grimvald","Tin",30,5,1,"Tin"),
        new("grimvald","CoreWood",25,10,1,"Core Wood"),
        new("grimvald","SurtlingCore",45,2,1,"Surtling Cores"),

        new("rudy_warg","ArrowFire",25,20,1,"Fire Arrows"),
        new("rudy_warg","DeerHide",25,5,1,"Deer Hide"),
        new("rudy_warg","LeatherScraps",18,10,1,"Leather Scraps"),
        new("rudy_warg","Feathers",20,10,1,"Feathers"),
        new("rudy_warg","CoreWood",25,10,1,"Core Wood"),

        new("mokra_dzika","Thistle",18,5,2,"Thistle"),
        new("mokra_dzika","Bloodbag",30,5,2,"Bloodbags"),
        new("mokra_dzika","Root",35,5,2,"Roots"),
        new("mokra_dzika","Guck",35,5,2,"Guck"),
        new("mokra_dzika","TurnipSeeds",45,3,2,"Turnip Seeds"),

        new("encek","Entrails",25,5,2,"Entrails"),
        new("encek","Iron",55,5,2,"Iron"),
        new("encek","ElderBark",25,10,2,"Ancient Bark"),
        new("encek","Chain",60,2,2,"Chains"),
        new("encek","Guck",35,5,2,"Guck"),

        new("hrothgar","Obsidian",35,5,3,"Obsidian"),
        new("hrothgar","Silver",70,5,3,"Silver"),
        new("hrothgar","WolfPelt",35,5,3,"Wolf Pelts"),
        new("hrothgar","Crystal",50,5,3,"Crystal"),
        new("hrothgar","FreezeGland",45,5,3,"Freeze Glands"),

        new("ylva_frost","OnionSeeds",45,3,3,"Onion Seeds"),
        new("ylva_frost","Onion",30,5,3,"Onions"),
        new("ylva_frost","WolfPelt",35,5,3,"Wolf Pelts"),
        new("ylva_frost","Crystal",50,5,3,"Crystal"),
        new("ylva_frost","FreezeGland",45,5,3,"Freeze Glands"),

        new("bjarki_goldtooth","Cloudberry",25,5,4,"Cloudberries"),
        new("bjarki_goldtooth","Barley",40,5,4,"Barley"),
        new("bjarki_goldtooth","Flax",40,5,4,"Flax"),
        new("bjarki_goldtooth","BlackMetalScrap",65,5,4,"Black Metal Scrap"),
        new("bjarki_goldtooth","LoxPelt",45,5,4,"Lox Pelts"),

        new("ragnar_turnipson","Barley",40,5,4,"Barley"),
        new("ragnar_turnipson","Flax",40,5,4,"Flax"),
        new("ragnar_turnipson","Cloudberry",25,5,4,"Cloudberries"),
        new("ragnar_turnipson","LoxMeat",45,5,4,"Lox Meat"),
        new("ragnar_turnipson","BlackMetalScrap",65,5,4,"Black Metal Scrap"),

        new("cmok","MushroomJotunPuffs",45,5,5,"Jotun Puffs"),
        new("cmok","YggdrasilWood",45,10,5,"Yggdrasil Wood"),
        new("cmok","BlackMarble",55,10,5,"Black Marble"),
        new("cmok","Sap",55,5,5,"Sap"),
        new("cmok","Softtissue",65,5,5,"Soft Tissue"),

        new("grelka","MushroomMagecap",45,5,5,"Magecap"),
        new("grelka","RoyalJelly",55,5,5,"Royal Jelly"),
        new("grelka","Sap",55,5,5,"Sap"),
        new("grelka","BlackMarble",55,10,5,"Black Marble"),
        new("grelka","YggdrasilWood",45,10,5,"Yggdrasil Wood"),

        new("spalony_zenek","Grausten",55,10,6,"Grausten"),
        new("spalony_zenek","Blackwood",55,10,6,"Blackwood"),
        new("spalony_zenek","FlametalOre",90,3,6,"Flametal Ore"),
        new("spalony_zenek","ProustitePowder",65,3,6,"Proustite Powder"),
        new("spalony_zenek","CelestialFeather",70,3,6,"Celestial Feathers"),

        new("skjold_cinderborn","Blackwood",55,10,6,"Blackwood"),
        new("skjold_cinderborn","Grausten",55,10,6,"Grausten"),
        new("skjold_cinderborn","FlametalOre",90,3,6,"Flametal Ore"),
        new("skjold_cinderborn","CelestialFeather",70,3,6,"Celestial Feathers"),
        new("skjold_cinderborn","ProustitePowder",65,3,6,"Proustite Powder")
    };
}
