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

        // Clone only the mesh object bearing the original ForceField material.
        // It can be a sibling of the EffectArea in Valheim's prefab hierarchy.
        var bubble = UnityEngine.Object.Instantiate(visual.gameObject, parent);
        bubble.name = "ImmersiveTrader_HaldorForceField";
        for (int i = bubble.transform.childCount - 1; i >= 0; i--)
            UnityEngine.Object.DestroyImmediate(bubble.transform.GetChild(i).gameObject);
        bubble.transform.localPosition = visual.transform.position - area.transform.position;
        bubble.transform.localRotation = visual.transform.rotation;
        bubble.transform.localScale = visual.transform.lossyScale;

        if (!_loggedDetails)
            Plugin.Log.LogInfo($"Haldor shield copied: area={area.name}, visual={visual.name}, radius={protection.GetComponent<SphereCollider>()?.radius}.");
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
