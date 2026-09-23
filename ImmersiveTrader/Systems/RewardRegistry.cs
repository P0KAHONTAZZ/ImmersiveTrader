using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class RewardRegistry
{
    public static readonly List<RewardDefinition> Rewards = new();

    public static void Initialize()
    {
        Rewards.Clear();

        Add("midka", "ancient_parcel", "Feathers", 20);
        Add("midka", "carved_idol", "Honey", 20);
        Add("midka", "sealed_mead_cask", "DeerMeat", 15);
        Add("midka", "merchants_gem", "LeatherScraps", 20);
        Add("midka", "runic_ledger", "FineWood", 20);

        Add("troldad", "ancient_parcel", "Feathers", 20);
        Add("troldad", "carved_idol", "DeerHide", 20);
        Add("troldad", "sealed_mead_cask", "Resin", 20);
        Add("troldad", "merchants_gem", "DeerMeat", 20);
        Add("troldad", "runic_ledger", "RoundLog", 20);

        Add("grimvald", "ancient_parcel", "FineWood", 20);
        Add("grimvald", "carved_idol", "Resin", 30);
        Add("grimvald", "sealed_mead_cask", "SurtlingCore", 10);
        Add("grimvald", "merchants_gem", "RoundLog", 20);
        Add("grimvald", "runic_ledger", "GreydwarfEye", 20);

        Add("rudy_warg", "ancient_parcel", "Feathers", 20);
        Add("rudy_warg", "carved_idol", "TrollHide", 15);
        Add("rudy_warg", "sealed_mead_cask", "DeerHide", 20);
        Add("rudy_warg", "merchants_gem", "Flint", 20);
        Add("rudy_warg", "runic_ledger", "DeerMeat", 15);

        Add("mokra_dzika", "ancient_parcel", "Guck", 10);
        Add("mokra_dzika", "carved_idol", "Bloodbag", 15);
        Add("mokra_dzika", "sealed_mead_cask", "TurnipSeeds", 10);
        Add("mokra_dzika", "merchants_gem", "ElderBark", 15);
        Add("mokra_dzika", "runic_ledger", "Ooze", 10);

        Add("encek", "ancient_parcel", "Entrails", 15);
        Add("encek", "carved_idol", "Chain", 10);
        Add("encek", "sealed_mead_cask", "Bloodbag", 15);
        Add("encek", "merchants_gem", "Thistle", 20);
        Add("encek", "runic_ledger", "SurtlingCore", 10);

        Add("hrothgar", "ancient_parcel", "Obsidian", 20);
        Add("hrothgar", "carved_idol", "WolfFang", 10);
        Add("hrothgar", "sealed_mead_cask", "WolfPelt", 10);
        Add("hrothgar", "merchants_gem", "FreezeGland", 10);
        Add("hrothgar", "runic_ledger", "Feathers", 20);

        Add("ylva_frost", "ancient_parcel", "OnionSeeds", 10);
        Add("ylva_frost", "carved_idol", "Crystal", 15);
        Add("ylva_frost", "sealed_mead_cask", "FreezeGland", 15);
        Add("ylva_frost", "merchants_gem", "Obsidian", 20);
        Add("ylva_frost", "runic_ledger", "WolfMeat", 15);

        Add("bjarki_goldtooth", "ancient_parcel", "Barley", 20);
        Add("bjarki_goldtooth", "carved_idol", "Flax", 20);
        Add("bjarki_goldtooth", "sealed_mead_cask", "LoxMeat", 15);
        Add("bjarki_goldtooth", "merchants_gem", "Tar", 20);
        Add("bjarki_goldtooth", "runic_ledger", "LoxPelt", 15);

        Add("ragnar_turnipson", "ancient_parcel", "Needle", 15);
        Add("ragnar_turnipson", "carved_idol", "Cloudberry", 20);
        Add("ragnar_turnipson", "sealed_mead_cask", "Barley", 20);
        Add("ragnar_turnipson", "merchants_gem", "Flax", 15);
        Add("ragnar_turnipson", "runic_ledger", "LoxMeat", 15);

        Add("cmok", "ancient_parcel", "Sap", 10);
        Add("cmok", "carved_idol", "RoyalJelly", 10);
        Add("cmok", "sealed_mead_cask", "MushroomJotunPuffs", 15);
        Add("cmok", "merchants_gem", "MushroomMagecap", 15);
        Add("cmok", "runic_ledger", "SoftTissue", 10);

        Add("grelka", "ancient_parcel", "Carapace", 15);
        Add("grelka", "carved_idol", "Mandible", 10);
        Add("grelka", "sealed_mead_cask", "BlackMarble", 20);
        Add("grelka", "merchants_gem", "YggdrasilWood", 15);
        Add("grelka", "runic_ledger", "RoyalJelly", 10);

        Add("spalony_zenek", "ancient_parcel", "Grausten", 20);
        Add("spalony_zenek", "carved_idol", "AskHide", 15);
        Add("spalony_zenek", "sealed_mead_cask", "Vineberry", 15);
        Add("spalony_zenek", "merchants_gem", "ProustitePowder", 10);
        Add("spalony_zenek", "runic_ledger", "Feathers", 20);

        Add("skjold_cinderborn", "ancient_parcel", "Grausten", 20);
        Add("skjold_cinderborn", "carved_idol", "AskHide", 15);
        Add("skjold_cinderborn", "sealed_mead_cask", "ProustitePowder", 10);
        Add("skjold_cinderborn", "merchants_gem", "Vineberry", 15);
        Add("skjold_cinderborn", "runic_ledger", "Ashwood", 15);
    }

    private static void Add(string traderId, string treasureId, string itemPrefab, int baseAmount)
        => Rewards.Add(new RewardDefinition(traderId, treasureId, itemPrefab, baseAmount));
}