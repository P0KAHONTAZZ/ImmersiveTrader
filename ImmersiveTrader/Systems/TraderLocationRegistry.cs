using System.Collections.Generic;
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

            var container = ZoneManager.Instance.CreateLocationContainer($"ImmersiveTrader_Location_{trader.Id}");
            var npcPrefab = PrefabManager.Instance.GetPrefab($"ImmersiveTrader_NPC_{trader.Id}");
            if (npcPrefab == null) continue;

            var npc = Object.Instantiate(npcPrefab, container.transform);
            npc.name = npcPrefab.name;
            npc.transform.localPosition = Vector3.zero;

            var config = new LocationConfig
            {
                Biome = ToBiome(trader.Biome),
                Quantity = 1,
                Priotized = true,
                CenterFirst = false,
                ClearArea = true,
                ExteriorRadius = 8f,
                MinDistanceFromSimilar = 1000f
            };

            ZoneManager.Instance.AddCustomLocation(new CustomLocation(container, false, config));
        }

        _registered = true;
    }

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
