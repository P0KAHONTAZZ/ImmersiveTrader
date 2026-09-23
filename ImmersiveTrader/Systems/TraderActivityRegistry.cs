using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class TraderActivityRegistry
{
    public static readonly IReadOnlyList<TraderActivityDefinition> Activities = new List<TraderActivityDefinition>
    {
        Hunt("troldad", "troldad_greydwarfs", "Clear the treeline", "Greydwarf", 8, "RoundLog", 20, "Clear Greydwarfs from Troldad's hunting grounds."),
        Gather("midka", "midka_honey", "Field medicine", "Honey", 10, "MeadHealthMinor", 2, "Bring ingredients for Midka's field supplies."),
        Gather("grimvald", "grimvald_resin", "Keep the lamps burning", "Resin", 15, "FineWood", 10, "Bring resin for the Black Forest camp."),
        Hunt("rudy_warg", "rudy_greydwarf", "Hunting competition", "Greydwarf", 10, "ArrowFire", 20, "Thin the creatures around Rudy's hunting routes."),
        Hunt("mokra_dzika", "dzika_leeches", "Thin the leeches", "Leech", 6, "Bloodbag", 10, "Reduce the leech population around the swamp camp."),
        Gather("encek", "encek_thistle", "Swamp provisions", "Thistle", 12, "Entrails", 10, "Bring useful provisions from the swamp and nearby forest."),
        Hunt("hrothgar", "hrothgar_drakes", "Clear the peaks", "Hatchling", 5, "Obsidian", 15, "Drive drakes away from the mountain route."),
        Gather("ylva_frost", "ylva_obsidian", "Black glass", "Obsidian", 12, "FreezeGland", 6, "Bring obsidian from the high slopes."),
        Hunt("bjarki_goldtooth", "bjarki_fulings", "Plains patrol", "Goblin", 8, "Barley", 15, "Cut down Fulings threatening the trade road."),
        Gather("ragnar_turnipson", "ragnar_cloudberries", "Sweet provisions", "Cloudberry", 15, "Flax", 10, "Bring food for Ragnar's camp."),
        Hunt("cmok", "cmok_seekers", "Quiet the mist", "Seeker", 6, "RoyalJelly", 8, "Cull Seekers around the route through the mist."),
        Gather("grelka", "grelka_puffs", "Mushroom run", "MushroomJotunPuffs", 12, "YggdrasilWood", 10, "Bring fresh Jotun Puffs."),
        Hunt("spalony_zenek", "zenek_twitchers", "Ash patrol", "Charred_Twitcher", 8, "Grausten", 15, "Clear Charred from the approaches to the camp."),
        Gather("skjold_cinderborn", "skjold_grausten", "Reinforce the camp", "Grausten", 15, "Ashwood", 10, "Bring stone for the camp fortifications.")
    };

    private static TraderActivityDefinition Hunt(string trader, string id, string title, string target, int count, string reward, int rewardCount, string text) =>
        new(trader, id, title, TraderActivityType.Hunt, target, count, reward, rewardCount, text);

    private static TraderActivityDefinition Gather(string trader, string id, string title, string target, int count, string reward, int rewardCount, string text) =>
        new(trader, id, title, TraderActivityType.Gather, target, count, reward, rewardCount, text);
}
