using System.Linq;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Builds decorative camp props defensively. Missing/renamed vanilla prefabs are skipped
/// instead of preventing the trader location from spawning.
/// </summary>
public static class TraderCampBuilder
{
    public static void Build(string traderId, Transform parent)
    {
        var camp = TraderCampRegistry.Camps.FirstOrDefault(x => x.TraderId == traderId);
        if (camp == null) return;

        foreach (var prop in camp.Props)
        {
            var source = PrefabManager.Instance.GetPrefab(prop.Prefab);
            if (source == null) continue;

            var instance = Object.Instantiate(source, parent);
            instance.name = $"ImmersiveTrader_Camp_{traderId}_{prop.Prefab}";
            instance.transform.localPosition = prop.Position;
            instance.transform.localRotation = Quaternion.Euler(prop.Rotation);
            instance.transform.localScale = source.transform.localScale * prop.Scale;
        }
    }
}
