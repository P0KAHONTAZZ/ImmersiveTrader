using System;
using System.Linq;
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

        // Persisted ZDOs are recreated by ZNetScene before/while a zone loads. If that
        // trader already exists around this camp, never instantiate another copy.
        foreach (var npc in UnityEngine.Object.FindObjectsOfType<TraderNpc>())
        {
            if (npc == null || !string.Equals(npc.TraderId, TraderId, StringComparison.OrdinalIgnoreCase)) continue;
            if (Vector3.Distance(npc.transform.position, camp) <= radius) return;
        }

        string prefabName = $"ImmersiveTrader_NPCLOOK_{TraderId}";
        var prefab = PrefabManager.Instance.GetPrefab(prefabName);
        if (prefab == null)
        {
            Plugin.Log.LogWarning($"Location spawner: prefab missing: {prefabName}");
            return;
        }

        var spawned = UnityEngine.Object.Instantiate(prefab, camp, transform.rotation);
        if (spawned == null) return;

        var view = spawned.GetComponent<ZNetView>();
        if (view != null) view.m_persistent = true;
        Plugin.Log.LogInfo($"Location spawner created {TraderId} at {camp.x:0},{camp.z:0}");
    }
}
