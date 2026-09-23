using UnityEngine;

namespace ImmersiveTrader.Models;

public sealed record TraderCampProp(
    string Prefab,
    Vector3 Position,
    Vector3 Rotation,
    float Scale = 1f,
    bool Optional = true
);

public sealed record TraderCampDefinition(string TraderId, TraderCampProp[] Props);
