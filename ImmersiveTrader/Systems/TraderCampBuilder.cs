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

        foreach (var prop in camp.Props)
        {
            var source = PrefabManager.Instance.GetPrefab(prop.Prefab);
            if (source == null)
            {
                // Camps are atmosphere, never a hard dependency for trader generation.
                continue;
            }

            var instance = Object.Instantiate(source, parent);
            instance.name = $"ImmersiveTrader_Camp_{traderId}_{prop.Prefab}";
            instance.transform.localPosition = prop.Position;
            instance.transform.localRotation = Quaternion.Euler(prop.Rotation);
            instance.transform.localScale = source.transform.localScale * prop.Scale;

            // Location containers are authored prefabs. Runtime-spawn networking on copied
            // decorative pieces is unnecessary and can create duplicate ownership/ZDO state.
            var nview = instance.GetComponent<ZNetView>();
            if (nview != null) Object.DestroyImmediate(nview);
        }
    }
}
