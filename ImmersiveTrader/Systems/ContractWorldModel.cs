using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// One shared ground model for all hunting contracts: a rolled parchment tied with a red
/// ribbon and a wax seal, with a slightly unrolled flap and darker rolled ends.
/// Visual only (built on the cloned prefab); contract logic is untouched.
/// Deliberately does not glow, so contracts read differently from enhancement scrolls.
/// </summary>
internal static class ContractWorldModel
{
    private static Shader? shader;
    private static Material? paper, paperEnd, ribbon, wax, waxRim;

    internal static void Attach(GameObject prefab, Sprite? parchment)
    {
        foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = false;

        shader ??= Shader.Find("Standard") ?? Shader.Find("Sprites/Default");
        if (shader == null)
        {
            Plugin.Log.LogWarning("Contract scroll shader unavailable; keeping base model.");
            foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true)) renderer.enabled = true;
            return;
        }

        paper ??= Mat("ImmersiveTrader_ContractPaper", new Color(0.86f, 0.75f, 0.53f), 0.1f);
        paperEnd ??= Mat("ImmersiveTrader_ContractPaperEnd", new Color(0.60f, 0.47f, 0.29f), 0.05f);
        ribbon ??= Mat("ImmersiveTrader_ContractRibbon", new Color(0.52f, 0.07f, 0.06f), 0.35f);
        wax ??= Mat("ImmersiveTrader_ContractWax", new Color(0.62f, 0.06f, 0.04f), 0.6f);
        waxRim ??= Mat("ImmersiveTrader_ContractWaxRim", new Color(0.40f, 0.03f, 0.02f), 0.5f);

        const float r = 0.055f;      // roll radius
        const float len = 0.46f;     // roll length along X
        var root = new GameObject("ImmersiveTrader_ContractScrollWorld");
        root.transform.SetParent(prefab.transform, false);
        root.transform.localPosition = new Vector3(0f, r, 0f);

        // Rolled parchment (cylinder axis along X).
        Part(root, PrimitiveType.Cylinder, "ContractRoll", Vector3.zero, new Vector3(0f, 0f, 90f),
             new Vector3(r * 2f, len * 0.5f, r * 2f), paper);
        // Darker, slightly inset rolled ends so the spiral edge reads as rolled paper.
        foreach (float x in new[] { -len * 0.5f, len * 0.5f })
            Part(root, PrimitiveType.Cylinder, "ContractRollEnd", new Vector3(x, 0f, 0f), new Vector3(0f, 0f, 90f),
                 new Vector3(r * 1.7f, 0.004f, r * 1.7f), paperEnd);

        // Unrolled flap lying on the ground next to the roll.
        Part(root, PrimitiveType.Cube, "ContractFlap", new Vector3(0f, -r + 0.004f, r + 0.045f), new Vector3(-6f, 0f, 0f),
             new Vector3(len * 0.96f, 0.004f, 0.10f), paper);

        // Ribbon wrapped around the middle (slightly larger ring) with two hanging tails.
        Part(root, PrimitiveType.Cylinder, "ContractRibbon", Vector3.zero, new Vector3(0f, 0f, 90f),
             new Vector3(r * 2.12f, 0.022f, r * 2.12f), ribbon);
        Part(root, PrimitiveType.Cube, "ContractRibbonTailA", new Vector3(-0.025f, -r + 0.006f, -r - 0.05f),
             new Vector3(0f, 18f, 0f), new Vector3(0.028f, 0.004f, 0.11f), ribbon);
        Part(root, PrimitiveType.Cube, "ContractRibbonTailB", new Vector3(0.028f, -r + 0.006f, -r - 0.045f),
             new Vector3(0f, -14f, 0f), new Vector3(0.028f, 0.004f, 0.10f), ribbon);

        // Wax seal on top of the ribbon, facing up.
        Part(root, PrimitiveType.Cylinder, "ContractSealRim", new Vector3(0f, r + 0.006f, 0f), Vector3.zero,
             new Vector3(0.062f, 0.006f, 0.062f), waxRim);
        Part(root, PrimitiveType.Cylinder, "ContractSeal", new Vector3(0f, r + 0.011f, 0f), Vector3.zero,
             new Vector3(0.048f, 0.005f, 0.048f), wax);
        Part(root, PrimitiveType.Sphere, "ContractSealDrop", new Vector3(0.028f, r + 0.004f, 0.022f), Vector3.zero,
             new Vector3(0.018f, 0.008f, 0.018f), wax);

        WorldItemPhysics.Setup(prefab, new Vector3(len + 0.02f, r * 2f + 0.02f, r * 2f + 0.20f), 0f,
            "ImmersiveTrader_ContractScrollWorld", -r);
    }

    private static Material Mat(string name, Color color, float smoothness)
    {
        var m = new Material(shader) { name = name, color = color };
        if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", smoothness);
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", 0f);
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
        if (collider != null) Object.Destroy(collider);
    }
}
