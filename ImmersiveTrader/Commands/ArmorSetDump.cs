using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using Jotunn.Managers;

namespace ImmersiveTrader.Commands;

/// <summary>
/// Dumps every armor set of the running game build (from ObjectDB recipes) with its set bonus,
/// pieces, crafting station/level and exact materials (display name + prefab name).
/// Output: BepInEx/config/ImmersiveTrader-armor-sets.txt and the console.
/// </summary>
internal static class ArmorSetDump
{
    internal static string Run()
    {
        var db = ObjectDB.instance;
        if (db == null) return "Enter a world first.";
        string L(string s) => Localization.instance != null ? Localization.instance.Localize(s) : s;

        var recipes = new Dictionary<string, Recipe>();
        foreach (var r in db.m_recipes)
            if (r != null && r.m_item != null && r.m_enabled && !recipes.ContainsKey(r.m_item.name))
                recipes[r.m_item.name] = r;

        var sets = new SortedDictionary<string, List<ItemDrop>>(StringComparer.OrdinalIgnoreCase);
        foreach (var go in db.m_items)
        {
            var drop = go != null ? go.GetComponent<ItemDrop>() : null;
            var shared = drop?.m_itemData?.m_shared;
            if (shared == null || string.IsNullOrEmpty(shared.m_setName)) continue;
            if (!sets.TryGetValue(shared.m_setName, out var list)) sets[shared.m_setName] = list = new List<ItemDrop>();
            list.Add(drop!);
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Armor sets in this build: {sets.Count}");
        foreach (var pair in sets)
        {
            var first = pair.Value[0].m_itemData.m_shared;
            var se = first.m_setStatusEffect;
            sb.AppendLine();
            sb.AppendLine($"== {pair.Key}  (pieces for bonus: {first.m_setSize})");
            if (se != null) sb.AppendLine($"   Bonus: {L(se.m_name)} - {L(se.GetTooltipString()).Replace("\n", "; ")}");
            foreach (var drop in pair.Value.OrderBy(d => d.m_itemData.m_shared.m_itemType))
            {
                var sh = drop.m_itemData.m_shared;
                sb.Append($"   - {L(sh.m_name)} [{drop.name}] armor {sh.m_armor}");
                if (recipes.TryGetValue(drop.name, out var rec))
                {
                    string station = rec.m_craftingStation != null ? L(rec.m_craftingStation.m_name) : "hand";
                    string mats = string.Join(", ", rec.m_resources
                        .Where(x => x?.m_resItem != null)
                        .Select(x => $"{x.m_amount} {L(x.m_resItem.m_itemData.m_shared.m_name)} [{x.m_resItem.name}]"));
                    sb.AppendLine($" | {station} lvl {rec.m_minStationLevel} | {mats}");
                }
                else sb.AppendLine(" | no recipe (drop/other)");
            }
        }

        string path = Path.Combine(Paths.ConfigPath, "ImmersiveTrader-armor-sets.txt");
        File.WriteAllText(path, sb.ToString());
        Plugin.Log.LogInfo("Armor sets dumped to " + path);
        return sb.ToString();
    }
}
