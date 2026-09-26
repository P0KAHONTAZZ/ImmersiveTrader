using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader;

internal sealed class SE_EnhancementDamage : StatusEffect
{
    internal enum Kind { Fire, Frost, Lightning, Poison, Spirit, Chop, Pickaxe }
    internal Kind DamageKind;

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        base.ModifyAttack(skill, ref hitData);
        switch (DamageKind)
        {
            case Kind.Fire: hitData.m_damage.m_fire += Random.Range(5f, 10f); break;
            case Kind.Frost: hitData.m_damage.m_frost += Random.Range(5f, 10f); break;
            case Kind.Lightning: hitData.m_damage.m_lightning += Random.Range(5f, 10f); break;
            case Kind.Poison: hitData.m_damage.m_poison += Random.Range(5f, 10f); break;
            case Kind.Spirit: hitData.m_damage.m_spirit += Random.Range(5f, 10f); break;
            case Kind.Chop: hitData.m_damage.m_chop *= 1.20f; break;
            case Kind.Pickaxe: hitData.m_damage.m_pickaxe *= 1.20f; break;
        }
    }
}

[HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyMaxCarryWeight))]
internal static class EnhancementCarryPatch
{
    private static void Postfix(SEMan __instance, float baseLimit, ref float limit)
    {
        if (Player.m_localPlayer != null && ReferenceEquals(Player.m_localPlayer.GetSEMan(), __instance) &&
            EnhancementStatusRegistry.Has(Player.m_localPlayer, "burden"))
            limit += baseLimit * 0.10f;
    }
}

[HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyAttackStaminaUsage))]
internal static class EnhancementAttackStaminaPatch
{
    private static void Postfix(SEMan __instance, float baseStaminaUse, ref float staminaUse)
    {
        var player = Player.m_localPlayer;
        if (player == null || !ReferenceEquals(player.GetSEMan(), __instance)) return;
        var weapon = player.GetCurrentWeapon();
        if (weapon == null) return;
        var skill = weapon.m_shared.m_skillType;

        if (EnhancementStatusRegistry.Has(player, "hunter") && skill == Skills.SkillType.Bows)
            staminaUse -= baseStaminaUse * 0.10f;

        if (EnhancementStatusRegistry.Has(player, "endurance") && IsMelee(skill))
            staminaUse -= baseStaminaUse * 0.10f;
    }

    private static bool IsMelee(Skills.SkillType skill) =>
        skill == Skills.SkillType.Axes || skill == Skills.SkillType.Clubs ||
        skill == Skills.SkillType.Knives || skill == Skills.SkillType.Polearms ||
        skill == Skills.SkillType.Swords || skill == Skills.SkillType.Unarmed ||
        skill == Skills.SkillType.Spears;
}

/// <summary>
/// Vitality: max HP x1.10, applied where the game stores the food-based maximum
/// (Character.SetMaxHealth, called by Player.UpdateFood about once per second).
/// Dynamic: follows food. On expiry the next food tick stores the plain maximum and vanilla
/// clamps current HP down to it (SetHealth to a positive value, never damage or death).
/// </summary>
[HarmonyPatch]
internal static class EnhancementMaxHealthPatch
{
    private static System.Reflection.MethodBase? Target() =>
        AccessTools.Method(typeof(Character), "SetMaxHealth", new[] { typeof(float) });

    private static bool Prepare()
    {
        if (Target() != null) return true;
        Plugin.Log.LogWarning("Vitality: Character.SetMaxHealth(float) not found; max HP bonus disabled.");
        return false;
    }

    private static System.Reflection.MethodBase TargetMethod() => Target()!;

    private static void Prefix(Character __instance, ref float __0)
    {
        if (__instance is Player player && ReferenceEquals(player, Player.m_localPlayer) &&
            EnhancementStatusRegistry.Has(player, "vitality"))
            __0 *= 1.10f;
    }
}

/// <summary>Focus: max Eitr x1.15 via Player.SetMaxEitr (same dynamic/clamp behaviour as Vitality).</summary>
[HarmonyPatch]
internal static class EnhancementMaxEitrPatch
{
    private static System.Reflection.MethodBase? Target() =>
        AccessTools.Method(typeof(Player), "SetMaxEitr", new[] { typeof(float), typeof(bool) })
        ?? AccessTools.Method(typeof(Player), "SetMaxEitr");

    private static bool Prepare()
    {
        if (Target() != null) return true;
        Plugin.Log.LogWarning("Focus: Player.SetMaxEitr not found; max Eitr bonus disabled.");
        return false;
    }

    private static System.Reflection.MethodBase TargetMethod() => Target()!;

    private static void Prefix(Player __instance, ref float __0)
    {
        if (ReferenceEquals(__instance, Player.m_localPlayer) && EnhancementStatusRegistry.Has(__instance, "focus"))
            __0 *= 1.15f;
    }
}
