using System;
using System.Collections.Generic;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Lightweight location child. The location itself contains no persistent Character.
/// When the zone is instantiated, the owner creates one networked trader only if an
/// existing persisted trader for this exact camp is not already present.
/// </summary>
public sealed class TraderLocationSpawner : MonoBehaviour
{
    public string TraderId = string.Empty;
    private bool _attempted;

    private void Start()
    {
        TryEnsureTrader();
    }

    private void TryEnsureTrader()
    {
        if (_attempted || string.IsNullOrEmpty(TraderId) || ZNetScene.instance == null) return;
        _attempted = true;

        // Only the authoritative peer may create the persistent network object.
        var markerView = GetComponentInParent<ZNetView>();
        if (markerView != null && markerView.IsValid() && !markerView.IsOwner()) return;

        Vector3 camp = transform.position;
        const float radius = 12f;

        // Query the world's ZDO sectors around this camp before looking at live objects.
        // Valheim 1.0 exposes FindObjects for this purpose; a persisted trader can exist
        // in the ZDO database before ZNetScene has materialized its GameObject.
        if (ZDOMan.instance != null)
        {
            var sector = ZoneSystem.GetZone(camp);
            var zdos = new List<ZDO>();
            var visited = new HashSet<ZDOID>();
            ZDOMan.instance.FindObjects(sector, zdos, visited);

            foreach (var zdo in zdos)
            {
                if (zdo == null || !zdo.IsValid()) continue;
                var zdoPrefab = ZNetScene.instance.GetPrefab(zdo.GetPrefab());
                string zdoPrefabName = zdoPrefab?.name ?? string.Empty;
                if (!zdoPrefabName.Equals($"ImmersiveTrader_NPCLOOK_{TraderId}", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (Vector3.Distance(zdo.GetPosition(), camp) <= radius)
                    return;
            }
        }

        // Also cover the first live load where the object may already be instantiated.
        foreach (var npc in UnityEngine.Object.FindObjectsOfType<TraderNpc>())
        {
            if (npc != null && string.Equals(npc.TraderId, TraderId, StringComparison.OrdinalIgnoreCase) &&
                Vector3.Distance(npc.transform.position, camp) <= radius)
                return;
        }

        string npcPrefabName = $"ImmersiveTrader_NPCLOOK_{TraderId}";
        var prefab = PrefabManager.Instance.GetPrefab(npcPrefabName);
        if (prefab == null)
        {
            Plugin.Log.LogWarning($"Location spawner: prefab missing: {npcPrefabName}");
            return;
        }

        var spawned = UnityEngine.Object.Instantiate(prefab, camp, transform.rotation);
        if (spawned == null) return;

        var view = spawned.GetComponent<ZNetView>();
        if (view != null) view.m_persistent = true;
        Plugin.Log.LogInfo($"Location spawner created {TraderId} at {camp.x:0},{camp.z:0}");
    }
}
