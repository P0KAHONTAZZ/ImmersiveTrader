using ImmersiveTrader.Components;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

public static class LegendaryMietegRegistry
{
    private static bool _prefabRegistered;
    private static bool _locationsRegistered;

    public static void RegisterPrefab()
    {
        if (_prefabRegistered) return;

        var prefab = PrefabManager.Instance.CreateClonedPrefab("ImmersiveTrader_NPC_legendary_mieteg", "Haldor");
        if (prefab == null) return;

        var vanillaTrader = prefab.GetComponent<Trader>();
        if (vanillaTrader != null) Object.DestroyImmediate(vanillaTrader);

        var npc = prefab.GetComponent<TraderNpc>() ?? prefab.AddComponent<TraderNpc>();
        npc.TraderId = "legendary_mieteg";
        PrefabManager.Instance.AddPrefab(prefab);
        _prefabRegistered = true;
    }

    public static void RegisterLocations()
    {
        if (_locationsRegistered) return;

        // Prepared rare sites. Runtime activation/deactivation is deliberately separated
        // from world generation so one site can be selected for a 1-2 day appearance.
        foreach (var biome in new[] { Heightmap.Biome.BlackForest, Heightmap.Biome.Swamp, Heightmap.Biome.Mountain, Heightmap.Biome.Plains })
        {
            var container = ZoneManager.Instance.CreateLocationContainer($"ImmersiveTrader_MietegSite_{biome}");
            var prefab = PrefabManager.Instance.GetPrefab("ImmersiveTrader_NPC_legendary_mieteg");
            if (prefab == null) continue;

            var npc = Object.Instantiate(prefab, container.transform);
            npc.name = prefab.name;
            npc.transform.localPosition = Vector3.zero;

            var config = new LocationConfig
            {
                Biome = biome,
                Quantity = 2,
                Unique = false,
                Priotized = false,
                ClearArea = false,
                ExteriorRadius = 5f,
                MinDistance = 2500f,
                MaxDistance = 8000f,
                MinDistanceFromSimilar = 2500f,
                Group = "ImmersiveTrader_LegendaryMieteg",
                IconPlaced = false,
                IconAlways = false
            };

            ZoneManager.Instance.AddCustomLocation(new CustomLocation(container, false, config));
        }

        _locationsRegistered = true;
    }
}
