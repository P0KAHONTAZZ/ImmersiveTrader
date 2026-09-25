using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Physical rolled contract scroll. The UI icon stays the approved 2D art,
/// while the dropped item is a small parchment cylinder with visible end caps.
/// </summary>
internal static class ContractWorldModel
{
    internal static void Attach(GameObject prefab, Sprite? parchment)
    {
        if (prefab.GetComponent<Rigidbody>() == null)
        {
            var body = prefab.AddComponent<Rigidbody>();
            body.mass = 0.1f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }

        foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = false;

        var root = new GameObject("ImmersiveTrader_ContractScrollWorld");
        root.transform.SetParent(prefab.transform, false);
        root.transform.localPosition = new Vector3(0f, .13f, 0f);
        root.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

        var shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) { Plugin.Log.LogWarning("Contract scroll shader unavailable."); return; }

        // Warm parchment material. If the approved icon exists, use it as a
        // subtle texture instead of showing the square atlas cell as a flat card.
        var paper = new Material(shader) { name = "ImmersiveTrader_ContractPaper" };
        paper.color = new Color(.72f, .53f, .28f, 1f);
        if (parchment != null) paper.mainTexture = parchment.texture;

        var edge = new Material(shader) { name = "ImmersiveTrader_ContractEdge" };
        edge.color = new Color(.34f, .17f, .07f, 1f);

        // Main rolled parchment.
        var roll = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        roll.name = "ContractRoll";
        roll.transform.SetParent(root.transform, false);
        roll.transform.localScale = new Vector3(.105f, .29f, .105f);
        var rollRenderer = roll.GetComponent<Renderer>();
        if (rollRenderer != null) rollRenderer.sharedMaterial = paper;
        var primitiveCollider = roll.GetComponent<Collider>();
        if (primitiveCollider != null) Object.Destroy(primitiveCollider);

        // Darker wooden/parchment caps make the object readable in grass/snow.
        foreach (float y in new[] { -.305f, .305f })
        {
            var cap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cap.name = "ContractRollBand";
            cap.transform.SetParent(root.transform, false);
            cap.transform.localPosition = new Vector3(0f, y, 0f);
            cap.transform.localScale = new Vector3(.125f, .018f, .125f);
            var renderer = cap.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = edge;
            var collider = cap.GetComponent<Collider>();
            if (collider != null) Object.Destroy(collider);
        }

        // Small wax/seal-like knot in the middle.
        var seal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        seal.name = "ContractSeal";
        seal.transform.SetParent(root.transform, false);
        seal.transform.localPosition = new Vector3(0f, 0f, -.105f);
        seal.transform.localScale = new Vector3(.095f, .06f, .035f);
        var sealRenderer = seal.GetComponent<Renderer>();
        if (sealRenderer != null)
        {
            var sealMaterial = new Material(shader) { name = "ImmersiveTrader_ContractSealMaterial" };
            sealMaterial.color = new Color(.42f, .06f, .035f, 1f);
            sealRenderer.sharedMaterial = sealMaterial;
        }
        var sealCollider = seal.GetComponent<Collider>();
        if (sealCollider != null) Object.Destroy(sealCollider);

        // Replace the broad flat-sheet collider with a compact scroll collider.
        foreach (var collider in prefab.GetComponents<Collider>())
            Object.Destroy(collider);
        var box = prefab.AddComponent<BoxCollider>();
        box.size = new Vector3(.62f, .24f, .24f);
        box.center = new Vector3(0f, .13f, 0f);
    }
}
