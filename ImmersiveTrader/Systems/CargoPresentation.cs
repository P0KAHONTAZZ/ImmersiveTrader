using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Jotunn.Managers;

namespace ImmersiveTrader;

/// <summary>Vanilla chest silhouette and resource labels for the physical cargo.</summary>
internal static class CargoPresentation
{
    private static readonly Dictionary<string, Sprite> Icons = new();
    private static Texture2D? packages;
    private static readonly Dictionary<string, string> Resources = new()
    {
        ["medicine"] = "HealthPotion", ["honey"] = "Honey", ["bandages"] = "LinenThread",
        ["herbs"] = "Dandelion", ["resin"] = "Resin", ["corewood"] = "RoundLog",
        ["hides"] = "TrollHide", ["meat"] = "RawMeat", ["wood"] = "Wood",
        ["amber"] = "Amber", ["copper"] = "Copper", ["tin"] = "Tin",
        ["cores"] = "SurtlingCore", ["arrows"] = "ArrowWood", ["rubies"] = "Ruby",
        ["thistle"] = "Thistle", ["entrails"] = "Entrails", ["iron"] = "Iron",
        ["roots"] = "Root", ["bloodbags"] = "Bloodbag", ["guck"] = "Guck",
        ["chains"] = "Chain", ["sausages"] = "Sausages", ["bark"] = "ElderBark",
        ["obsidian"] = "Obsidian", ["silver"] = "Silver", ["crystal"] = "Crystal",
        ["pelts"] = "WolfPelt", ["onions"] = "Onion", ["seeds"] = "OnionSeeds",
        ["mead"] = "MeadFrostResist", ["barley"] = "Barley", ["flour"] = "BarleyFlour",
        ["flax"] = "Flax", ["cloudberries"] = "Cloudberry", ["blackmetal"] = "BlackMetal",
        ["coins"] = "Coins", ["puffs"] = "MushroomJotunPuffs", ["softtissue"] = "Softtissue",
        ["yggwood"] = "YggdrasilWood", ["marble"] = "BlackMarble", ["sap"] = "Sap",
        ["magecaps"] = "MushroomMagecap", ["eitr"] = "Eitr", ["jelly"] = "RoyalJelly",
        ["grausten"] = "Grausten", ["ashwood"] = "Blackwood", ["flametal"] = "FlametalNew",
        ["smoke"] = "BombSmoke", ["fortification"] = "Grausten",
        ["spicy_food"] = "MorgenHeart", ["fire_medicine"] = "MeadFireResist"
    };

    internal static Sprite? IconFor(string id, string displayName)
    {
        if (Icons.TryGetValue(id, out var cached)) return cached;
        packages ??= LoadPackages();
        if (packages == null) return null;
        int shape = displayName.StartsWith("Beczka", StringComparison.Ordinal) ? 1 :
            displayName.StartsWith("Worek", StringComparison.Ordinal) ? 2 :
            displayName.StartsWith("Paczka", StringComparison.Ordinal) || displayName.StartsWith("Wiązka", StringComparison.Ordinal) ? 3 :
            displayName.StartsWith("Kosz", StringComparison.Ordinal) ? 4 : 0;
        Sprite? label = null;
        foreach (var pair in Resources)
        {
            if (!id.EndsWith("_" + pair.Key, StringComparison.Ordinal)) continue;
            var sprites = PrefabManager.Instance.GetPrefab(pair.Value)?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_icons;
            label = sprites != null && sprites.Length > 0 ? sprites[0] : null;
            break;
        }
        const int size = 128;
        var target = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.ARGB32);
        var previous = RenderTexture.active;
        try
        {
            RenderTexture.active = target;
            GL.Clear(true, true, Color.clear);
            GL.PushMatrix();
            try
            {
                GL.LoadPixelMatrix(0, size, size, 0);
                Graphics.DrawTexture(new Rect(6, 3, 116, 122), packages,
                    new Rect(shape / 5f, 0f, 1f / 5f, 1f), 0, 0, 0, 0);
                if (label != null) Draw(label, new Rect(48, 54, 34, 34));
            }
            finally { GL.PopMatrix(); }
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            texture.Apply();
            return Icons[id] = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(.5f, .5f));
        }
        catch (Exception error)
        {
            Plugin.Log.LogWarning($"Cargo icon {id}: {error.Message}");
            return null;
        }
        finally
        {
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(target);
        }
    }

    private static Texture2D? LoadPackages()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ImmersiveTrader.Assets.CargoPackages.png");
        if (stream == null) { Plugin.Log.LogWarning("Cargo package art missing from DLL."); return null; }
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(memory.ToArray())) return null;
        texture.filterMode = FilterMode.Bilinear;
        return texture;
    }

    private static void Draw(Sprite sprite, Rect destination)
    {
        var source = sprite.textureRect;
        var texture = sprite.texture;
        Graphics.DrawTexture(destination, texture,
            new Rect(source.x / texture.width, source.y / texture.height,
                source.width / texture.width, source.height / texture.height), 0, 0, 0, 0);
    }

    internal static void AttachWorldCrate(GameObject item)
    {
        var chest = PrefabManager.Instance.GetPrefab("piece_chest_wood");
        if (chest == null) { Plugin.Log.LogWarning("Cargo chest model missing: piece_chest_wood"); return; }
        if (item.GetComponent<Rigidbody>() == null)
        {
            var body = item.AddComponent<Rigidbody>();
            body.mass = 8f;
        }
        if (item.GetComponentInChildren<Collider>(true) == null)
        {
            var collider = item.AddComponent<BoxCollider>();
            collider.size = new Vector3(.55f, .4f, .42f);
            collider.center = new Vector3(0f, .2f, 0f);
        }

        int copied = 0;
        foreach (var renderer in chest.GetComponentsInChildren<MeshRenderer>(true))
        {
            var filter = renderer.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) continue;
            var visual = new GameObject("ImmersiveTrader_CargoCrate");
            visual.transform.SetParent(item.transform, false);
            visual.transform.localPosition = chest.transform.InverseTransformPoint(renderer.transform.position);
            visual.transform.localRotation = Quaternion.Inverse(chest.transform.rotation) * renderer.transform.rotation;
            visual.transform.localScale = renderer.transform.lossyScale;
            visual.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
            visual.AddComponent<MeshRenderer>().sharedMaterials = renderer.sharedMaterials;
            copied++;
        }
        if (copied == 0) return;
        foreach (var renderer in item.GetComponentsInChildren<Renderer>(true))
            if (renderer.gameObject.name != "ImmersiveTrader_CargoCrate") renderer.enabled = false;
    }
}
