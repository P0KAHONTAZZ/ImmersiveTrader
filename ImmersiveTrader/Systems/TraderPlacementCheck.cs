using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Reports whether generated trader camps meet the placement rules:
/// Midka and Troldad within 1000 m of the world spawn and at least 1000 m apart;
/// every camp above sea level.
/// </summary>
internal static class TraderPlacementCheck
{
    internal const float StarterMaxFromSpawn = 1000f;
    internal const float StarterMinApart = 1000f;
    private static bool _reported;

    internal static List<string> Report()
    {
        var lines = new List<string>();
        var zs = ZoneSystem.instance;
        if (zs == null) { lines.Add("World not loaded."); return lines; }

        Vector3 spawn = Vector3.zero;
        var positions = new Dictionary<string, Vector3>(StringComparer.OrdinalIgnoreCase);
        foreach (var inst in zs.m_locationInstances.Values)
        {
            string name = inst.m_location?.m_prefabName ?? string.Empty;
            if (name == "StartTemple") spawn = inst.m_position;
            const string prefix = "ImmersiveTrader_Location_";
            int i = name.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (i >= 0) positions[name.Substring(i + prefix.Length)] = inst.m_position;
        }

        float Flat(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
        if (positions.TryGetValue("midka", out var m) && positions.TryGetValue("troldad", out var t))
        {
            float dm = Flat(m, spawn), dt = Flat(t, spawn), apart = Flat(m, t);
            bool ok = dm <= StarterMaxFromSpawn && dt <= StarterMaxFromSpawn && apart >= StarterMinApart;
            lines.Add($"{(ok ? "OK" : "VIOLATION")}: Midka {dm:0} m from spawn, Troldad {dt:0} m from spawn, {apart:0} m apart " +
                      $"(rule: <= {StarterMaxFromSpawn:0} m, >= {StarterMinApart:0} m apart).");
        }
        else lines.Add("Midka/Troldad camps not generated (existing world or placement failed).");

        float water = zs.m_waterLevel;
        foreach (var pair in positions.OrderBy(p => p.Key))
        {
            float ground = zs.GetGroundHeight(pair.Value);
            if (ground < water + 0.5f)
                lines.Add($"WET CAMP: {pair.Key} ground {ground - water:0.0} m vs sea level (NPC guard moves it to dry land).");
        }
        return lines;
    }

    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    private static class ReportOnSpawn
    {
        private static void Postfix(Player __instance)
        {
            if (_reported || !ReferenceEquals(__instance, Player.m_localPlayer)) return;
            _reported = true;
            try { foreach (var line in Report()) Plugin.Log.LogInfo("Trader placement: " + line); }
            catch (Exception e) { Plugin.Log.LogWarning("Trader placement check failed: " + e.Message); }
        }
    }
}
