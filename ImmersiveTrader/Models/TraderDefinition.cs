namespace ImmersiveTrader.Models;

public sealed record TraderDefinition(
    string Id,
    string Name,
    string Biome,
    int BiomeTier,
    string Theme,
    bool LiesAboutRewards = false,
    bool IsLegendary = false
);