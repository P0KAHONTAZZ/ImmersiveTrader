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

    public static void Stamp(ItemDrop.ItemData item, string contractId, string traderId, int issuedDay)
    {
        item.m_customData[ContractIdKey] = contractId;
        item.m_customData[TraderIdKey] = traderId;
        item.m_customData[ProgressKey] = "0";
        item.m_customData[IssuedDayKey] = issuedDay.ToString(CultureInfo.InvariantCulture);
    }

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
        return definition == null ? string.Empty : $"{definition.Title}: {progress}/{definition.RequiredAmount}";
    }
}
