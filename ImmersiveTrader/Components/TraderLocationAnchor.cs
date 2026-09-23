using System;
using System.Collections;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Non-networked child of a generated trader location. The location is the source of
/// truth. Every time its zone materializes, the anchor ensures one live, non-persistent
/// trader exists for that camp. NPC lifetime therefore follows the location/zone instead
/// of creating a second persistent ZDO lifecycle.
/// </summary>
public sealed class TraderLocationAnchor : MonoBehaviour
{
    public string TraderId = string.Empty;
    private GameObject? _npc;

    private IEnumerator Start()
    {
        // Let the complete location hierarchy materialize before creating the character.
        yield return null;
        EnsureNpc();
    }

    private void OnEnable()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(EnsureNextFrame());
    }

    private IEnumerator EnsureNextFrame()
    {
        yield return null;
        EnsureNpc();
    }

    private void EnsureNpc()
    {
        if (_npc != null || string.IsNullOrEmpty(TraderId) || ZNetScene.instance == null)
            return;

        const float radius = 8f;
        foreach (var npc in UnityEngine.Object.FindObjectsByType<TraderNpc>(FindObjectsSortMode.None))
        {
            if (npc == null || !string.Equals(npc.TraderId, TraderId, StringComparison.OrdinalIgnoreCase))
                continue;
            if (Vector3.Distance(npc.transform.position, transform.position) <= radius)
            {
                _npc = npc.gameObject;
                return;
            }
        }

        string prefabName = $"ImmersiveTrader_NPCLOOK_{TraderId}";
        var prefab = PrefabManager.Instance.GetPrefab(prefabName);
        if (prefab == null)
        {
            Plugin.Log.LogWarning($"Location anchor: prefab missing: {prefabName}");
            return;
        }

        _npc = UnityEngine.Object.Instantiate(prefab, transform.position, transform.rotation);
        _npc.name = $"ImmersiveTrader_LocationNpc_{TraderId}";

        // This instance belongs to the currently loaded location. Do not leave a ZDO
        // behind after the zone unloads; the anchor will recreate it next time.
        var view = _npc.GetComponent<ZNetView>();
        if (view != null)
            view.m_persistent = false;

        Plugin.Log.LogInfo($"Location anchor created {TraderId} at {transform.position.x:0},{transform.position.z:0}");
    }

    private void OnDestroy()
    {
        if (_npc == null) return;

        var view = _npc.GetComponent<ZNetView>();
        if (view != null && view.IsValid() && ZNetScene.instance != null)
        {
            if (!view.IsOwner())
                view.ClaimOwnership();
            ZNetScene.instance.Destroy(_npc);
        }
        else
        {
            UnityEngine.Object.Destroy(_npc);
        }

        _npc = null;
    }
}
