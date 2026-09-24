using Jotunn.Entities;
using Jotunn.Managers;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class ContractRegistry
{
    public const string PrefabName = "ImmersiveTrader_ContractScroll";
    public static string GetPrefabName(string contractId) => $"{PrefabName}_{contractId}";

    public static void Register()
    {
        // Coins has a complete world-drop hierarchy (mesh, collider and rigidbody).
        // YmirRemains lacks a reliable dropped-item representation in this game build.
        // Replace this temporary base with a dedicated scroll prefab when art is ready.
        var source = PrefabManager.Instance.GetPrefab("Coins");
        if (source == null) { Plugin.Log.LogWarning("Contract scroll base prefab missing: Coins"); return; }
        // Keep the original prefab for scrolls created by earlier versions.
        RegisterItem(PrefabName, "Hunting Contract", source, ContractIconRegistry.Parchment);

        int registered = 0;
        foreach (TraderActivityDefinition definition in TraderActivityRegistry.Activities)
        {
            RegisterItem(GetPrefabName(definition.Id), definition.Title, source, ContractIconRegistry.Parchment);
            registered++;
        }
        Plugin.Log.LogInfo($"Physical hunting contract scrolls registered: {registered} named contracts + legacy scroll.");
    }

    private static void RegisterItem(string prefabName, string title, UnityEngine.GameObject source, UnityEngine.Sprite? parchment)
    {
        var custom = new CustomItem(prefabName, source);
        var shared = custom.ItemDrop.m_itemData.m_shared;
        // StoreGui.FillList accesses icon slot zero for every offered item.
        var coinIcons = PrefabManager.Instance.GetPrefab("Coins")?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_icons;
        if (parchment != null)
            shared.m_icons = new[] { parchment };
        else if (shared.m_icons == null || shared.m_icons.Length == 0 || shared.m_icons[0] == null)
        {
            if (coinIcons == null || coinIcons.Length == 0 || coinIcons[0] == null)
            {
                Plugin.Log.LogError("Contract scroll has no icon and Coins fallback is unavailable; skipping registration.");
                return;
            }
            shared.m_icons = new[] { coinIcons[0] };
            Plugin.Log.LogWarning("Contract scroll uses temporary Coins icon until dedicated art is available.");
        }
        shared.m_name = title;
        shared.m_description = "Hunting contract. Its title is written by the issuing trader. Carry it while hunting; progress is stored on this scroll and survives world reloads. Return it to the issuer when complete.";
        shared.m_weight = 0.1f;
        shared.m_maxStackSize = 1;
        shared.m_teleportable = true;
        ContractWorldModel.Attach(custom.ItemPrefab, parchment);
        ItemManager.Instance.AddItem(custom);
    }
}
