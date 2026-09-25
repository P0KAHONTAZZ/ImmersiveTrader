namespace ImmersiveTrader.Models;

public sealed record TraderOfferDefinition(
    string TraderId,
    string ItemPrefab,
    int Price,
    int Stack,
    int RequiredTier,
    string Label
);
