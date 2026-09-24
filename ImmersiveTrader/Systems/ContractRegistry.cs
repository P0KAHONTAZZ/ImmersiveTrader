using System.Linq;
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
        shared.m_name = "Hunting Contract";
        shared.m_description = "A sealed hunting contract. Carry it while hunting and return it to the issuing trader when complete.";
        shared.m_weight = 0.1f;
        shared.m_maxStackSize = 1;
        shared.m_teleportable = true;
        ItemManager.Instance.AddItem(custom);
        Plugin.Log.LogInfo("Physical hunting contract scroll registered.");
    }
}
