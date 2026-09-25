using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class TraderRegistry
{
    public static readonly IReadOnlyList<TraderDefinition> Traders = new List<TraderDefinition>
    {
        new("midka", "Midka", "Meadows", 0, "War healer & field medic"),
        new("troldad", "Troldad", "Meadows", 0, "He came back to life after stroke like this", LiesAboutRewards: true),

        new("grimvald", "Grimvald", "Black Forest", 1, "Forest quartermaster"),
        new("rudy_warg", "Rudy Warg", "Black Forest", 1, "Tracker of the old forest"),

        new("mokra_dzika", "Mokra Dzika", "Swamp", 2, "Swamp herbalist"),
        new("encek", "Encek", "Swamp", 2, "Pro Roguelike player"),

        new("hrothgar", "Hrothgar", "Mountains", 3, "Wolf hunter of the high peaks"),
        new("ylva_frost", "Ylva Frost", "Mountains", 3, "Keeper of frost and mountain herbs"),

        new("bjarki_goldtooth", "Bjarki Goldtooth", "Plains", 4, "Gold-toothed caravan master"),
        new("ragnar_turnipson", "Ragnar Turnipson", "Plains", 4, "Plains cook and stubborn farmer"),

        new("cmok", "Tyrron", "Mistlands", 5, "Dungeon Hunter"),
        new("grelka", "Grelka", "Mistlands", 5, "Mistlands mushroom forager"),

        new("spalony_zenek", "Spalony Zenek", "Ashlands", 6, "Ashlands scavenger and firebrand"),
        new("skjold_cinderborn", "Skjold Cinderborn", "Ashlands", 6, "Shieldsmith of the burning coast"),

        new("legendary_mieteg", "Legendary Mieteg", "Special", 99, "Legendary cache and jackpot", IsLegendary: true)
    };

    public static void Initialize()
    {
    }
}