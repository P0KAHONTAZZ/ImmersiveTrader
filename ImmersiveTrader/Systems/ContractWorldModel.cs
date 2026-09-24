using UnityEngine;

namespace ImmersiveTrader;

/// <summary>Displays the same parchment as a small physical sheet when a contract is dropped.</summary>
internal static class ContractWorldModel
{
    internal static void Attach(GameObject prefab, Sprite? parchment)
    {
        // Humanoid.DropItem expects a Rigidbody on the root of the spawned item.
        // Coins in this Valheim build provides the ItemDrop and ZNetView but no root body.
        if (prefab.GetComponent<Rigidbody>() == null)
        {
            var body = prefab.AddComponent<Rigidbody>();
            body.mass = 0.1f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }
        if (prefab.GetComponentInChildren<Collider>(true) == null)
        {
            var collider = prefab.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.42f, 0.05f, 0.42f);
        }
        if (parchment == null) return;
        var shader = Shader.Find("Sprites/Default");
        if (shader == null) { Plugin.Log.LogWarning("Contract sheet shader unavailable."); return; }

        // Keep the vanilla Coins physics and networking, replacing only its visuals.
        foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = false;

        var sheet = new GameObject("ImmersiveTrader_ContractSheet");
        sheet.transform.SetParent(prefab.transform, false);
        sheet.transform.localPosition = new Vector3(0f, 0.06f, 0f);
        var mesh = new Mesh { name = "ImmersiveTrader_ContractSheet" };
        mesh.vertices = new[] {
            new Vector3(-0.22f, 0f, -0.22f), new Vector3(0.22f, 0f, -0.22f),
            new Vector3(0.22f, 0f, 0.22f), new Vector3(-0.22f, 0f, 0.22f)
        };
        mesh.uv = new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        mesh.triangles = new[] { 0, 2, 1, 0, 3, 2, 0, 1, 2, 0, 2, 3 };
        mesh.RecalculateNormals();
        sheet.AddComponent<MeshFilter>().sharedMesh = mesh;
        var material = new Material(shader) { name = "ImmersiveTrader_ContractParchment", mainTexture = parchment.texture };
        sheet.AddComponent<MeshRenderer>().sharedMaterial = material;
    }
}
