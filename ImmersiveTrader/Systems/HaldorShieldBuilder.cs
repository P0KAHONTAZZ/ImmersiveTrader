using System;
using System.Linq;
using System.Reflection;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>Transfers Haldor's visual shield and monster exclusion to our own camp.</summary>
public static class HaldorShieldBuilder
{
    private static bool _loggedDetails;

    public static bool Build(Transform parent)
    {
        var vendor = ZoneManager.Instance.GetZoneLocation("Vendor_BlackForest");
        if (vendor == null)
            return false;

        vendor.m_prefab.Load();
        var source = vendor.m_prefab.Asset;
        if (source == null)
            return false;

        var visual = source.GetComponentsInChildren<Renderer>(true).FirstOrDefault(renderer =>
            renderer.sharedMaterials.Any(material => material != null &&
                material.name.IndexOf("ForceField", StringComparison.OrdinalIgnoreCase) >= 0));

        var areas = source.GetComponentsInChildren<EffectArea>(true);
        var area = areas.FirstOrDefault(IsNoMonstersArea);
        if (area == null && visual != null)
            area = areas.OrderBy(a => (a.transform.position - visual.transform.position).sqrMagnitude).FirstOrDefault();

        if (visual == null || area == null)
        {
            if (!_loggedDetails)
                Plugin.Log.LogWarning($"Haldor shield components: ForceField={visual != null}, EffectAreas={areas.Length}.");
            _loggedDetails = true;
            return false;
        }

        // Copy the original EffectArea, collider and values. If it lives on the
        // location root, strip all its children so no camp or Haldor is duplicated.
        if (area.GetComponent<Trader>() != null || area.GetComponent<Character>() != null)
        {
            Plugin.Log.LogWarning("Haldor's EffectArea shares the merchant object; refusing to clone the NPC.");
            return false;
        }

        var protection = UnityEngine.Object.Instantiate(area.gameObject, parent);
        protection.name = "ImmersiveTrader_HaldorProtection";
        for (int i = protection.transform.childCount - 1; i >= 0; i--)
            UnityEngine.Object.DestroyImmediate(protection.transform.GetChild(i).gameObject);

        foreach (var component in protection.GetComponents<Component>())
        {
            if (component != null && (component.GetType().Name == "Location" ||
                                      component.GetType().Name == "LocationProxy"))
                UnityEngine.Object.DestroyImmediate(component);
        }

        if (protection.GetComponent<EffectArea>() == null || protection.GetComponent<Collider>() == null)
        {
            UnityEngine.Object.DestroyImmediate(protection);
            Plugin.Log.LogWarning("Haldor's monster protection has no local collider; using the previous protection.");
            return false;
        }

        protection.transform.localPosition = Vector3.zero;
        protection.transform.localRotation = Quaternion.identity;
        protection.transform.localScale = area.transform.lossyScale;

        // Clone only the mesh object bearing the original ForceField material.
        // It can be a sibling of the EffectArea in Valheim's prefab hierarchy.
        var visualRoot = visual.transform;
        while (visualRoot.parent != null && visualRoot.parent != source.transform &&
               visualRoot.parent.GetComponentsInChildren<Trader>(true).Length == 0 &&
               visualRoot.parent.GetComponentsInChildren<Character>(true).Length == 0)
            visualRoot = visualRoot.parent;

        if (visualRoot.GetComponentsInChildren<Trader>(true).Length != 0 ||
            visualRoot.GetComponentsInChildren<Character>(true).Length != 0)
        {
            UnityEngine.Object.DestroyImmediate(protection);
            Plugin.Log.LogWarning("ForceField is part of the Haldor character; refusing to duplicate him.");
            return false;
        }

        var bubble = UnityEngine.Object.Instantiate(visualRoot.gameObject, parent);
        bubble.name = "ImmersiveTrader_HaldorForceField";
        bubble.transform.localPosition = visualRoot.position - area.transform.position;
        bubble.transform.localRotation = visualRoot.rotation;
        bubble.transform.localScale = visualRoot.lossyScale;
        foreach (var effect in bubble.GetComponentsInChildren<EffectArea>(true))
            UnityEngine.Object.DestroyImmediate(effect);
        foreach (var renderer in bubble.GetComponentsInChildren<Renderer>(true))
            if (renderer.sharedMaterials.Any(material => material != null &&
                material.name.IndexOf("ForceField", StringComparison.OrdinalIgnoreCase) >= 0))
                renderer.enabled = true;

        var sourceRadius = area.GetComponent<SphereCollider>();
        var copyRadius = protection.GetComponent<SphereCollider>();
        float sourceWorldRadius = sourceRadius == null ? 0f :
            sourceRadius.radius * Mathf.Max(area.transform.lossyScale.x, area.transform.lossyScale.z);
        float copyWorldRadius = copyRadius == null ? 0f :
            copyRadius.radius * Mathf.Max(protection.transform.lossyScale.x, protection.transform.lossyScale.z);
        if (copyWorldRadius < 10f)
        {
            UnityEngine.Object.DestroyImmediate(bubble);
            UnityEngine.Object.DestroyImmediate(protection);
            Plugin.Log.LogWarning($"Haldor protection radius too small: {copyWorldRadius:0.0}m; source={sourceWorldRadius:0.0}m.");
            return false;
        }

        if (!_loggedDetails)
            Plugin.Log.LogInfo($"Haldor shield copied: area={area.name}, visual={visualRoot.name}, source radius={sourceWorldRadius:0.0}m, copy radius={copyWorldRadius:0.0}m.");
        _loggedDetails = true;
        return true;
    }

    private static bool IsNoMonstersArea(EffectArea area)
    {
        var field = typeof(EffectArea).GetField("m_type",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var value = field?.GetValue(area)?.ToString();
        return value != null && value.IndexOf("NoMonsters", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
