namespace ImmersiveTrader.Models;

public enum TraderActivityType
{
    Hunt,
    Gather
}

public sealed record TraderActivityDefinition(
    string TraderId,
    string Id,
    string Title,
    TraderActivityType Type,
    string TargetPrefab,
    int RequiredAmount,
    string RewardPrefab,
    int RewardAmount,
    string Description
);
