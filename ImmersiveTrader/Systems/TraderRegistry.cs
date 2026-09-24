using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class TraderRegistry
{
    public static readonly IReadOnlyList<TraderDefinition> Traders = new List<TraderDefinition>
    {
        new("midka", "Midka", "Meadows", 0, "Experienced war healer and field medic"),
        new("troldad", "Troldad", "Meadows", 0, "Lumberjack, hunter and alcoholic", LiesAboutRewards: true),

        new("grimvald", "Grimvald", "Black Forest", 1, "Wood, resin and utility"),
        new("rudy_warg", "Rudy Warg", "Black Forest", 1, "Hunting and archery"),

        new("mokra_dzika", "Mokra Dzika", "Swamp", 2, "Swamp reagents and farming"),
        new("encek", "Encek", "Swamp", 2, "Blood, entrails and utility"),

        new("hrothgar", "Hrothgar", "Mountains", 3, "Wolves, obsidian and archery"),
        new("ylva_frost", "Ylva Frost", "Mountains", 3, "Onions, crystals and frost"),

        new("bjarki_goldtooth", "Bjarki Goldtooth", "Plains", 4, "Barley, flax, lox and tar"),
        new("ragnar_turnipson", "Ragnar Turnipson", "Plains", 4, "Food, needles and farming"),

        new("cmok", "Ćmok", "Mistlands", 5, "Sap, royal jelly and magical supplies"),
        new("grelka", "Grelka", "Mistlands", 5, "Mushrooms, carapace and black marble"),

        new("spalony_zenek", "Spalony Zenek", "Ashlands", 6, "Ashlands materials and ammunition"),
        new("skjold_cinderborn", "Skjold Cinderborn", "Ashlands", 6, "Ashlands building and crafting"),

        new("legendary_mieteg", "Legendary Mieteg", "Special", 99, "Legendary cache and jackpot", IsLegendary: true)
    };

    public static void Initialize()
    {
        RewardRegistry.Initialize();
        RewardRegistry.Validate();
    }
}