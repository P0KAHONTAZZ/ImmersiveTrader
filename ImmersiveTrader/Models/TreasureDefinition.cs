namespace ImmersiveTrader.Models;

public sealed record TreasureDefinition(
    string Id,
    string DisplayName,
    string BasePrefabName,
    float Weight
);