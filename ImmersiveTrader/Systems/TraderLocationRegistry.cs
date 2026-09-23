using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderLocationRegistry
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered) return;

        foreach (var trader in TraderRegistry.Traders)
        {
            if (trader.IsLegendary) continue;

            // Jotunn's empty location container is already disabled. Build the location
            // directly inside it. This is the documented runtime-code path and prevents
            // Character/ZNetView lifecycle from running while the location is only a template.
            var container = ZoneManager.Instance.CreateLocationContainer($"ImmersiveTrader_Location_{trader.Id}");

            // The trader belongs to the location itself. It is deliberately non-persistent:
            // the generated location is the source of truth and recreates exactly one NPC
            // whenever its zone is loaded. This avoids a second, independent ZDO lifetime
            // that previously accumulated one extra trader across restarts.
            var npcPrefab = PrefabManager.Instance.GetPrefab($"ImmersiveTrader_NPCLOOK_{trader.Id}");
            if (npcPrefab != null)
            {
                var npc = Object.Instantiate(npcPrefab, container.transform);
                npc.name = $"ImmersiveTrader_LocationNpc_{trader.Id}";
                npc.transform.localPosition = Vector3.zero;
                npc.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Plugin.Log.LogWarning($"Location trader prefab missing: {trader.Id}");
            }

            TraderCampBuilder.Build(trader.Id, container.transform);

            var range = GetDistanceRange(trader.Biome);
            var config = new LocationConfig
            {
                Biome = ToBiome(trader.Biome),
                Quantity = 1,
                Unique = true,
                Priotized = true,
                CenterFirst = false,
                ClearArea = true,
                ExteriorRadius = 10f,
                MinDistance = range.min,
                MaxDistance = range.max,
                MinDistanceFromSimilar = 900f,
                Group = $"ImmersiveTrader_{trader.Biome.Replace(" ", "")}",
                IconPlaced = true,
                IconAlways = true
            };

            ZoneManager.Instance.AddCustomLocation(new CustomLocation(container, false, config));
        }

        _registered = true;
    }

    private static (float min, float max) GetDistanceRange(string biome) => biome switch
    {
        "Meadows" => (500f, 2000f),
        "Black Forest" => (1000f, 3000f),
        "Swamp" => (2000f, 4500f),
        "Mountains" => (3000f, 6000f),
        "Plains" => (4000f, 7000f),
        "Mistlands" => (5000f, 8500f),
        "Ashlands" => (7000f, 10000f),
        _ => (500f, 10000f)
    };

    private static Heightmap.Biome ToBiome(string biome) => biome switch
    {
        "Meadows" => Heightmap.Biome.Meadows,
        "Black Forest" => Heightmap.Biome.BlackForest,
        "Swamp" => Heightmap.Biome.Swamp,
        "Mountains" => Heightmap.Biome.Mountain,
        "Plains" => Heightmap.Biome.Plains,
        "Mistlands" => Heightmap.Biome.Mistlands,
        "Ashlands" => Heightmap.Biome.AshLands,
        _ => Heightmap.Biome.Meadows
    };
}