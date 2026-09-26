using System;
using System.Linq;
using Jotunn.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ImmersiveTrader.Components;

/// <summary>Visual-only hide cloak for Troldad's native Troll skeleton.</summary>
public sealed class TroldadVisualProps : MonoBehaviour
{
    private void Start()
    {
        try
        {
            var back = FindBone("Spine", "Chest", "spine", "Body");
            if (back != null) AttachVisual("CapeTrollHide", "attach_skin", back,
                new Vector3(0f, -0.15f, -0.4f), Quaternion.Euler(0f, 180f, 0f), 1.35f);
            else Plugin.Log.LogWarning("Troldad back bone unavailable; cloak skipped.");
        }
        catch (Exception error) { Plugin.Log.LogWarning($"Troldad visual props unavailable: {error}"); }
    }

    private Transform FindBone(params string[] names)
        => GetComponentsInChildren<Transform>(true).FirstOrDefault(x =>
            names.Any(name => string.Equals(x.name, name, StringComparison.OrdinalIgnoreCase)));

    private static void AttachVisual(string itemName, string attachmentName, Transform bone,
        Vector3 offset, Quaternion rotation, float scale)
    {
        var prefab = PrefabManager.Instance.GetPrefab(itemName);
        var source = prefab?.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(x => x.name == attachmentName);
        if (source == null) { Plugin.Log.LogWarning($"Troldad visual missing: {itemName}/{attachmentName}"); return; }

        var copy = Object.Instantiate(source.gameObject, bone, false);
        copy.name = "ImmersiveTrader_Troldad_" + itemName;
        copy.transform.localPosition = offset;
        copy.transform.localRotation = rotation;
        copy.transform.localScale = Vector3.one * scale;
        foreach (var component in copy.GetComponentsInChildren<Component>(true))
            if (component is Collider || component is Rigidbody || component is ZNetView ||
                component is ZSyncTransform || component is MonoBehaviour)
                Object.DestroyImmediate(component);
        foreach (var part in copy.GetComponentsInChildren<Transform>(true))
            part.gameObject.SetActive(true);
        Plugin.Log.LogInfo($"Troldad visual prop attached: {itemName}.");
    }
}
