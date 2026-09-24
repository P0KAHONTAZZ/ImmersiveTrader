using System;
using System.Linq;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>Copies only Haldor's native shield subtree into each existing trader location.</summary>
public static class HaldorShieldBuilder
{
    public static bool Build(Transform parent)
    {
        var vendor = ZoneManager.Instance.GetZoneLocation("Vendor_BlackForest");
        if (vendor == null)
            return false;

        vendor.m_prefab.Load();
        var source = vendor.m_prefab.Asset;
        if (source == null)
            return false;

        foreach (var renderer in source.GetComponentsInChildren<Renderer>(true))
        {
            if (!renderer.sharedMaterials.Any(material =>
                material != null && material.name.IndexOf("ForceField", StringComparison.OrdinalIgnoreCase) >= 0))
                continue;

            // Clone the smallest complete subtree containing both the effect area
            // (monster exclusion) and its ForceField visual. Never clone a merchant.
            for (var node = renderer.transform; node != null; node = node.parent)
            {
                if (node.GetComponent<EffectArea>() == null)
                    continue;
                if (node.GetComponentsInChildren<Trader>(true).Length != 0 ||
                    node.GetComponentsInChildren<Character>(true).Length != 0)
                    break;

                var shield = UnityEngine.Object.Instantiate(node.gameObject, parent);
                shield.name = "ImmersiveTrader_HaldorShield";
                shield.transform.localPosition = Vector3.zero;
                shield.transform.localRotation = Quaternion.identity;
                Plugin.Log.LogInfo($"Attached vanilla Haldor shield from {node.name}.");
                return true;
            }
        }

        Plugin.Log.LogWarning("Could not isolate the vanilla ForceField and EffectArea from Vendor_BlackForest.");
        return false;
    }
}
