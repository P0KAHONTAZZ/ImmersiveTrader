using System.Collections.Generic;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderCampRegistry
{
    public static readonly IReadOnlyList<TraderCampDefinition> Camps = new List<TraderCampDefinition>
    {
        Sheltered("midka", "Meadows",
            P("piece_table", -3f, -2f, 90f), P("piece_chest", -4f, -1f, 15f),
            P("piece_groundtorch_green", -5f, 0f),
            P("fire_pit", 2.5f, 1.5f), P("piece_chest_wood", -2f, 1.5f, 25f),
            P("piece_workbench", -3.5f, -1f, 90f), P("piece_sitting_log", 1f, -2.5f, 15f)),
        Sheltered("troldad", "Meadows",
            P("fire_pit", 3f, 1f), P("piece_chest_wood", -2.5f, 1f, 330f),
            P("piece_sitting_log", 1.5f, -2.5f, 25f), P("wood_stack", -3.5f, -1.5f, 80f)),
        Sheltered("grimvald", "Black Forest",
            P("fire_pit", 2.5f, 1.5f), P("piece_chest_wood", -2.5f, 1.5f, 20f),
            P("wood_stack", -3.5f, -1f, 70f), P("piece_sitting_log", 1f, -2.5f)),
        Sheltered("rudy_warg", "Black Forest",
            P("fire_pit", 2f, 2f), P("piece_chest_wood", -2.5f, 1f, 40f),
            P("piece_sitting_log", 1f, -2.5f, 350f)),
        Sheltered("mokra_dzika", "Swamp",
            P("fire_pit", 2f, 2f), P("piece_chest_wood", -2f, 1f, 45f),
            P("piece_sitting_log", 1f, -2.5f, 20f)),
        Sheltered("encek", "Swamp",
            P("fire_pit", 2.5f, 1f), P("piece_chest_wood", -2.5f, 1f, 20f),
            P("piece_sitting_log", 1f, -2.5f, 340f)),
        Sheltered("hrothgar", "Mountains",
            P("fire_pit", 2f, 1.5f), P("piece_chest_wood", -2f, 1.5f, 30f),
            P("piece_sitting_log", 1f, -2.5f)),
        Sheltered("ylva_frost", "Mountains",
            P("fire_pit", 2.5f, 1f), P("piece_chest_wood", -2.5f, 1f, 30f),
            P("piece_sitting_log", 1f, -2.5f, 15f)),
        Sheltered("bjarki_goldtooth", "Plains",
            P("fire_pit", 2.5f, 1.5f), P("piece_chest_wood", -2.5f, 1.5f, 20f),
            P("piece_sitting_log", 1f, -2.5f)),
        Sheltered("ragnar_turnipson", "Plains",
            P("fire_pit", 2f, 2f), P("piece_chest_wood", -2.5f, 1f, 25f),
            P("piece_sitting_log", 1f, -2.5f)),
        Sheltered("cmok", "Mistlands",
            P("fire_pit", 2f, 1.5f), P("piece_chest_wood", -2f, 1.5f, 30f),
            P("piece_sitting_log", 1f, -2.5f)),
        Sheltered("grelka", "Mistlands",
            P("fire_pit", 2.5f, 1f), P("piece_chest_wood", -2.5f, 1f, 25f),
            P("piece_sitting_log", 1f, -2.5f)),
        Sheltered("spalony_zenek", "Ashlands",
            P("fire_pit", 2.5f, 1.5f), P("piece_chest_wood", -2.5f, 1.5f, 25f),
            P("piece_sitting_log", 1f, -2.5f)),
        Sheltered("skjold_cinderborn", "Ashlands",
            P("fire_pit", 2f, 2f), P("piece_chest_wood", -2.5f, 1f, 20f),
            P("piece_sitting_log", 1f, -2.5f))
    };

    // Small roofed stalls fit inside the existing ten-metre locations. The NPC stands
    // at the centre and keeps a clear route to the player; the shelter sits behind it.
    private static TraderCampDefinition Sheltered(string id, string biome, params TraderCampProp[] details)
    {
        var pieces = new List<TraderCampProp>();
        bool stone = biome == "Mountains" || biome == "Mistlands" || biome == "Ashlands";
        bool swamp = biome == "Swamp";
        string wall = stone ? "stone_wall_2x1" : "woodwall";
        string roof = biome == "Mistlands" || biome == "Ashlands" ? "wood_roof_45" : "wood_roof";
        float height = swamp ? 0.75f : 0f;

        // Raised, open-front market stall; visual geometry only.
        for (int x = -1; x <= 1; x++)
        {
            pieces.Add(P3("wood_floor", x * 2f, height, 5f));
            pieces.Add(P3(wall, x * 2f, height + 1f, 7f));
            pieces.Add(P3(roof, x * 2f, height + 2.7f, 5f));
        }
        pieces.Add(P3("wood_pole2", -3f, height + 1.3f, 3.5f));
        pieces.Add(P3("wood_pole2", 3f, height + 1.3f, 3.5f));
        pieces.Add(P3("wood_pole2", -3f, height + 1.3f, 7f));
        pieces.Add(P3("wood_pole2", 3f, height + 1.3f, 7f));
        pieces.Add(P3("wood_beam", -2f, height + 2.6f, 3.5f));
        pieces.Add(P3("wood_beam", 0f, height + 2.6f, 3.5f));
        pieces.Add(P3("wood_beam", 2f, height + 2.6f, 3.5f));
        if (swamp)
        {
            pieces.Add(P3("wood_stair", 0f, 0f, 3f));
            pieces.Add(P("piece_groundtorch_green", -3.8f, 3f));
        }
        if (stone)
        {
            pieces.Add(P3("stone_floor_2x2", -2f, -0.25f, 5f));
            pieces.Add(P3("stone_floor_2x2", 0f, -0.25f, 5f));
            pieces.Add(P3("stone_floor_2x2", 2f, -0.25f, 5f));
        }
        pieces.AddRange(details);
        return new TraderCampDefinition(id, pieces.ToArray());
    }

    private static TraderCampProp P3(string prefab, float x, float height, float z) =>
        new(prefab, new Vector3(x, height, z), Vector3.zero);
    private static TraderCampProp P(string prefab, float x, float z, float y = 0f, float scale = 1f) =>
        new(prefab, new Vector3(x, 0f, z), new Vector3(0f, y, 0f), scale);
}
