using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Records boss progression on the character that participated in the kill.
/// Vanilla's defeated_* keys remain world-wide; ImmersiveTrader uses its own
/// character unique keys so one player's boss kill does not unlock traders for others.
/// </summary>
[HarmonyPatch(typeof(Character), "OnDeath")]
internal static class PersonalBossProgression
{
    private static readonly Dictionary<string, string> BossKeys = new()
    {
        ["Eikthyr"] = "defeated_eikthyr",
        ["gd_king"] = "defeated_gdking",
        ["Bonemass"] = "defeated_bonemass",
        ["Dragon"] = "defeated_dragon",
        ["GoblinKing"] = "defeated_goblinking",
        ["SeekerQueen"] = "defeated_queen",
        ["Fader"] = "defeated_fader"
    };

    private const float ParticipationRadius = 100f;

    private static void Prefix(Character __instance)
    {
        if (__instance == null || !__instance.IsBoss()) return;
        string prefab = Utils.GetPrefabName(__instance.gameObject);
        if (!BossKeys.TryGetValue(prefab, out string vanillaKey)) return;

        foreach (Player player in Player.GetAllPlayers())
        {
            if (player == null || Vector3.Distance(player.transform.position, __instance.transform.position) > ParticipationRadius)
                continue;

            string key = ProgressionGate.PersonalBossKey(vanillaKey);
            if (!player.HaveUniqueKey(key))
            {
                player.AddUniqueKey(key);
                Plugin.Log.LogInfo($"Personal boss progression granted: player={player.GetPlayerName()}, boss={prefab}, key={key}");
            }
        }
    }
}
