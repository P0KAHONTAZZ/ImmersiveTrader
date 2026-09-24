using System;
using System.Globalization;
using System.Linq;

namespace ImmersiveTrader;

public static class ContractMetadata
{
    private const string ContractIdKey = "ImmersiveTrader.ContractId";
    private const string TraderIdKey = "ImmersiveTrader.ContractTrader";
    private const string ProgressKey = "ImmersiveTrader.ContractProgress";
    private const string IssuedDayKey = "ImmersiveTrader.ContractIssuedDay";
    private const string RequiredKey = "ImmersiveTrader.ContractRequired";
    private const string VariantKey = "ImmersiveTrader.ContractVariant";

    public static void Stamp(ItemDrop.ItemData item, string contractId, string traderId, int issuedDay, int requiredAmount, bool rare)
    {
        item.m_customData[ContractIdKey] = contractId;
        item.m_customData[TraderIdKey] = traderId;
        item.m_customData[ProgressKey] = "0";
        item.m_customData[IssuedDayKey] = issuedDay.ToString(CultureInfo.InvariantCulture);
        item.m_customData[RequiredKey] = requiredAmount.ToString(CultureInfo.InvariantCulture);
        item.m_customData[VariantKey] = rare ? "Rare" : "Normal";
    }

    public static int GetRequiredAmount(ItemDrop.ItemData item, int fallback)
        => item.m_customData.TryGetValue(RequiredKey, out var raw) &&
           int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int required) && required > 0
            ? required : fallback;

    public static bool TryRead(ItemDrop.ItemData item, out string contractId, out string traderId, out int progress)
    {
        contractId = string.Empty; traderId = string.Empty; progress = 0;
        // Metadata on another item must never turn it into a contract. This also
        // excludes unstamped generic scrolls from progress and turn-in handling.
        if (item.m_dropPrefab == null ||
            !string.Equals(item.m_dropPrefab.name.Replace("(Clone)", string.Empty),
                ContractRegistry.PrefabName, StringComparison.Ordinal))
            return false;

        return item.m_customData.TryGetValue(ContractIdKey, out contractId)
            && item.m_customData.TryGetValue(TraderIdKey, out traderId)
            && item.m_customData.TryGetValue(ProgressKey, out var raw)
            && !string.IsNullOrEmpty(contractId)
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
        string variant = item.m_customData.TryGetValue(VariantKey, out var stored) ? stored : "Normal";
        return $"{definition.Title} ({variant})\nCel: {required} x {definition.TargetPrefab}" +
            $"\nPostęp: {progress}/{required}" +
            $"\nNagroda: {definition.RewardSkillLevels:0} EXP ({definition.RewardSkill})" +
            $"\nOddaj zwój: {issuer}";
    }
}
