using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

/// <summary>
/// Curated trader stock. Offers stay narrow and thematic so shops reduce grind
/// without replacing biome exploration or core crafting progression.
/// </summary>
public static class TraderShopRegistry
{
    public static readonly IReadOnlyList<TraderOfferDefinition> Offers = new List<TraderOfferDefinition>
    {
        new("midka", "MeadHealthMinor", 50, 1, 0, "Minor Healing Mead"),
        new("troldad", "ArrowFlint", 18, 20, 0, "Flint Arrows"),
        new("grimvald", "Resin", 12, 10, 1, "Resin"),
        new("rudy_warg", "ArrowFire", 25, 20, 1, "Fire Arrows"),
        new("mokra_dzika", "Thistle", 18, 5, 2, "Thistle"),
        new("encek", "Entrails", 25, 5, 2, "Entrails"),
        new("hrothgar", "Obsidian", 35, 5, 3, "Obsidian"),
        new("ylva_frost", "OnionSeeds", 45, 3, 3, "Onion Seeds"),
        new("bjarki_goldtooth", "Cloudberry", 25, 5, 4, "Cloudberries"),
        new("ragnar_turnipson", "Barley", 40, 5, 4, "Barley"),
        new("cmok", "MushroomJotunPuffs", 45, 5, 5, "Jotun Puffs"),
        new("grelka", "MushroomMagecap", 45, 5, 5, "Magecap")
    };
}
