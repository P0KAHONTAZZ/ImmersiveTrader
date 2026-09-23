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
            var npcPrefab = PrefabManager.Instance.GetPrefab($"ImmersiveTrader_NPCLOOK_{trader.Id}");
            if (npcPrefab == null) continue;

            var npc = Object.Instantiate(npcPrefab, container.transform);
            npc.name = npcPrefab.name;
            npc.transform.localPosition = Vector3.zero;

            // Camp props belong to the generated location container so the trader never
            // spawns as an isolated character in an empty biome.
            TraderCampBuilder.Build(trader.Id, container.transform);

            if (trader.Id == "troldad")
            {
                var jackiePrefab = PrefabManager.Instance.GetPrefab("ImmersiveTrader_Jackie");
                if (jackiePrefab != null)
                {
                    var jackie = Object.Instantiate(jackiePrefab, container.transform);
                    jackie.name = "Jackie";
                    jackie.transform.localPosition = new Vector3(2.2f, 0f, 1.2f);
                    jackie.transform.localRotation = Quaternion.Euler(0f, 210f, 0f);

                    var companion = jackie.GetComponent<ImmersiveTrader.Components.JackieCompanion>()
                        ?? jackie.AddComponent<ImmersiveTrader.Components.JackieCompanion>();
                    companion.SetHome(npc.transform);
                }
            }

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
                IconAlways = false
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