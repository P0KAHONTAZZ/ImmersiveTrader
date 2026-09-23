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

        // The registered prefab is intentionally inactive. This prevents ZNetView from
        // creating a ZDO before we can enforce the location-owned non-persistent policy.
        bool wasActive = prefab.activeSelf;
        prefab.SetActive(false);
        _npc = UnityEngine.Object.Instantiate(prefab, transform.position, transform.rotation);
        prefab.SetActive(wasActive);
        _npc.name = $"ImmersiveTrader_LocationNpc_{TraderId}";

        var view = _npc.GetComponent<ZNetView>();
        if (view != null)
            view.m_persistent = false;
        _npc.SetActive(true);

        // Troldad keeps the current human-sized Troll scale, but uses the visual/state
        // of a two-star Troll. Valheim levels are 1-based: level 3 renders two stars.
        if (string.Equals(TraderId, "troldad", StringComparison.OrdinalIgnoreCase))
        {
            var character = _npc.GetComponent<Character>();
            if (character != null)
                character.SetLevel(3);
        }

        // This instance belongs to the currently loaded location. Do not leave a ZDO
        // behind after the zone unloads; the anchor will recreate it next time.

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
