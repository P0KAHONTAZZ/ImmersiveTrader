using System;

namespace ImmersiveTrader;

/// <summary>
/// Reduces the kill requirement of newly issued contracts according to the
/// player's reputation with the issuing trader. Issued scrolls keep their
/// stamped requirement, so gaining reputation never mutates an active contract.
/// </summary>
public static class ContractRequirementScaling
{
    public static float MultiplierForLevel(int reputationLevel) => reputationLevel switch
    {
        <= 1 => 1.00f,
        2 => 0.90f,
        3 => 0.80f,
        4 => 0.70f,
        _ => 0.60f
    };

    public static int Scale(int baseRequired, int reputationLevel)
    {
        if (baseRequired <= 1) return Math.Max(1, baseRequired);
        // Ceiling avoids making small elite-monster contracts disproportionately easy.
        return Math.Max(1, (int)Math.Ceiling(baseRequired * MultiplierForLevel(reputationLevel)));
    }
}
