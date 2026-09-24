using System;
using System.Linq;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>Original, self-contained houses built from vanilla Valheim materials.</summary>
public static class TraderHouseBuilder
{
    public static bool Build(string traderId, string biome, Transform parent)
    {
        var wood = MaterialOf("woodwall");
        var thatch = MaterialOf("wood_roof");
        if (wood == null || thatch == null)
        {
            Plugin.Log.LogWarning($"House materials unavailable for {traderId}; retaining the original camp.");
            return false;
        }

        bool stone = biome == "Mountains" || biome == "Mistlands" || biome == "Ashlands";
        var wall = stone ? MaterialOf("stone_wall_2x1") ?? wood : wood;
        float halfWidth = traderId == "troldad" ? 4f : 3f;
        float rear = traderId == "bjarki_goldtooth" ? 10f : 8f;
        float height = stone ? 4f : 3.4f;
        float ridge = height + (stone ? 2.3f : 1.8f);
        var house = new GameObject($"ImmersiveTrader_House_{traderId}");
        house.transform.SetParent(parent, false);

        // Complete, continuous walls; only the 2m front doorway is open.
        Box(house.transform, "floor", wood, new Vector3(0, -0.12f, rear / 2), new Vector3(halfWidth * 2, 0.24f, rear));
        Box(house.transform, "west wall", wall, new Vector3(-halfWidth, height / 2, rear / 2), new Vector3(0.23f, height, rear));
        Box(house.transform, "east wall", wall, new Vector3(halfWidth, height / 2, rear / 2), new Vector3(0.23f, height, rear));
        Box(house.transform, "back wall", wall, new Vector3(0, height / 2, rear), new Vector3(halfWidth * 2, height, 0.23f));
        float side = (halfWidth - 1f) / 2;
        Box(house.transform, "front left", wall, new Vector3(-(halfWidth + 1f) / 2, height / 2, 0), new Vector3(halfWidth - 1f, height, 0.23f));
        Box(house.transform, "front right", wall, new Vector3((halfWidth + 1f) / 2, height / 2, 0), new Vector3(halfWidth - 1f, height, 0.23f));
        Box(house.transform, "door lintel", wall, new Vector3(0, height - 0.35f, 0), new Vector3(2f, 0.7f, 0.23f));

        // Both roof halves share the same ridge and overlap the walls. A single
        // pair of meshes replaces manually rotated pieces that left visible gaps.
        var leftEave = new Vector3(-halfWidth - 0.45f, height - 0.1f, -0.45f);
        var rightEave = new Vector3(halfWidth + 0.45f, height - 0.1f, -0.45f);
        var ridgeFront = new Vector3(0, ridge, -0.45f);
        var ridgeBack = new Vector3(0, ridge, rear + 0.45f);
        Roof(house.transform, "west roof", thatch, leftEave, ridgeFront,
            ridgeBack, new Vector3(leftEave.x, leftEave.y, rear + 0.45f));
        Roof(house.transform, "east roof", thatch, ridgeFront, rightEave,
            new Vector3(rightEave.x, rightEave.y, rear + 0.45f), ridgeBack);
        Gable(house.transform, "front gable", wall, 0, halfWidth, height, ridge);
        Gable(house.transform, "back gable", wall, rear, halfWidth, height, ridge);

        // Foundation, window recesses and awning differ by biome, while the
        // player entrance stays unobstructed for every trader.
        if (stone)
        {
            var baseMaterial = MaterialOf("stone_floor_2x2") ?? wall;
            Box(house.transform, "stone plinth", baseMaterial,
                new Vector3(0, -0.38f, rear / 2), new Vector3(halfWidth * 2 + 0.7f, 0.38f, rear + 0.6f));
        }
        if (biome == "Swamp")
        {
            for (int x = -1; x <= 1; x += 2)
                for (int z = 1; z <= 7; z += 6)
                    Box(house.transform, "swamp support", wood, new Vector3(x * (halfWidth - 0.3f), -0.75f, z),
                        new Vector3(0.35f, 1.5f, 0.35f));
        }
        return true;
    }

    private static Material? MaterialOf(string prefab)
    {
        var source = PrefabManager.Instance.GetPrefab(prefab);
        return source == null ? null : source.GetComponentsInChildren<Renderer>(true)
            .FirstOrDefault(r => r.sharedMaterial != null)?.sharedMaterial;
    }

    private static void Box(Transform parent, string name, Material material, Vector3 position, Vector3 size)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localScale = size;
        go.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void Roof(Transform parent, string name, Material material, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var mesh = new Mesh { name = name };
        mesh.vertices = new[] { a, b, c, d };
        float length = Vector3.Distance(a, b) / 2f;
        float depth = Vector3.Distance(a, d) / 2f;
        mesh.uv = new[] { Vector2.zero, new Vector2(length, 0), new Vector2(length, depth), new Vector2(0, depth) };
        mesh.triangles = new[] { 0, 2, 1, 0, 3, 2, 1, 2, 0, 2, 3, 0 };
        mesh.RecalculateNormals();
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = material;
        go.AddComponent<MeshCollider>().sharedMesh = mesh;
    }

    private static void Gable(Transform parent, string name, Material material,
        float z, float halfWidth, float eave, float ridge)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var mesh = new Mesh { name = name };
        mesh.vertices = new[]
        {
            new Vector3(-halfWidth, eave, z), new Vector3(halfWidth, eave, z),
            new Vector3(0, ridge, z)
        };
        mesh.uv = new[] { Vector2.zero, new Vector2(halfWidth * 2, 0), new Vector2(halfWidth, ridge - eave) };
        mesh.triangles = new[] { 0, 2, 1, 1, 2, 0 };
        mesh.RecalculateNormals();
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = material;
    }
}
