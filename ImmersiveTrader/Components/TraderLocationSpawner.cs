using System;
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

        // Runtime safety: if the persisted trader has already materialized in this camp,
        // do not create another copy. ZDOMan internals are intentionally not accessed here
        // because Valheim 1.0 no longer exposes the old m_objectsByID field.
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
