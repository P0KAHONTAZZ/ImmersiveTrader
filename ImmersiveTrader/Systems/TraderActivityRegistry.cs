using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

/// <summary>
/// Data-first side-task catalogue. Runtime progress tracking will be attached to
/// server-authoritative kill/pickup events after the trade UI is in place.
/// </summary>
public static class TraderActivityRegistry
{
    public static readonly IReadOnlyList<TraderActivityDefinition> Activities = new List<TraderActivityDefinition>
    {
        new("troldad", "troldad_greydwarfs", "Clear the treeline", TraderActivityType.Hunt,
            "Greydwarf", 8, "RoundLog", 20, "Kill Greydwarfs that keep wandering into Troldad's hunting grounds."),
        new("midka", "midka_honey", "Field medicine", TraderActivityType.Gather,
            "Honey", 10, "MeadHealthMinor", 2, "Bring ingredients Midka can turn into field supplies."),
        new("mokra_dzika", "dzika_leeches", "Thin the leeches", TraderActivityType.Hunt,
            "Leech", 6, "Bloodbag", 10, "Reduce the leech population around the swamp camp."),
        new("encek", "encek_thistle", "Swamp provisions", TraderActivityType.Gather,
            "Thistle", 12, "Entrails", 10, "Bring useful provisions from the swamp and nearby forest.")
    };
}
