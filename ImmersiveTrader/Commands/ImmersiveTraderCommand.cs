using System;
using System.Collections.Generic;
using System.Linq;
using ImmersiveTrader.Models;
using ImmersiveTrader.Components;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using BepInEx.Logging;

namespace ImmersiveTrader.Commands;

public sealed class ImmersiveTraderCommand : ConsoleCommand
{
    public override string Name => "it";
    public override string Help => "ImmersiveTrader tools: it help | list | spawn <id> | look <id> | diagnose | clearspawned | items | give <treasureId> <sourceTraderId> | route <source> <target> | task <offer|accept|status|turnin> <traderId>";

    public override void Run(string[] args, Terminal context)
    {
        if (args.Length == 0 || Eq(args[0], "help")) { PrintHelp(context); return; }
        switch (args[0].ToLowerInvariant())
        {
            case "list": PrintTraders(context); break;
            case "spawn": SpawnTrader(args, context); break;
            case "look": SpawnNpcLook(args, context); break;
            case "clearspawned": ClearSpawnedTraders(context); break;
            case "diagnose": DiagnoseNearby(context); break;
            case "items": PrintTreasures(context); break;
            case "give": GiveTreasure(args, context); break;
            case "route": PrintRoute(args, context); break;
            case "task": TaskCommand(args, context); break;
            default: context.AddString($"Unknown ImmersiveTrader command: {args[0]}"); PrintHelp(context); break;
        }
    }

    public override List<string> CommandOptionList() => new() { "help", "list", "spawn", "look", "diagnose", "clearspawned", "items", "give", "route", "task" };

    private static bool Eq(string a, string b) => a.Equals(b, StringComparison.OrdinalIgnoreCase);

    private static void PrintHelp(Terminal c)
    {
        c.AddString("ImmersiveTrader developer commands:");
        c.AddString("  it list");
        c.AddString("  it spawn <traderId>");
        c.AddString("  it look <traderId>");
        c.AddString("  it diagnose");
        c.AddString("  it clearspawned");
        c.AddString("  it items");
        c.AddString("  it give <treasureId> <sourceTraderId>");
        c.AddString("  it route <sourceTraderId> <targetTraderId>");
        c.AddString("  it task <offer|accept|status|turnin> <traderId>");
    }

    private static void PrintTraders(Terminal c)
    {
        foreach (var t in TraderRegistry.Traders) c.AddString($"{t.Id} - {t.Name} [{t.Biome}] tier={t.BiomeTier}");
    }

    private static TraderDefinition? FindTrader(string id) =>
        TraderRegistry.Traders.FirstOrDefault(t => Eq(t.Id, id));

    private static void SpawnTrader(string[] args, Terminal c)
    {
        if (Player.m_localPlayer == null) { c.AddString("Enter a world first."); return; }
        if (args.Length < 2) { c.AddString("Usage: it spawn <traderId>"); return; }
        var trader = FindTrader(args[1]);
        if (trader == null) { c.AddString("Unknown trader. Use 'it list'."); return; }
        var prefabName = $"ImmersiveTrader_NPC_{trader.Id}";
        var prefab = PrefabManager.Instance.GetPrefab(prefabName);
        if (prefab == null) { c.AddString($"Prefab not registered: {prefabName}"); return; }
        var p = Player.m_localPlayer;
        var spawned = UnityEngine.Object.Instantiate(prefab, p.transform.position + p.transform.forward * 2f, Quaternion.identity);
        c.AddString(spawned != null ? $"Spawned {trader.Name}." : $"Failed to spawn {trader.Name}.");
    }


    private static void SpawnNpcLook(string[] args, Terminal c)
    {
        if (Player.m_localPlayer == null) { c.AddString("Enter a world first."); return; }
        if (args.Length < 2) { c.AddString("Usage: it look <traderId>"); return; }
        var trader = FindTrader(args[1]);
        if (trader == null || trader.IsLegendary) { c.AddString("Unknown regular trader."); return; }
        string prefabName = $"ImmersiveTrader_NPCLOOK_{trader.Id}";
        var prefab = PrefabManager.Instance.GetPrefab(prefabName);
        if (prefab == null) { c.AddString($"Prototype not registered: {prefabName}"); return; }
        var p = Player.m_localPlayer;
        var spawned = UnityEngine.Object.Instantiate(prefab, p.transform.position + p.transform.forward * 3f, Quaternion.identity);
        c.AddString(spawned != null ? $"Spawned NPC-shell prototype for {trader.Name}." : "Spawn failed.");
    }

    private static void DiagnoseNearby(Terminal c)
    {
        var player = Player.m_localPlayer;
        if (player == null) { c.AddString("Enter a world first."); return; }

        c.AddString("=== Nearby object diagnostic (30m) ===");
        int traderCount = 0, namedCount = 0, hildirCount = 0;
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (go == null || !go.scene.IsValid() || !go.activeInHierarchy) continue;
            float distance = Vector3.Distance(player.transform.position, go.transform.position);
            if (distance > 30f) continue;

            string name = go.name ?? string.Empty;
            bool oursByName = name.IndexOf("ImmersiveTrader", StringComparison.OrdinalIgnoreCase) >= 0;
            bool vanillaShell = name.IndexOf("Hildir", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                name.IndexOf("Haldor", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                name.IndexOf("Troll", StringComparison.OrdinalIgnoreCase) >= 0;
            var npc = go.GetComponent<TraderNpc>();
            var trader = go.GetComponent<Trader>();
            if (!oursByName && !vanillaShell && npc == null && trader == null) continue;

            var view = go.GetComponent<ZNetView>();
            string zdo = view == null ? "no-ZNV" : (!view.IsValid() ? "ZNV-invalid" : $"ZNV-valid owner={view.IsOwner()}");
            string parent = go.transform.parent != null ? go.transform.parent.name : "<root>";
            c.AddString($"OBJ {++namedCount}: {name} parent={parent} d={distance:0.0} {zdo} TraderNpc={(npc != null ? npc.TraderId : "-")} Trader={(trader != null)}");
            if (npc != null) traderCount++;
            if (name.IndexOf("Hildir", StringComparison.OrdinalIgnoreCase) >= 0) hildirCount++;
        }
        c.AddString($"Summary: matchingObjects={namedCount}, TraderNpc={traderCount}, HildirNamed={hildirCount}");
    }


    private static void ClearSpawnedTraders(Terminal c)
    {
        if (Player.m_localPlayer == null) { c.AddString("Enter a world first."); return; }

        int removed = 0;
        foreach (var npc in UnityEngine.Object.FindObjectsOfType<TraderNpc>())
        {
            if (npc == null || npc.gameObject == null) continue;
            string name = npc.gameObject.name;
            if (!name.StartsWith("ImmersiveTrader_NPC_", StringComparison.Ordinal) &&
                !name.StartsWith("ImmersiveTrader_NPCLOOK_", StringComparison.Ordinal))
                continue;

            // Cleanup is intentionally exhaustive now. Old builds persisted both loose
            // test NPCs and networked copies embedded in generated locations. Keeping
            // one of those legacy copies is exactly what made the duplicate return after
            // a reload. Current location templates recreate their single canonical NPC.
            var nview = npc.GetComponent<ZNetView>();
            if (nview != null && nview.IsValid() && ZNetScene.instance != null)
            {
                if (!nview.IsOwner()) nview.ClaimOwnership();
                ZNetScene.instance.Destroy(npc.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(npc.gameObject);
            }
            removed++;
        }

        c.AddString($"Removed {removed} ImmersiveTrader NPC instance(s). Reload the world once.");
    }


    private static void PrintTreasures(Terminal c)
    {
        if (Player.m_localPlayer == null) { c.AddString("Enter a world first."); return; }
        var carried = InventoryTreasureService.GetCarried(Player.m_localPlayer);
        if (carried.Count == 0) { c.AddString("No active ImmersiveTrader shipments."); return; }
        int i = 1;
        foreach (var x in carried)
        {
            var source = FindTrader(x.SourceTraderId);
            c.AddString($"#{i++} {x.TreasureId} | from={source?.Name ?? x.SourceTraderId} | tier={x.SourceBiomeTier} | shipment={x.ShipmentId}");
        }
        c.AddString($"Active shipments: {carried.Count}/{Plugin.MaxCarriedTreasures.Value}");
    }

    private static void GiveTreasure(string[] args, Terminal c)
    {
        if (Player.m_localPlayer == null) { c.AddString("Enter a world first."); return; }
        if (args.Length < 3) { c.AddString("Usage: it give <treasureId> <sourceTraderId>"); return; }
        var def = TreasureRegistry.Treasures.FirstOrDefault(t => Eq(t.Id, args[1]));
        var source = FindTrader(args[2]);
        if (def == null || source == null || source.IsLegendary) { c.AddString("Unknown treasure/source or legendary source."); return; }
        if (InventoryTreasureService.Count(Player.m_localPlayer) >= Plugin.MaxCarriedTreasures.Value) { c.AddString("Shipment limit reached."); return; }
        var prefab = ObjectDB.instance.GetItemPrefab($"ImmersiveTrader_{def.Id}");
        if (prefab == null) { c.AddString("Treasure prefab not found."); return; }
        var inv = Player.m_localPlayer.GetInventory();
        var before = inv.GetAllItems().ToList();
        if (!inv.AddItem(prefab, 1)) { c.AddString("Could not add treasure."); return; }
        var item = inv.GetAllItems().LastOrDefault(x => !before.Contains(x) && x.m_dropPrefab != null && x.m_dropPrefab.name.StartsWith($"ImmersiveTrader_{def.Id}"));
        if (item == null) { c.AddString("Treasure was added but could not be identified."); return; }
        TreasureMetadata.Stamp(item, source.Id, source.BiomeTier);
        c.AddString($"Added {def.DisplayName} from {source.Name}. Use 'it items' to inspect it.");
    }


    private static void TaskCommand(string[] args, Terminal c)
    {
        var player = Player.m_localPlayer;
        if (player == null) { c.AddString("Enter a world first."); return; }
        if (args.Length < 3) { c.AddString("Usage: it task <offer|accept|status|turnin> <traderId>"); return; }
        var trader = FindTrader(args[2]);
        if (trader == null) { c.AddString("Unknown trader ID."); return; }

        switch (args[1].ToLowerInvariant())
        {
            case "offer":
                var offers = TraderActivityService.GetOffers(player, trader.Id);
                if (offers.Length == 0) c.AddString("All contracts complete.");
                else foreach (var offer in offers)
                    c.AddString($"{offer.Title}: {offer.Description} Target={offer.TargetPrefab} x{offer.RequiredAmount}, reward=+{offer.RewardSkillLevels:0} {offer.RewardSkill}");
                break;
            case "accept":
                c.AddString(TraderActivityService.TryAccept(player, trader.Id) ? "Task accepted." : "Could not accept task.");
                break;
            case "status":
                c.AddString(TraderActivityService.GetStatus(player, trader.Id));
                break;
            case "turnin":
                c.AddString(TraderActivityService.TryTurnIn(player, trader.Id) ? "Task interaction handled." : "No active task.");
                break;
            default:
                c.AddString("Usage: it task <offer|accept|status|turnin> <traderId>");
                break;
        }
    }

    private static void PrintRoute(string[] args, Terminal c)
    {
        if (args.Length < 3) { c.AddString("Usage: it route <sourceTraderId> <targetTraderId>"); return; }
        var source = FindTrader(args[1]);
        var target = FindTrader(args[2]);
        if (source == null || target == null) { c.AddString("Unknown trader ID."); return; }
        if (source.Id == target.Id) { c.AddString("Same trader cannot receive its own shipment."); return; }
        if (target.IsLegendary) { c.AddString($"{source.Name} -> {target.Name}: legendary fixed reward table (no distance multiplier)."); return; }
        int mult = RewardScaling.GetMultiplier(source.BiomeTier, target.BiomeTier);
        c.AddString($"{source.Name} (tier {source.BiomeTier}) -> {target.Name} (tier {target.BiomeTier}) = x{mult}");
    }
}
