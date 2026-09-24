using HarmonyLib;

namespace ImmersiveTrader.Patches;

[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.DropItem), new[] { typeof(Inventory), typeof(ItemDrop.ItemData), typeof(int) })]
internal static class ContractDropDiagnostics
{
    private static void Prefix(ItemDrop.ItemData __1)
    {
        var item = __1;
        var prefab = item.m_dropPrefab;
        if (prefab == null || !prefab.name.StartsWith(ContractRegistry.PrefabName)) return;
        var registered = ZNetScene.instance?.GetPrefab(ContractRegistry.PrefabName);
        bool network = prefab.GetComponent<ZNetView>() != null;
        bool physics = prefab.GetComponent<UnityEngine.Rigidbody>() != null;
        int renderers = prefab.GetComponentsInChildren<UnityEngine.Renderer>(true).Length;
        if (registered != null && network && physics && renderers > 0) return;
        ContractMetadata.TryRead(item, out string contract, out _, out _);
        Plugin.Log.LogWarning($"Contract drop {contract}: prefab={prefab?.name ?? "NULL"}, " +
            $"registered={(registered != null)}, network={network}, " +
            $"physics={physics}, " +
            $"renderers={renderers}");
    }
}
