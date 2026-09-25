using UnityEngine;

namespace ImmersiveTrader.Models;

public sealed record TraderCampProp(
    string Prefab,
    Vector3 Position,
    Vector3 Rotation,
    float Scale = 1f,
    bool Optional = true
);

// LevelRadius > 0 flattens terrain under a larger settlement and widens the location footprint.
public sealed record TraderCampDefinition(string TraderId, TraderCampProp[] Props, float LevelRadius = 0f);
