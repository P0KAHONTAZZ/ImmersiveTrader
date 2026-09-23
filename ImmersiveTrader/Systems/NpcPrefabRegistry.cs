using ImmersiveTrader.Components;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

public static class NpcPrefabRegistry
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered) return;

        foreach (var trader in TraderRegistry.Traders)
        {
            if (trader.IsLegendary) continue;

            // Haldor is used as a stable passive trader base. Custom visuals will replace
            // individual traders in the asset-bundle stage.
            var prefab = PrefabManager.Instance.CreateClonedPrefab($"ImmersiveTrader_NPC_{trader.Id}", "Haldor");
            if (prefab == null) continue;

            var vanillaTrader = prefab.GetComponent<Trader>();
            if (vanillaTrader != null)
                Object.DestroyImmediate(vanillaTrader);

            var interaction = prefab.GetComponent<TraderNpc>() ?? prefab.AddComponent<TraderNpc>();
            interaction.TraderId = trader.Id;

            PrefabManager.Instance.AddPrefab(prefab);
        }

        _registered = true;
    }
}
