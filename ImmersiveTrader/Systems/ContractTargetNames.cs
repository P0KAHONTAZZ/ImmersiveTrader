using System.Collections.Generic;

namespace ImmersiveTrader;

/// <summary>Player-facing names for contract targets; prefab ids remain internal.</summary>
public static class ContractTargetNames
{
    private static readonly IReadOnlyDictionary<string, string> Names = new Dictionary<string, string>
    {
        ["Greydwarf_Shaman"] = "Greydwarf Shaman",
        ["Greydwarf_Elite"] = "Greydwarf Brute",
        ["Troll"] = "Troll",
        ["Wraith"] = "Wraith",
        ["BlobElite"] = "Blob Elite",
        ["Abomination"] = "Abomination",
        ["Fenring"] = "Fenring",
        ["Hatchling"] = "Drake",
        ["StoneGolem"] = "Stone Golem",
        ["Deathsquito"] = "Deathsquito",
        ["GoblinBrute"] = "Fuling Berserker",
        ["Lox"] = "Lox",
        ["Tick"] = "Tick",
        ["SeekerBrute"] = "Seeker Soldier",
        ["Gjall"] = "Gjall",
        ["Charred_Mage"] = "Charred Warlock",
        ["FallenValkyrie"] = "Fallen Valkyrie",
        ["Morgen"] = "Morgen"
    };

    public static string DisplayName(string prefabName)
        => Names.TryGetValue(prefabName, out string name) ? name : prefabName.Replace('_', ' ');
}
