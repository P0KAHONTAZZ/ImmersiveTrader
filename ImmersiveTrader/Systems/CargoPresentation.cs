using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Jotunn.Managers;

namespace ImmersiveTrader;

/// <summary>Custom cargo icons + Hildir chest world models.</summary>
internal static class CargoPresentation
{
    private static readonly Dictionary<string, Sprite> Icons = new();
    private static Texture2D? finalCargo;

    // Approved custom art. Missing cargo gets a Hildir chest + native resource badge,
    // so every one of the 70 shipments has a cargo-looking icon.
    private static readonly Dictionary<string, int> FinalCargoArt = new()
    {
        ["field_medicine"]=0, ["healing_honey"]=1, ["bandages"]=2, ["corewood"]=3,
        ["copper"]=4, ["thistle"]=5, ["entrails"]=6, ["scrap_iron"]=7,
        ["silver"]=8, ["frost_mead"]=9, ["cloudberries"]=10, ["blackmetal"]=11,
        ["sap"]=12, ["eitr"]=13, ["flametal"]=14, ["fortification"]=15
    };

    private static readonly Dictionary<string, string> Resources = new()
    {
        ["field_medicine"]="HealthPotion", ["healing_honey"]="Honey", ["bandages"]="LinenThread",
        ["herbs"]="Dandelion", ["resin"]="Resin", ["corewood"]="RoundLog", ["hides"]="TrollHide",
        ["meat"]="RawMeat", ["wood"]="Wood", ["amber"]="Amber", ["copper"]="Copper",
        ["tin"]="Tin", ["cores"]="SurtlingCore", ["arrows"]="ArrowWood", ["rubies"]="Ruby",
        ["thistle"]="Thistle", ["entrails"]="Entrails", ["scrap_iron"]="IronScrap",
        ["iron"]="Iron", ["roots"]="Root", ["bloodbags"]="Bloodbag", ["guck"]="Guck",
        ["chains"]="Chain", ["sausages"]="Sausages", ["bark"]="ElderBark", ["obsidian"]="Obsidian",
        ["silver"]="Silver", ["wolf_meat"]="WolfMeat", ["crystal"]="Crystal", ["pelts"]="WolfPelt",
        ["onions"]="Onion", ["seeds"]="OnionSeeds", ["frost_mead"]="MeadFrostResist",
        ["barley"]="Barley", ["flour"]="BarleyFlour", ["flax"]="Flax", ["cloudberries"]="Cloudberry",
        ["blackmetal"]="BlackMetal", ["coins"]="Coins", ["lox_meat"]="LoxMeat",
        ["puffs"]="MushroomJotunPuffs", ["softtissue"]="Softtissue", ["yggwood"]="YggdrasilWood",
        ["marble"]="BlackMarble", ["sap"]="Sap", ["magecaps"]="MushroomMagecap", ["eitr"]="Eitr",
        ["jelly"]="RoyalJelly", ["grausten"]="Grausten", ["ashwood"]="Blackwood",
        ["flametal"]="FlametalNew", ["smoke"]="BombSmoke", ["fortification"]="Grausten",
        ["spicy_food"]="MorgenHeart", ["fire_medicine"]="MeadFireResist"
    };

    internal static Sprite? IconFor(string id, string displayName)
    {
        if (Icons.TryGetValue(id, out var cached)) return cached;

        var approved = FinalIcon(id);
        if (approved != null) return Icons[id] = approved;

        // Complete coverage for cargo without dedicated approved artwork.
        var chest = PrefabManager.Instance.GetPrefab(HildirChestFor(id));
        var chestIcons = chest?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_icons;
        var baseIcon = chestIcons != null && chestIcons.Length > 0 ? chestIcons[0] : null;
        var resource = ResourcePrefab(id);
        var resourceIcons = resource?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_icons;
        var badge = resourceIcons != null && resourceIcons.Length > 0 ? resourceIcons[0] : null;
        if (baseIcon == null) return badge;

        const int size = 128;
        var rt = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.ARGB32);
        var previous = RenderTexture.active;
        try
        {
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear);
            GL.PushMatrix();
            try
            {
                GL.LoadPixelMatrix(0, size, size, 0);
                Draw(baseIcon, new Rect(8, 8, 112, 112));
                if (badge != null) Draw(badge, new Rect(76, 72, 46, 46));
            }
            finally { GL.PopMatrix(); }
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0,0,size,size), new Vector2(.5f,.5f));
            sprite.name = $"ImmersiveTrader_Cargo_{id}";
            return Icons[id] = sprite;
        }
        finally
        {
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);
        }
    }

    private static Sprite? FinalIcon(string id)
    {
        string? best = null; int index = -1;
        foreach (var pair in FinalCargoArt)
            if (id.EndsWith("_" + pair.Key, StringComparison.Ordinal) &&
                (best == null || pair.Key.Length > best.Length))
            { best = pair.Key; index = pair.Value; }
        if (index < 0) return null;

        finalCargo ??= LoadEmbedded("ImmersiveTrader.Assets.CargoFinalIcons.png");
        if (finalCargo == null) return null;
        const int cell = 128;
        if (finalCargo.width < (index + 1) * cell) return null;

        // Trim transparent padding inside each atlas cell, then scale it into
        // the same 112x112 visual box. This fixes the uneven crop/size.
        int x0=index*cell, minX=cell, minY=cell, maxX=-1, maxY=-1;
        for(int y=0;y<cell;y++) for(int x=0;x<cell;x++)
            if(finalCargo.GetPixel(x0+x,y).a > .08f)
            { minX=Math.Min(minX,x); minY=Math.Min(minY,y); maxX=Math.Max(maxX,x); maxY=Math.Max(maxY,y); }
        if(maxX < minX) return null;

        int w=maxX-minX+1, h=maxY-minY+1;
        var src=Sprite.Create(finalCargo,new Rect(x0+minX,minY,w,h),new Vector2(.5f,.5f));
        const int size=128;
        var rt=RenderTexture.GetTemporary(size,size,0,RenderTextureFormat.ARGB32);
        var previous=RenderTexture.active;
        try
        {
            RenderTexture.active=rt; GL.Clear(true,true,Color.clear); GL.PushMatrix();
            try
            {
                GL.LoadPixelMatrix(0,size,size,0);
                float scale=Math.Min(112f/w,112f/h), dw=w*scale, dh=h*scale;
                Draw(src,new Rect((size-dw)/2f,(size-dh)/2f,dw,dh));
            }
            finally { GL.PopMatrix(); }
            var tex=new Texture2D(size,size,TextureFormat.RGBA32,false);
            tex.ReadPixels(new Rect(0,0,size,size),0,0); tex.Apply();
            var result=Sprite.Create(tex,new Rect(0,0,size,size),new Vector2(.5f,.5f));
            result.name=$"ImmersiveTrader_FinalCargo_{id}";
            return result;
        }
        finally { RenderTexture.active=previous; RenderTexture.ReleaseTemporary(rt); }
    }

    private static Texture2D? LoadEmbedded(string resource)
    {
        using var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
        if(stream==null) return null;
        using var memory=new MemoryStream(); stream.CopyTo(memory);
        var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);
        if(!texture.LoadImage(memory.ToArray())) return null;
        texture.filterMode=FilterMode.Bilinear;
        return texture;
    }

    private static GameObject? ResourcePrefab(string id)
    {
        string? best=null;
        foreach(var key in Resources.Keys)
            if(id.EndsWith("_"+key,StringComparison.Ordinal) && (best==null || key.Length>best.Length))
                best=key;
        return best==null ? null : PrefabManager.Instance.GetPrefab(Resources[best]);
    }

    // Rotate the three official quest-chest models across cargo tiers.
    private static string HildirChestFor(string id)
    {
        if(id.StartsWith("midka_") || id.StartsWith("troldad_") || id.StartsWith("grimvald_") || id.StartsWith("rudy_"))
            return "chest_hildir1"; // brass
        if(id.StartsWith("mokra_") || id.StartsWith("encek_") || id.StartsWith("hrothgar_") || id.StartsWith("ylva_"))
            return "chest_hildir2"; // silver
        return "chest_hildir3"; // bronze
    }

    private static void Draw(Sprite sprite, Rect destination)
    {
        var source=sprite.textureRect; var texture=sprite.texture;
        Graphics.DrawTexture(destination,texture,
            new Rect(source.x/texture.width,source.y/texture.height,source.width/texture.width,source.height/texture.height),
            0,0,0,0);
    }

    internal static void AttachWorldCrate(GameObject item, string id)
    {
        var chest=PrefabManager.Instance.GetPrefab(HildirChestFor(id));
        if(chest==null) { Plugin.Log.LogWarning($"Hildir cargo model missing for {id}"); return; }

        if(item.GetComponent<Rigidbody>()==null)
        {
            var body=item.AddComponent<Rigidbody>();
            body.mass=8f; body.interpolation=RigidbodyInterpolation.Interpolate;
        }

        foreach(var renderer in item.GetComponentsInChildren<Renderer>(true)) renderer.enabled=false;

        var root=new GameObject("ImmersiveTrader_HildirCargo");
        root.transform.SetParent(item.transform,false);
        root.transform.localPosition=Vector3.zero;

        int copied=0;
        foreach(var renderer in chest.GetComponentsInChildren<MeshRenderer>(true))
        {
            var filter=renderer.GetComponent<MeshFilter>();
            if(filter==null || filter.sharedMesh==null) continue;
            var child=new GameObject("HildirChestMesh");
            child.transform.SetParent(root.transform,false);
            child.transform.localPosition=chest.transform.InverseTransformPoint(renderer.transform.position);
            child.transform.localRotation=Quaternion.Inverse(chest.transform.rotation)*renderer.transform.rotation;
            child.transform.localScale=renderer.transform.lossyScale;
            child.AddComponent<MeshFilter>().sharedMesh=filter.sharedMesh;
            child.AddComponent<MeshRenderer>().sharedMaterials=renderer.sharedMaterials;
            copied++;
        }

        if(copied==0)
        {
            foreach(var renderer in item.GetComponentsInChildren<Renderer>(true)) renderer.enabled=true;
            UnityEngine.Object.Destroy(root);
            Plugin.Log.LogWarning($"Hildir cargo mesh unavailable for {id}");
        }
    }
}
