using System;
using Jotunn.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ImmersiveTrader.Components;

/// <summary>Local Wisplight effect for the two Mistlands traders.</summary>
public sealed class TraderWispLight : MonoBehaviour
{
    private const string DecorationName = "ImmersiveTrader_ActiveWisplight";
    private float _nextAttempt;
    private Transform _orb;
    private float _phase;

    private void Update()
    {
        // Keep the vanilla demister and particle effects on the orb. Only its
        // local position moves, as a free Wisplight does near its wearer.
        if (_orb != null)
        {
            float t = Time.time * 0.85f + _phase;
            _orb.localPosition = new Vector3(
                0.4f + Mathf.Sin(t) * 0.45f,
                1.8f + Mathf.Sin(t * 1.4f) * 0.22f,
                0.2f + Mathf.Cos(t * 0.8f) * 0.35f);
            return;
        }
        if (Time.time < _nextAttempt) return;
        _nextAttempt = Time.time + 2f;
        if (Player.m_localPlayer == null) return; // Dedicated server creates no local visual.

        // The humanoid renderer policy hides objects mounted directly on the Hildir
        // shell. Attach to the Player visual that is allowed to render instead.
        var visual = transform.Find("ImmersiveTrader_PlayerVisual");
        if (visual == null || !visual.gameObject.activeInHierarchy) return;
        if (visual.Find(DecorationName) != null)
        {
            _orb = visual.Find(DecorationName);
            return;
        }

        var prefab = PrefabManager.Instance.GetPrefab("demister_ball") ??
            ZNetScene.instance?.GetPrefab("demister_ball");
        if (prefab == null)
        {
            Plugin.Log.LogWarning("Mistlands Wisplight prefab demister_ball is unavailable.");
            enabled = false;
            return;
        }

        GameObject copy = null;
        try
        {
            bool active = prefab.activeSelf;
            prefab.SetActive(false);
            try { copy = Object.Instantiate(prefab, visual); }
            finally { prefab.SetActive(active); }
            copy.name = DecorationName;
            copy.transform.localPosition = new Vector3(0.35f, 1.8f, 0.2f);
            copy.transform.localRotation = Quaternion.identity;
            // A visual child must never create a separate persisted network entity.
            foreach (var sync in copy.GetComponentsInChildren<ZSyncTransform>(true))
                Object.DestroyImmediate(sync);
            foreach (var view in copy.GetComponentsInChildren<ZNetView>(true))
                Object.DestroyImmediate(view);
            copy.SetActive(true);
            if (copy.GetComponentInChildren<Demister>(true) == null)
                throw new MissingComponentException("Wisplight effect has no Demister.");
            _orb = copy.transform;
            _phase = UnityEngine.Random.Range(0f, 6.28f);
            Plugin.Log.LogInfo($"Mistlands Wisplight active for {GetComponent<TraderNpc>()?.TraderId}.");
        }
        catch (Exception error)
        {
            Plugin.Log.LogWarning($"Mistlands Wisplight could not be attached: {error}");
            if (copy != null) Object.Destroy(copy);
            enabled = false;
        }
    }
}
