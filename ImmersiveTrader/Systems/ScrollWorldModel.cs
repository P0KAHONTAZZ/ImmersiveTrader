using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// One shared ground model for all 16 enhancement scrolls (contracts keep ContractWorldModel).
/// An open parchment sheet between two rollers with gold bands and wooden knobs.
/// Only the sheet/roller colour differs per scroll (taken from its inventory icon) so scrolls
/// are recognisable on the ground. Built from primitives on the cloned prefab only.
/// </summary>
internal static class ScrollWorldModel
{
    private static readonly Dictionary<string, Color> PaperColor = new(StringComparer.OrdinalIgnoreCase)
    {
        ["embers"] = new Color(0.58f, 0.17f, 0.15f),
        ["frost"] = new Color(0.10f, 0.38f, 0.58f),
        ["storm"] = new Color(0.67f, 0.43f, 0.11f),
        ["venom"] = new Color(0.19f, 0.40f, 0.16f),
        ["spirit"] = new Color(0.40f, 0.15f, 0.50f),
        ["lumberjack"] = new Color(0.53f, 0.30f, 0.15f),
        ["miner"] = new Color(0.26f, 0.30f, 0.33f),
        ["burden"] = new Color(0.59f, 0.36f, 0.19f),
        ["vitality"] = new Color(0.55f, 0.15f, 0.19f),
        ["endurance"] = new Color(0.28f, 0.45f, 0.15f),
        ["focus"] = new Color(0.42f, 0.15f, 0.41f),
        ["craftsman"] = new Color(0.16f, 0.30f, 0.43f),
        ["wanderer"] = new Color(0.15f, 0.34f, 0.50f),
        ["pathfinder"] = new Color(0.27f, 0.45f, 0.15f),
        ["hunter"] = new Color(0.62f, 0.18f, 0.18f),
        ["rested"] = new Color(0.69f, 0.09f, 0.11f),
    };

    private const float Scale = 1.3f;
    private static Shader? shader;
    private static Material? gold, wood;

    internal static void Attach(GameObject prefab, string id)
    {
        foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = false;

        shader ??= Shader.Find("Standard") ?? Shader.Find("Sprites/Default");
        if (shader == null) { Plugin.Log.LogWarning("Scroll world model: shader unavailable; keeping base model."); Restore(prefab); return; }

        var paperColor = PaperColor.TryGetValue(id, out var c) ? c : new Color(0.6f, 0.2f, 0.15f);
        var glowColor = GlowColor(paperColor);
        // Paper glows faintly by itself: always visible, even when the engine drops point
        // lights beyond its per-pixel light limit.
        var paper = Mat("ImmersiveTrader_ScrollPaper_" + id, paperColor, 0.15f);
        Emit(paper, glowColor * 0.45f);
        var roller = Mat("ImmersiveTrader_ScrollRoller_" + id, paperColor * 0.72f + new Color(0, 0, 0, 0.28f), 0.25f);
        gold ??= Mat("ImmersiveTrader_ScrollGold", new Color(0.83f, 0.62f, 0.20f), 0.65f, 0.7f);
        wood ??= Mat("ImmersiveTrader_ScrollWood", new Color(0.23f, 0.13f, 0.07f), 0.1f);

        var root = new GameObject("ImmersiveTrader_ScrollWorld");
        root.transform.SetParent(prefab.transform, false);
        root.transform.localPosition = new Vector3(0f, 0.045f, 0f);
        root.transform.localScale = Vector3.one * Scale;

        // Open sheet lying between the rollers (x = width, z = length between rollers).
        Part(root, PrimitiveType.Cube, "ScrollSheet", new Vector3(0f, 0.012f, 0f), Vector3.zero,
             new Vector3(0.30f, 0.006f, 0.20f), paper);

        // Two rollers along X at both ends of the sheet.
        foreach (float z in new[] { -0.11f, 0.11f })
        {
            // Cylinder primitive is Y-up (height 2 units); rotate so its axis runs along X.
            Part(root, PrimitiveType.Cylinder, "ScrollRoller", new Vector3(0f, 0.035f, z), new Vector3(0f, 0f, 90f),
                 new Vector3(0.07f, 0.17f, 0.07f), roller);
            foreach (float x in new[] { -0.105f, 0.105f })
                Part(root, PrimitiveType.Cylinder, "ScrollBand", new Vector3(x, 0.035f, z), new Vector3(0f, 0f, 90f),
                     new Vector3(0.077f, 0.012f, 0.077f), gold);
            foreach (float x in new[] { -0.185f, 0.185f })
                Part(root, PrimitiveType.Cylinder, "ScrollKnob", new Vector3(x, 0.035f, z), new Vector3(0f, 0f, 90f),
                     new Vector3(0.05f, 0.018f, 0.05f), wood);
        }

        // Glow in the scroll's colour: emissive emblem + small pulsing point light.
        var emblem = Mat("ImmersiveTrader_ScrollEmblem_" + id, new Color(0.83f, 0.62f, 0.20f), 0.65f, 0.7f);
        Emit(emblem, glowColor * 2.5f);
        Part(root, PrimitiveType.Cylinder, "ScrollEmblem", new Vector3(0f, 0.017f, 0f), Vector3.zero,
             new Vector3(0.075f, 0.002f, 0.075f), emblem);

        var lightGo = new GameObject("ScrollGlowLight");
        lightGo.transform.SetParent(root.transform, false);
        lightGo.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = glowColor;
        light.range = 3.2f;
        // Blue/purple read darker to the eye: compensate by perceived luminance.
        float luma = 0.2126f * glowColor.r + 0.7152f * glowColor.g + 0.0722f * glowColor.b;
        light.intensity = 2.2f * Mathf.Clamp(0.55f / Mathf.Max(0.05f, luma), 1f, 2.2f);
        light.shadows = LightShadows.None;
        light.renderMode = LightRenderMode.ForcePixel;
        lightGo.AddComponent<Components.ScrollGlowPulse>();

        WorldItemPhysics.Setup(prefab, new Vector3(0.42f, 0.09f, 0.30f) * Scale, 0.045f, "ImmersiveTrader_ScrollWorld", 0f);
    }

    /// <summary>Same hue as the icon paper, brighter and more saturated so it reads as light.</summary>
    private static Color GlowColor(Color paper)
    {
        Color.RGBToHSV(paper, out float h, out float sat, out _);
        return Color.HSVToRGB(h, Mathf.Clamp(sat, 0.5f, 0.8f), 1f);
    }

    private static void Emit(Material m, Color color)
    {
        if (!m.HasProperty("_EmissionColor")) return;
        m.EnableKeyword("_EMISSION");
        m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        m.SetColor("_EmissionColor", color);
    }

    private static Material Mat(string name, Color color, float smoothness, float metallic = 0f)
    {
        var m = new Material(shader) { name = name, color = color };
        if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", smoothness);
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
        return m;
    }

    private static void Part(GameObject root, PrimitiveType type, string name, Vector3 position, Vector3 euler,
                             Vector3 scale, Material material)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(root.transform, false);
        go.transform.localPosition = position;
        go.transform.localRotation = Quaternion.Euler(euler);
        go.transform.localScale = scale;
        var renderer = go.GetComponent<Renderer>();
        if (renderer != null) renderer.sharedMaterial = material;
        var collider = go.GetComponent<Collider>();
        if (collider != null) UnityEngine.Object.Destroy(collider);
    }

    private static void Restore(GameObject prefab)
    {
        foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = true;
    }
}
