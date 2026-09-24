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
    private static readonly Dictionary<Skills.SkillType, Sprite> icons = new();
    private const int Size = 128;

    internal static Sprite? Parchment => parchment ??= LoadParchment();

    internal static Sprite? ForSkill(Skills.SkillType skill)
    {
        if (icons.TryGetValue(skill, out var cached)) return cached;
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
