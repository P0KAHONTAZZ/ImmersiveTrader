using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Physics for procedurally built ground models (scrolls, contracts).
/// Thin colliders with discrete collision detection tunnel through terrain when dropped;
/// Valheim then relocates the item. A thick box + continuous detection prevents that.
/// </summary>
internal static class WorldItemPhysics
{
    internal static void Setup(GameObject prefab, Vector3 visualSize, float visualBottom)
    {
        var body = prefab.GetComponent<Rigidbody>() ?? prefab.AddComponent<Rigidbody>();
        body.mass = 0.1f;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.maxDepenetrationVelocity = 2f;

        // Remove every collider inherited from the base prefab (root and children).
        foreach (var collider in prefab.GetComponentsInChildren<Collider>(true))
            Object.Destroy(collider);

        // At least 20 cm thick; bottom aligned with the visual bottom so it rests on the ground.
        float height = Mathf.Max(0.20f, visualSize.y);
        var box = prefab.AddComponent<BoxCollider>();
        box.size = new Vector3(Mathf.Max(0.2f, visualSize.x), height, Mathf.Max(0.2f, visualSize.z));
        box.center = new Vector3(0f, visualBottom + height * 0.5f, 0f);
    }
}
