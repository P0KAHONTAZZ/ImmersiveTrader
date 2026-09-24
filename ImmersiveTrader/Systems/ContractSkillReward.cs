using System;
using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>Grants exact skill levels while retaining partial progress to the next level.</summary>
public static class ContractSkillReward
{
    public static bool TryGrant(Player player, Skills.SkillType type, int levels, out float before, out float after)
    {
        before = after = 0f;
        try
        {
            var skills = AccessTools.Method(typeof(Player), "GetSkills")?.Invoke(player, null)
                ?? AccessTools.Field(typeof(Player), "m_skills")?.GetValue(player);
            var getSkill = AccessTools.Method(typeof(Skills), "GetSkill", new[] { typeof(Skills.SkillType) });
            var skill = skills == null ? null : getSkill?.Invoke(skills, new object[] { type });
            if (skill == null) throw new InvalidOperationException($"Skill {type} is unavailable.");

            var levelField = AccessTools.Field(skill.GetType(), "m_level");
            var progressField = AccessTools.Field(skill.GetType(), "m_accumulator");
            var nextRequirement = AccessTools.Method(skill.GetType(), "GetNextLevelRequirement");
            if (levelField == null || progressField == null || nextRequirement == null)
                throw new InvalidOperationException("Valheim skill fields are unavailable.");

            before = Convert.ToSingle(levelField.GetValue(skill));
            after = Mathf.Min(100f, before + levels);
            if (after <= before) return false;

            float oldRequirement = Convert.ToSingle(nextRequirement.Invoke(skill, null));
            float progress = Convert.ToSingle(progressField.GetValue(skill));
            float fraction = oldRequirement > 0f ? Mathf.Clamp01(progress / oldRequirement) : 0f;

            try
            {
                levelField.SetValue(skill, after);
                float newRequirement = after < 100f ? Convert.ToSingle(nextRequirement.Invoke(skill, null)) : 0f;
                progressField.SetValue(skill, after < 100f ? fraction * newRequirement : 0f);
            }
            catch
            {
                levelField.SetValue(skill, before);
                progressField.SetValue(skill, progress);
                throw;
            }
            return true;
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Contract skill reward failed for {type}: {ex}");
            return false;
        }
    }
}
