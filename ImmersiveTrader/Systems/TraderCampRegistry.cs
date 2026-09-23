using System.Collections.Generic;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Small authored camp layouts around traders. Kept data-only until prefab names are
/// runtime-verified; world generation must never fail because of a decorative prop.
/// </summary>
public static class TraderCampRegistry
{
    public static readonly IReadOnlyList<TraderCampDefinition> Camps = new List<TraderCampDefinition>
    {
        new("midka", new[]
        {
            new TraderCampProp("fire_pit", new Vector3(2.5f, 0f, 1.5f), Vector3.zero),
            new TraderCampProp("piece_chest_wood", new Vector3(-2f, 0f, 1.5f), new Vector3(0f, 25f, 0f))
        }),
        new("troldad", new[]
        {
            new TraderCampProp("fire_pit", new Vector3(3f, 0f, 1f), Vector3.zero),
            new TraderCampProp("piece_chest_wood", new Vector3(-2.5f, 0f, 1f), new Vector3(0f, 330f, 0f))
        }),
        new("mokra_dzika", new[]
        {
            new TraderCampProp("fire_pit", new Vector3(2f, 0f, 2f), Vector3.zero),
            new TraderCampProp("piece_chest_wood", new Vector3(-2f, 0f, 1f), new Vector3(0f, 45f, 0f))
        }),
        new("encek", new[]
        {
            new TraderCampProp("fire_pit", new Vector3(2.5f, 0f, 1f), Vector3.zero),
            new TraderCampProp("piece_chest_wood", new Vector3(-2.5f, 0f, 1f), new Vector3(0f, 20f, 0f))
        })
    };
}
