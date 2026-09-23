using System.Collections.Generic;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class TraderCampRegistry
{
    public static readonly IReadOnlyList<TraderCampDefinition> Camps = new List<TraderCampDefinition>
    {
        Camp("midka",
            P("fire_pit", 2.5f, 1.5f), P("piece_chest_wood", -2f, 1.5f, 25f),
            P("piece_workbench", -3.5f, -1f, 90f), P("piece_sitting_log", 1f, -2.5f, 15f)),
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
}
