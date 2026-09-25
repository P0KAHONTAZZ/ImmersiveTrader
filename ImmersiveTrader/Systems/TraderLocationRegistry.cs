using System.Linq;
using ImmersiveTrader.Components;
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

            // The generated location owns a lightweight, non-networked anchor. The
            // anchor creates exactly one live trader while this location is loaded and
            // removes it with the location, so no trader ZDO survives independently.
            var anchor = new GameObject($"ImmersiveTrader_Anchor_{trader.Id}");
            anchor.transform.SetParent(container.transform, false);
            // An open, level camp places the trader at its center.
            anchor.transform.localPosition = Vector3.zero;
            bool hasVanillaShield = HaldorShieldBuilder.Build(container.transform);
            var traderAnchor = anchor.AddComponent<TraderLocationAnchor>();
            traderAnchor.TraderId = trader.Id;

            TraderCampBuilder.Build(trader.Id, container.transform);
            float levelRadius = TraderCampRegistry.Camps.FirstOrDefault(x => x.TraderId == trader.Id)?.LevelRadius ?? 0f;
            if (levelRadius > 0f)
                AddLeveling(container.transform, levelRadius);
            if (!hasVanillaShield)
            {
                Plugin.Log.LogWarning($"Vanilla trader shield unavailable for {trader.Id}; using the previous protection.");
                anchor.AddComponent<TraderSanctuary>();
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
                ExteriorRadius = Mathf.Max(14f, levelRadius + 2f),
                MinAltitude = 3f,
                MinDistance = range.min,
                MaxDistance = range.max,
                MinDistanceFromSimilar = 900f,
                Group = $"ImmersiveTrader_{trader.Id}",
                IconPlaced = true,
                IconAlways = true
            };

            ZoneManager.Instance.AddCustomLocation(new CustomLocation(container, false, config));
        }

        _registered = true;
    }

    // Same mechanism vanilla locations use: flatten a round pad at the location origin height.
    private static void AddLeveling(Transform parent, float radius)
    {
        var leveling = new GameObject("ImmersiveTrader_Leveling");
        leveling.transform.SetParent(parent, false);
        var terrain = leveling.AddComponent<TerrainModifier>();
        terrain.m_level = true;
        terrain.m_levelRadius = radius;
        terrain.m_square = false;
        terrain.m_smooth = true;
        terrain.m_smoothRadius = radius + 4f;
        terrain.m_paintCleared = false;
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