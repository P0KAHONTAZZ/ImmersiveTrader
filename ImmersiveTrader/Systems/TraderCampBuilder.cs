using System.Linq;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderCampBuilder
{
    public static void Build(string traderId, Transform parent)
    {
        var camp = TraderCampRegistry.Camps.FirstOrDefault(x => x.TraderId == traderId);
        if (camp == null) return;

        var root = new GameObject($"ImmersiveTrader_Camp_{traderId}");
        root.transform.SetParent(parent, false);

        foreach (var prop in camp.Props)
        {
            var source = PrefabManager.Instance.GetPrefab(prop.Prefab);
            if (source == null)
            {
                if (!prop.Optional)
                    Plugin.Log.LogWarning($"Required camp prop '{prop.Prefab}' missing for {traderId}.");
                continue;
            }

            var instance = Object.Instantiate(source, root.transform);
            instance.name = $"{prop.Prefab}_decor";
            instance.transform.localPosition = prop.Position;
            instance.transform.localRotation = Quaternion.Euler(prop.Rotation);
            instance.transform.localScale = source.transform.localScale * prop.Scale;

            MakeDecoration(instance);
        }
    }

    private static void MakeDecoration(GameObject instance)
    {
        // These objects are baked into the generated trader location. Keep their
        // networking components intact: many vanilla components resolve ZNetView during
        // Awake/Start. We only remove gameplay interaction components below. Jotunn fixes
        // prefab references when the CustomLocation is registered.
        foreach (var container in instance.GetComponentsInChildren<Container>(true))
            Object.DestroyImmediate(container);

        foreach (var crafting in instance.GetComponentsInChildren<CraftingStation>(true))
            Object.DestroyImmediate(crafting);

        foreach (var wear in instance.GetComponentsInChildren<WearNTear>(true))
            Object.DestroyImmediate(wear);
    }
}
