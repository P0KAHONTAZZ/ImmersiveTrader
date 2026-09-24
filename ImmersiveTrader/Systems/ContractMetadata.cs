using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace ImmersiveTrader;

public static class ContractMetadata
{
    private const string ContractIdKey = "ImmersiveTrader.ContractId";
    private const string TraderIdKey = "ImmersiveTrader.ContractTrader";
    private const string ProgressKey = "ImmersiveTrader.ContractProgress";
    private const string IssuedDayKey = "ImmersiveTrader.ContractIssuedDay";
    private const string RequiredKey = "ImmersiveTrader.ContractRequired";
    private const string VariantKey = "ImmersiveTrader.ContractVariant";
    private const string RewardSkillKey = "ImmersiveTrader.ContractRewardSkill";

    // Scrolls issued before the 24-skill expansion have no reward snapshot.
    // Keep their originally advertised reward after the definitions change.
    private static readonly IReadOnlyDictionary<string, Skills.SkillType> LegacyRewards =
        new Dictionary<string, Skills.SkillType>
        {
            ["midka_1"] = Skills.SkillType.Knives,
            ["midka_2"] = Skills.SkillType.Run,
            ["midka_3"] = Skills.SkillType.Blocking,
            ["midka_4"] = Skills.SkillType.Bows,
            ["midka_5"] = Skills.SkillType.Jump,
            ["troldad_1"] = Skills.SkillType.Run,
            ["troldad_2"] = Skills.SkillType.Blocking,
            ["troldad_3"] = Skills.SkillType.Bows,
            ["troldad_4"] = Skills.SkillType.Jump,
            ["troldad_5"] = Skills.SkillType.Sneak,
            ["grimvald_1"] = Skills.SkillType.Axes,
            ["grimvald_2"] = Skills.SkillType.Clubs,
            ["grimvald_3"] = Skills.SkillType.Swords,
            ["grimvald_4"] = Skills.SkillType.Sneak,
            ["grimvald_5"] = Skills.SkillType.Blocking,
            ["rudy_1"] = Skills.SkillType.Bows,
            ["rudy_2"] = Skills.SkillType.Spears,
            ["rudy_3"] = Skills.SkillType.Knives,
            ["rudy_4"] = Skills.SkillType.Run,
            ["rudy_5"] = Skills.SkillType.Sneak,
            ["dzika_1"] = Skills.SkillType.Spears,
            ["dzika_2"] = Skills.SkillType.Swords,
            ["dzika_3"] = Skills.SkillType.Blocking,
            ["dzika_4"] = Skills.SkillType.Clubs,
            ["dzika_5"] = Skills.SkillType.Axes,
            ["encek_1"] = Skills.SkillType.Polearms,
            ["encek_2"] = Skills.SkillType.Run,
            ["encek_3"] = Skills.SkillType.Clubs,
            ["encek_4"] = Skills.SkillType.Blocking,
            ["encek_5"] = Skills.SkillType.Sneak,
            ["hrothgar_1"] = Skills.SkillType.Bows,
            ["hrothgar_2"] = Skills.SkillType.Blocking,
            ["hrothgar_3"] = Skills.SkillType.Pickaxes,
            ["hrothgar_4"] = Skills.SkillType.Swords,
            ["hrothgar_5"] = Skills.SkillType.Run,
            ["ylva_1"] = Skills.SkillType.Spears,
            ["ylva_2"] = Skills.SkillType.Knives,
            ["ylva_3"] = Skills.SkillType.Blocking,
            ["ylva_4"] = Skills.SkillType.Bows,
            ["ylva_5"] = Skills.SkillType.Jump,
            ["bjarki_1"] = Skills.SkillType.Swords,
            ["bjarki_2"] = Skills.SkillType.Spears,
            ["bjarki_3"] = Skills.SkillType.Blocking,
            ["bjarki_4"] = Skills.SkillType.Bows,
            ["bjarki_5"] = Skills.SkillType.Polearms,
            ["ragnar_1"] = Skills.SkillType.Axes,
            ["ragnar_2"] = Skills.SkillType.Bows,
            ["ragnar_3"] = Skills.SkillType.Blocking,
            ["ragnar_4"] = Skills.SkillType.Clubs,
            ["ragnar_5"] = Skills.SkillType.Run,
            ["cmok_1"] = Skills.SkillType.Polearms,
            ["cmok_2"] = Skills.SkillType.Blocking,
            ["cmok_3"] = Skills.SkillType.Knives,
            ["cmok_4"] = Skills.SkillType.Bows,
            ["cmok_5"] = Skills.SkillType.Run,
            ["grelka_1"] = Skills.SkillType.Swords,
            ["grelka_2"] = Skills.SkillType.Clubs,
            ["grelka_3"] = Skills.SkillType.Spears,
            ["grelka_4"] = Skills.SkillType.Sneak,
            ["grelka_5"] = Skills.SkillType.Jump,
            ["zenek_1"] = Skills.SkillType.Swords,
            ["zenek_2"] = Skills.SkillType.Blocking,
            ["zenek_3"] = Skills.SkillType.Bows,
            ["zenek_4"] = Skills.SkillType.Spears,
            ["zenek_5"] = Skills.SkillType.Run,
            ["skjold_1"] = Skills.SkillType.Axes,
            ["skjold_2"] = Skills.SkillType.Polearms,
            ["skjold_3"] = Skills.SkillType.Blocking,
            ["skjold_4"] = Skills.SkillType.Bows,
            ["skjold_5"] = Skills.SkillType.Sneak
        };

    public static void Stamp(ItemDrop.ItemData item, string contractId, string traderId, int issuedDay, int requiredAmount, bool rare, Skills.SkillType rewardSkill)
    {
        item.m_customData[ContractIdKey] = contractId;
        item.m_customData[TraderIdKey] = traderId;
        item.m_customData[ProgressKey] = "0";
        item.m_customData[IssuedDayKey] = issuedDay.ToString(CultureInfo.InvariantCulture);
        item.m_customData[RequiredKey] = requiredAmount.ToString(CultureInfo.InvariantCulture);
        item.m_customData[VariantKey] = rare ? "Rare" : "Normal";
        item.m_customData[RewardSkillKey] = rewardSkill.ToString();
    }

    public static int GetRequiredAmount(ItemDrop.ItemData item, int fallback)
        => item.m_customData.TryGetValue(RequiredKey, out var raw) &&
           int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int required) && required > 0
            ? required : fallback;

    public static Skills.SkillType GetRewardSkill(ItemDrop.ItemData item, string contractId, Skills.SkillType fallback)
    {
        if (item.m_customData.TryGetValue(RewardSkillKey, out var stored) &&
            Enum.TryParse(stored, out Skills.SkillType skill) && Enum.IsDefined(typeof(Skills.SkillType), skill))
            return skill;
        return LegacyRewards.TryGetValue(contractId, out var original) ? original : fallback;
    }

    public static bool TryRead(ItemDrop.ItemData item, out string contractId, out string traderId, out int progress)
    {
        contractId = string.Empty; traderId = string.Empty; progress = 0;
        // Metadata on another item must never turn it into a contract. This also
        // excludes unstamped generic scrolls from progress and turn-in handling.
        if (item.m_dropPrefab == null || !item.m_customData.TryGetValue(ContractIdKey, out contractId))
            return false;
        string prefab = item.m_dropPrefab.name.Replace("(Clone)", string.Empty);
        if (!string.Equals(prefab, ContractRegistry.PrefabName, StringComparison.Ordinal) &&
            !string.Equals(prefab, ContractRegistry.GetPrefabName(contractId), StringComparison.Ordinal))
            return false;

        return !string.IsNullOrEmpty(contractId)
            && item.m_customData.TryGetValue(TraderIdKey, out traderId)
            && item.m_customData.TryGetValue(ProgressKey, out var raw)
            && !string.IsNullOrEmpty(traderId)
            && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out progress)
            && progress >= 0;
    }

    public static void SetProgress(ItemDrop.ItemData item, int progress)
        => item.m_customData[ProgressKey] = progress.ToString(CultureInfo.InvariantCulture);

    public static string GetProgressText(ItemDrop.ItemData item)
    {
        if (!TryRead(item, out string contractId, out string issuer, out int progress))
            return string.Empty;
        var definition = TraderActivityRegistry.Activities.FirstOrDefault(x => x.Id == contractId && x.TraderId == issuer);
        if (definition == null) return string.Empty;
        int required = GetRequiredAmount(item, definition.RequiredAmount);
        var rewardSkill = GetRewardSkill(item, contractId, definition.RewardSkill);
        string variant = item.m_customData.TryGetValue(VariantKey, out var stored) ? stored : "Normal";
        return $"{definition.Title} ({variant})\nCel: {required} x {definition.TargetPrefab}" +
            $"\nPostęp: {progress}/{required}" +
            $"\nNagroda: {definition.RewardSkillLevels:0} EXP ({rewardSkill})" +
            $"\nOddaj zwój: {issuer}";
    }
}
