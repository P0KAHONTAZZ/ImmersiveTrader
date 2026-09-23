namespace ImmersiveTrader.Models;

public sealed record RewardDefinition(
    string TraderId,
    string TreasureId,
    string ItemPrefab,
    int BaseAmount
);