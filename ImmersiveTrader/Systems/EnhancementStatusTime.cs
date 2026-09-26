using HarmonyLib;

namespace ImmersiveTrader;

internal static class EnhancementStatusTime
{
    private static readonly AccessTools.FieldRef<StatusEffect, float> Time =
        AccessTools.FieldRefAccess<StatusEffect, float>("m_time");

    internal static float Remaining(StatusEffect effect) =>
        System.Math.Max(0f, effect.m_ttl - Time(effect));

    internal static void Reset(StatusEffect effect) =>
        Time(effect) = 0f;
}
