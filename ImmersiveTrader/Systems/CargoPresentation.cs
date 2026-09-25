using System;
using System.Collections.Generic;
using UnityEngine;
using Jotunn.Managers;

namespace ImmersiveTrader;

/// <summary>
/// Cargo presentation intentionally reuses native Valheim item art.
/// This keeps every shipment readable in inventory and avoids maintaining
/// a partial custom atlas. The physical drop also uses the native resource
/// model rather than forcing every shipment into the same wooden chest.
/// </summary>
internal static class CargoPresentation
{
    private static readonly Dictionary<string, Sprite> Icons = new();

    private static readonly Dictionary<string, string> Resources = new()
    {
        ["field_medicine"] = "HealthPotion", ["healing_honey"] = "Honey", ["bandages"] = "LinenThread",
        ["herbs"] = "Dandelion", ["resin"] = "Resin", ["corewood"] = "RoundLog",
        ["hides"] = "TrollHide", ["meat"] = "RawMeat", ["wood"] = "Wood",
        ["amber"] = "Amber", ["copper"] = "Copper", ["tin"] = "Tin",
        ["cores"] = "SurtlingCore", ["arrows"] = "ArrowWood", ["rubies"] = "Ruby",
        ["thistle"] = "Thistle", ["entrails"] = "Entrails", ["scrap_iron"] = "IronScrap",
        ["iron"] = "Iron", ["roots"] = "Root", ["bloodbags"] = "Bloodbag", ["guck"] = "Guck",
        ["chains"] = "Chain", ["sausages"] = "Sausages", ["bark"] = "ElderBark",
        ["obsidian"] = "Obsidian", ["silver"] = "Silver", ["wolf_meat"] = "WolfMeat",
        ["crystal"] = "Crystal", ["pelts"] = "WolfPelt", ["onions"] = "Onion",
        ["seeds"] = "OnionSeeds", ["frost_mead"] = "MeadFrostResist",
        ["barley"] = "Barley", ["flour"] = "BarleyFlour", ["flax"] = "Flax",
        ["cloudberries"] = "Cloudberry", ["blackmetal"] = "BlackMetal",
        ["coins"] = "Coins", ["lox_meat"] = "LoxMeat",
        ["puffs"] = "MushroomJotunPuffs", ["softtissue"] = "Softtissue",
        ["yggwood"] = "YggdrasilWood", ["marble"] = "BlackMarble", ["sap"] = "Sap",
        ["magecaps"] = "MushroomMagecap", ["eitr"] = "Eitr", ["jelly"] = "RoyalJelly",
        ["grausten"] = "Grausten", ["ashwood"] = "Blackwood", ["flametal"] = "FlametalNew",
        ["smoke"] = "BombSmoke", ["fortification"] = "Grausten",
        ["spicy_food"] = "MorgenHeart", ["fire_medicine"] = "MeadFireResist"
    };

    internal static Sprite? IconFor(string id, string displayName)
    {
        if (Icons.TryGetValue(id, out var cached)) return cached;
        var prefab = ResourcePrefab(id);
        var sprites = prefab?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_icons;
        if (sprites != null && sprites.Length > 0 && sprites[0] != null)
            return Icons[id] = sprites[0];

        Plugin.Log.LogWarning($"Cargo icon source missing for {id}; keeping cloned base icon.");
        return null;
    }

    private static GameObject? ResourcePrefab(string id)
    {
        // Longest suffix wins: e.g. hrothgar_wolf_meat must resolve wolf_meat,
        // not the generic meat mapping.
        string? best = null;
        foreach (var key in Resources.Keys)
            if (id.EndsWith("_" + key, StringComparison.Ordinal) &&
                (best == null || key.Length > best.Length))
                best = key;
        return best == null ? null : PrefabManager.Instance.GetPrefab(Resources[best]);
    }

    internal static void AttachWorldCrate(GameObject item, string id)
    {
        var source = ResourcePrefab(id);
        if (source == null)
        {
            Plugin.Log.LogWarning($"Cargo world model source missing for {id}; keeping base model.");
            return;
        }

        if (item.GetComponent<Rigidbody>() == null)
        {
            var body = item.AddComponent<Rigidbody>();
            body.mass = 8f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // Hide only the cloned base prefab renderers. The new native visual is
        // added afterwards, so it cannot accidentally be disabled.
        foreach (var renderer in item.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = false;

        var visualRoot = new GameObject("ImmersiveTrader_CargoVisual");
        visualRoot.transform.SetParent(item.transform, false);
        visualRoot.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        visualRoot.transform.localRotation = Quaternion.Euler(0f, 25f, 0f);

        int copied = 0;
        foreach (var renderer in source.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer is not MeshRenderer meshRenderer) continue;
            var filter = renderer.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) continue;

            var child = new GameObject("NativeCargoMesh");
            child.transform.SetParent(visualRoot.transform, false);
            child.transform.localPosition = source.transform.InverseTransformPoint(renderer.transform.position);
            child.transform.localRotation = Quaternion.Inverse(source.transform.rotation) * renderer.transform.rotation;
            child.transform.localScale = renderer.transform.lossyScale;
            child.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
            child.AddComponent<MeshRenderer>().sharedMaterials = meshRenderer.sharedMaterials;
            copied++;
        }

        if (copied == 0)
        {
            foreach (var renderer in item.GetComponentsInChildren<Renderer>(true))
                renderer.enabled = true;
            UnityEngine.Object.Destroy(visualRoot);
            Plugin.Log.LogWarning($"Cargo native mesh unavailable for {id}; keeping base model.");
            return;
        }

        if (item.GetComponentInChildren<Collider>(true) == null)
        {
            var collider = item.AddComponent<BoxCollider>();
            collider.size = new Vector3(.5f, .35f, .5f);
            collider.center = new Vector3(0f, .18f, 0f);
        }
    }
}
