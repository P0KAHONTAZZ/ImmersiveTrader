using Jotunn.Entities;
using Jotunn.Managers;

namespace ImmersiveTrader;

public static class ContractRegistry
{
    public const string PrefabName = "ImmersiveTrader_ContractScroll";
    public static void Register()
    {
        var source = PrefabManager.Instance.GetPrefab("YmirRemains");
        if (source == null) { Plugin.Log.LogWarning("Contract scroll base prefab missing: YmirRemains"); return; }
        var custom = new CustomItem(PrefabName, source);
        var shared = custom.ItemDrop.m_itemData.m_shared;
        // StoreGui.FillList accesses icon slot zero for every offered item.
        // YmirRemains can have no inventory icon in the current Valheim build.
        var coinIcons = PrefabManager.Instance.GetPrefab("Coins")?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_icons;
        if (shared.m_icons == null || shared.m_icons.Length == 0)
        {
            if (coinIcons == null || coinIcons.Length == 0 || coinIcons[0] == null)
            {
                Plugin.Log.LogError("Contract scroll has no icon and Coins fallback is unavailable; skipping registration.");
                return;
            }
            shared.m_icons = new[] { coinIcons[0] };
            Plugin.Log.LogWarning("Contract scroll uses temporary Coins icon until dedicated art is available.");
        }
        shared.m_name = "Hunting Contract";
        shared.m_description = "Hunting contract. Its title is written by the issuing trader. Carry it while hunting; progress is stored on this scroll and survives world reloads. Return it to the issuer when complete.";
        shared.m_weight = 0.1f;
        shared.m_maxStackSize = 1;
        shared.m_teleportable = true;
        ItemManager.Instance.AddItem(custom);
        Plugin.Log.LogInfo("Physical hunting contract scroll registered.");
    }
}
