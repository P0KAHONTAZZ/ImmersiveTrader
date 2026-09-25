using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>Combines the contract parchment with the actual skill icon from the running game.</summary>
internal static class ContractIconRegistry
{
    private static Sprite? parchment;
    private static Texture2D? finalSheet;
    private static readonly Dictionary<Skills.SkillType, Sprite> icons = new();
    private static readonly Dictionary<Skills.SkillType, int> FinalArt = new()
    {
        [Skills.SkillType.Axes]=0, [Skills.SkillType.Blocking]=1, [Skills.SkillType.Bows]=2,
        [Skills.SkillType.Clubs]=3, [Skills.SkillType.Knives]=4, [Skills.SkillType.Polearms]=5,
        [Skills.SkillType.Swords]=6, [Skills.SkillType.Unarmed]=7, [Skills.SkillType.WoodCutting]=8,
        [Skills.SkillType.Pickaxes]=9, [Skills.SkillType.Spears]=10, [Skills.SkillType.Crossbows]=11,
        [Skills.SkillType.Run]=12, [Skills.SkillType.Swim]=13, [Skills.SkillType.Jump]=14,
        [Skills.SkillType.Sneak]=15, [Skills.SkillType.Fishing]=16, [Skills.SkillType.Cooking]=17,
        [Skills.SkillType.Farming]=18, [Skills.SkillType.Crafting]=19, [Skills.SkillType.BloodMagic]=20,
        [Skills.SkillType.ElementalMagic]=21
    };
    private const int Size = 128;

    internal static Sprite? Parchment => parchment ??= LoadParchment();

    internal static Sprite? ForSkill(Skills.SkillType skill)
    {
        if (icons.TryGetValue(skill, out var cached)) return cached;
        // The old approved strip was exported as a presentation strip, not a true
        // fixed-cell atlas. Reading it by cell caused the vertical fragments seen
        // in inventory. Build every contract icon from the full parchment plus
        // Valheim's native skill symbol instead: consistent for all 24 skills.
        var baseSprite = Parchment;
        var player = Player.m_localPlayer;
        if (baseSprite == null || player == null) return baseSprite;

        // Resolve the definition at runtime so the symbol matches this Valheim build.
        var skills = AccessTools.Method(typeof(Player), "GetSkills")?.Invoke(player, null);
        var definition = skills == null ? null : AccessTools.Method(typeof(Skills), "GetSkillDef")?.Invoke(skills, new object[] { skill });
        var gameIcon = definition == null ? null : AccessTools.Field(definition.GetType(), "m_icon")?.GetValue(definition) as Sprite;
        if (gameIcon == null) return baseSprite;

        try
        {
            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            var previous = RenderTexture.active;
            var surface = RenderTexture.GetTemporary(Size, Size, 0, RenderTextureFormat.ARGB32);
            try
            {
                RenderTexture.active = surface;
                GL.Clear(true, true, Color.clear);
                GL.PushMatrix();
                try
                {
                    GL.LoadPixelMatrix(0, Size, Size, 0);
                    DrawSprite(baseSprite, new Rect(0, 0, Size, Size));
                    DrawSprite(gameIcon, new Rect(35, 34, 58, 58));
                }
                finally { GL.PopMatrix(); }
                texture.ReadPixels(new Rect(0, 0, Size, Size), 0, 0);
                texture.Apply();
            }
            finally
            {
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(surface);
            }
            var sprite = Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f));
            sprite.name = $"ImmersiveTrader_Contract_{skill}";
            icons[skill] = sprite;
            return sprite;
        }
        catch (Exception error)
        {
            Plugin.Log.LogWarning($"Could not render contract skill icon {skill}: {error.Message}");
            return baseSprite;
        }
    }

    private static Sprite? LoadFinalSkillIcon(Skills.SkillType skill)
    {
        if (!FinalArt.TryGetValue(skill, out int index)) return null; // Ride/Dodge use the runtime Valheim-skill fallback until dedicated approved art exists.
        finalSheet ??= LoadTexture("ImmersiveTrader.Assets.ContractSkillIcons.png");
        if (finalSheet == null) return null;
        const int cell = 128;
        if (finalSheet.width < (index + 1) * cell || finalSheet.height < cell) return null;

        // Normalize every atlas cell: trim transparent margins and center the
        // artwork in the same 112x112 visual box. This removes the uneven
        // sizing/cropping visible in inventory and trader UI.
        int x0 = index * cell, minX = cell, minY = cell, maxX = -1, maxY = -1;
        for (int y = 0; y < cell; y++)
            for (int x = 0; x < cell; x++)
                if (finalSheet.GetPixel(x0 + x, y).a > .08f)
                {
                    minX = Math.Min(minX, x); minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y);
                }
        if (maxX < minX) return null;

        int w = maxX - minX + 1, h = maxY - minY + 1;
        var source = Sprite.Create(finalSheet, new Rect(x0 + minX, minY, w, h), new Vector2(.5f, .5f));
        var surface = RenderTexture.GetTemporary(Size, Size, 0, RenderTextureFormat.ARGB32);
        var previous = RenderTexture.active;
        try
        {
            RenderTexture.active = surface;
            GL.Clear(true, true, Color.clear);
            GL.PushMatrix();
            try
            {
                GL.LoadPixelMatrix(0, Size, Size, 0);
                float scale = Math.Min(112f / w, 112f / h);
                float dw = w * scale, dh = h * scale;
                DrawSprite(source, new Rect((Size - dw) / 2f, (Size - dh) / 2f, dw, dh));
            }
            finally { GL.PopMatrix(); }
            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, Size, Size), 0, 0);
            texture.Apply();
            var sprite = Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(.5f, .5f));
            sprite.name = $"ImmersiveTrader_FinalContract_{skill}";
            return sprite;
        }
        finally
        {
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(surface);
        }
    }

    private static Texture2D? LoadTexture(string resource)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
        if (stream == null) return null;
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(memory.ToArray())) return null;
        texture.filterMode = FilterMode.Bilinear;
        return texture;
    }

    private static void DrawSprite(Sprite sprite, Rect destination)
    {
        var source = sprite.textureRect;
        var texture = sprite.texture;
        var uv = new Rect(source.x / texture.width, source.y / texture.height,
            source.width / texture.width, source.height / texture.height);
        Graphics.DrawTexture(destination, texture, uv, 0, 0, 0, 0);
    }

    private static Sprite? LoadParchment()
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ImmersiveTrader.Assets.ContractScroll.png");
        if (stream == null) { Plugin.Log.LogWarning("Contract parchment resource missing."); return null; }
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(memory.ToArray())) return null;
        texture.filterMode = FilterMode.Bilinear;
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        sprite.name = "ImmersiveTrader_ContractParchment";
        return sprite;
    }
}

[HarmonyPatch]
internal static class ContractItemIconPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => typeof(ItemDrop.ItemData)
        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(method => method.Name == "GetIcon" && method.ReturnType == typeof(Sprite));

    private static void Postfix(ItemDrop.ItemData __instance, ref Sprite __result)
    {
        if (!ContractMetadata.TryRead(__instance, out string id, out string issuer, out _)) return;
        var contract = TraderActivityRegistry.Activities.FirstOrDefault(x => x.Id == id && x.TraderId == issuer);
        if (contract != null)
            __result = ContractIconRegistry.ForSkill(ContractMetadata.GetRewardSkill(__instance, id, contract.RewardSkill)) ?? __result;
    }
}
