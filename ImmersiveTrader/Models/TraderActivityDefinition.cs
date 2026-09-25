namespace ImmersiveTrader.Models;

public enum TraderActivityType
{
    Hunt
}

public sealed record TraderActivityDefinition(
    string TraderId,
    string Id,
    string Title,
    TraderActivityType Type,
    string TargetPrefab,
    int RequiredAmount,
    Skills.SkillType RewardSkill,
    float RewardSkillLevels,
    string Description
);
