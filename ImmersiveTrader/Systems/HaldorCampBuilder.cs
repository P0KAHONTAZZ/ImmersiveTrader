using System;
using System.Linq;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>Uses Valheim's Haldor camp, including its native protective field.</summary>
public static class HaldorCampBuilder
{
    public static bool Build(Transform parent, Transform traderAnchor)
    {
        var sourceLocation = ZoneManager.Instance.GetZoneLocation("Vendor_BlackForest");
        if (sourceLocation == null)
        {
            Plugin.Log.LogWarning("Vendor_BlackForest is unavailable; using the simple trader camp.");
            return false;
        }

        sourceLocation.m_prefab.Load();
        var source = sourceLocation.m_prefab.Asset;
        if (source == null)
        {
            Plugin.Log.LogWarning("Vendor_BlackForest prefab failed to load; using the simple trader camp.");
            return false;
        }

        // Jotunn's location container is disabled while the template is assembled,
        // so the vanilla characters cannot wake up before they are removed.
        var camp = UnityEngine.Object.Instantiate(source, parent);
        camp.name = "ImmersiveTrader_HaldorCamp";
        camp.transform.localPosition = Vector3.zero;
        camp.transform.localRotation = Quaternion.identity;

        var vanillaTraders = camp.GetComponentsInChildren<Trader>(true);
        if (vanillaTraders.Length == 0)
        {
            UnityEngine.Object.DestroyImmediate(camp);
            Plugin.Log.LogWarning("Vendor_BlackForest contains no Haldor trader; using the simple trader camp.");
            return false;
        }

        traderAnchor.localPosition = parent.InverseTransformPoint(vanillaTraders[0].transform.position);
        foreach (var trader in vanillaTraders)
            UnityEngine.Object.DestroyImmediate(trader.gameObject);

        // The native ForceField renderer and its other camp objects remain intact.
        // Do not clone Haldor himself: a second networked merchant would persist.
        bool hasField = camp.GetComponentsInChildren<Renderer>(true)
            .Any(renderer => renderer.sharedMaterials.Any(material =>
                material != null && material.name.IndexOf("ForceField", StringComparison.OrdinalIgnoreCase) >= 0));
        if (!hasField)
            Plugin.Log.LogWarning("Vendor_BlackForest has no ForceField renderer; check the camp in game.");

        return true;
    }
}
