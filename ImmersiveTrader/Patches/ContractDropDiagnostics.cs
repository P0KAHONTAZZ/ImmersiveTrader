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
        ContractMetadata.TryRead(item, out string contract, out _, out _);
        var registered = ZNetScene.instance?.GetPrefab(ContractRegistry.PrefabName);
        Plugin.Log.LogInfo($"Contract drop {contract}: prefab={prefab?.name ?? "NULL"}, " +
            $"registered={(registered != null)}, network={(prefab?.GetComponent<ZNetView>() != null)}, " +
            $"physics={(prefab?.GetComponent<UnityEngine.Rigidbody>() != null)}, " +
            $"renderers={prefab?.GetComponentsInChildren<UnityEngine.Renderer>(true).Length ?? 0}");
    }
}
