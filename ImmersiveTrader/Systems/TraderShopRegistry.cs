using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

/// <summary>Ordinary stock: only Midka sells Minor Healing Mead for 70 coins.
/// Cargo and hunting contracts are added to the native shop separately.</summary>
public static class TraderShopRegistry
{
    public static readonly IReadOnlyList<TraderOfferDefinition> Offers = new List<TraderOfferDefinition>
    {
        new("midka", "MeadHealthMinor", 70, 1, 0, "Minor Healing Mead")
    };
}
