using System.Collections.Generic;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderCampRegistry
{
    public static readonly IReadOnlyList<TraderCampDefinition> Camps = new List<TraderCampDefinition>
    {
        // Prototype settlement; the other Meadows trader and other biomes follow once it looks right in game.
        new("midka", MeadowsSettlement(), LevelRadius: 15f),
        Camp("troldad",
            P("fire_pit", 3f, 1f), P("piece_chest_wood", -2.5f, 1f, 330f),
            P("piece_sitting_log", 1.5f, -2.5f, 25f), P("wood_stack", -3.5f, -1.5f, 80f)),
        Camp("grimvald",
            P("fire_pit", 2.5f, 1.5f), P("piece_chest_wood", -2.5f, 1.5f, 20f),
            P("wood_stack", -3.5f, -1f, 70f), P("piece_sitting_log", 1f, -2.5f)),
        Camp("rudy_warg",
            P("fire_pit", 2f, 2f), P("piece_chest_wood", -2.5f, 1f, 40f),
            P("piece_sitting_log", 1f, -2.5f, 350f)),
        Camp("mokra_dzika",
            P("fire_pit", 2f, 2f), P("piece_chest_wood", -2f, 1f, 45f),
            P("piece_sitting_log", 1f, -2.5f, 20f)),
        Camp("encek",
            P("fire_pit", 2.5f, 1f), P("piece_chest_wood", -2.5f, 1f, 20f),
            P("piece_sitting_log", 1f, -2.5f, 340f)),
        Camp("hrothgar",
            P("fire_pit", 2f, 1.5f), P("piece_chest_wood", -2f, 1.5f, 30f),
            P("piece_sitting_log", 1f, -2.5f)),
        Camp("ylva_frost",
            P("fire_pit", 2.5f, 1f), P("piece_chest_wood", -2.5f, 1f, 30f),
            P("piece_sitting_log", 1f, -2.5f, 15f)),
        Camp("bjarki_goldtooth",
            P("fire_pit", 2.5f, 1.5f), P("piece_chest_wood", -2.5f, 1.5f, 20f),
            P("piece_sitting_log", 1f, -2.5f)),
        Camp("ragnar_turnipson",
            P("fire_pit", 2f, 2f), P("piece_chest_wood", -2.5f, 1f, 25f),
            P("piece_sitting_log", 1f, -2.5f)),
        Camp("cmok",
            P("fire_pit", 2f, 1.5f), P("piece_chest_wood", -2f, 1.5f, 30f),
            P("piece_sitting_log", 1f, -2.5f)),
        Camp("grelka",
            P("fire_pit", 2.5f, 1f), P("piece_chest_wood", -2.5f, 1f, 25f),
            P("piece_sitting_log", 1f, -2.5f)),
        Camp("spalony_zenek",
            P("fire_pit", 2.5f, 1.5f), P("piece_chest_wood", -2.5f, 1.5f, 25f),
            P("piece_sitting_log", 1f, -2.5f)),
        Camp("skjold_cinderborn",
            P("fire_pit", 2f, 2f), P("piece_chest_wood", -2.5f, 1f, 20f),
            P("piece_sitting_log", 1f, -2.5f))
    };

    private static TraderCampDefinition Camp(string id, params TraderCampProp[] props) => new(id, props);
    private static TraderCampProp P(string prefab, float x, float z, float y = 0f, float scale = 1f) =>
        new(prefab, new Vector3(x, 0f, z), new Vector3(0f, y, 0f), scale);
    private static TraderCampProp Piece(string prefab, float x, float height, float z, float yaw) =>
        new(prefab, new Vector3(x, height, z), new Vector3(0f, yaw, 0f));

    // ponytail: piece pivots are tuned from in-game screenshots, not read from the assets.
    // Confirmed: woodwall pivots at its centre; roof tiles rise along local -Z.
    private const float RoofUphillYaw = -90f;
    private const float WallCenterHeight = 1f;
    private const float PostHeight = .6f;

    /// <summary>Trader stands at the origin facing +Z; the village sits behind and beside them.</summary>
    private static TraderCampProp[] MeadowsSettlement()
    {
        var props = new List<TraderCampProp>();
        props.AddRange(Cabin(0f, -6.5f, 0f));       // Midka's hut, doorway facing the trader
        props.AddRange(Cabin(-7.5f, -2.5f, 90f));   // storage hut, doorway facing the yard
        props.AddRange(Fence(new Vector2(-11f, 1.5f), new Vector2(-11f, -10.5f), new Vector2(9f, -10.5f), new Vector2(9f, -2.5f)));
        props.AddRange(new[]
        {
            P("fire_pit", 4.5f, 1.5f), P("piece_cookingstation", 4.5f, 1.5f),
            P("piece_bench01", 4.5f, 3.6f), P("piece_bench01", 6.6f, 1.5f, 90f),
            P("piece_sitting_log", 2.5f, 3.2f, 30f),
            P("piece_chest_wood", -2f, 1.5f, 25f), P("piece_workbench", -3.3f, -3.5f, 90f),
            P("piece_table", 5.2f, -3.8f), P("wood_stack", 3.3f, -7f, 90f),
            P("piece_groundtorch_wood", -1.8f, -3.8f), P("piece_groundtorch_wood", 2.4f, -3.8f),
            P("piece_groundtorch_wood", 6.8f, -1.2f), P("piece_groundtorch_wood", -5f, 2.2f)
        });
        return props.ToArray();
    }

    /// <summary>4x4 m log hut with a gable roof and an open doorway on its local +Z side.</summary>
    private static IEnumerable<TraderCampProp> Cabin(float cx, float cz, float yaw)
    {
        var turn = Quaternion.Euler(0f, yaw, 0f);
        TraderCampProp At(string prefab, float x, float y, float z, float localYaw)
        {
            var p = turn * new Vector3(x, y, z);
            return Piece(prefab, cx + p.x, p.y, cz + p.z, yaw + localYaw);
        }

        foreach (float x in new[] { -1f, 1f })
        foreach (float z in new[] { -1f, 1f })
        {
            yield return At("wood_floor", x, .05f, z, 0f);
            yield return At("wood_roof", x, 2.5f, z, x < 0 ? RoofUphillYaw : RoofUphillYaw + 180f);
        }

        yield return At("woodwall", -1f, WallCenterHeight, -2f, 0f);
        yield return At("woodwall", 1f, WallCenterHeight, -2f, 0f);
        yield return At("woodwall", -1f, WallCenterHeight, 2f, 180f); // x = +1 stays open as the doorway
        foreach (float x in new[] { -2f, 2f })
        foreach (float z in new[] { -1f, 1f })
            yield return At("woodwall", x, WallCenterHeight, z, 90f);
    }

    /// <summary>Post-and-rail fence along a polyline, one post every ~2 m.</summary>
    private static IEnumerable<TraderCampProp> Fence(params Vector2[] path)
    {
        yield return Piece("wood_pole2", path[0].x, PostHeight, path[0].y, 0f);
        for (int i = 1; i < path.Length; i++)
        {
            Vector2 a = path[i - 1], b = path[i], d = b - a;
            float yaw = Mathf.Atan2(-d.y, d.x) * Mathf.Rad2Deg; // beams run along local X
            int spans = Mathf.Max(1, Mathf.RoundToInt(d.magnitude / 2f));
            for (int s = 1; s <= spans; s++)
            {
                var post = Vector2.Lerp(a, b, s / (float)spans);
                var mid = Vector2.Lerp(a, b, (s - .5f) / spans);
                yield return Piece("wood_pole2", post.x, PostHeight, post.y, 0f);
                yield return Piece("wood_beam", mid.x, .55f, mid.y, yaw);
                yield return Piece("wood_beam", mid.x, 1.15f, mid.y, yaw);
            }
        }
    }
}
