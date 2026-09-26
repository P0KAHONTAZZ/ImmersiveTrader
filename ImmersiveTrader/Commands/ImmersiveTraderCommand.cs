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
    public override string Help => "ImmersiveTrader tools: it help | list | find <id> | goto <id> | findall | spawn <id> | look <id> | diagnose | clearspawned | items | give <treasureId> <sourceTraderId> | route <source> <target> | task <offer|accept|status|turnin> <traderId> | rep <traderId> [add <points>|reset] | access <traderId> [grant|revoke]";

    public override void Run(string[] args, Terminal context)
    {
        if (args.Length == 0 || Eq(args[0], "help")) { PrintHelp(context); return; }
        switch (args[0].ToLowerInvariant())
        {
            case "list": PrintTraders(context); break;
            case "find": FindLocation(args, context); break;
            case "goto": GotoLocation(args, context); break;
            case "findall": FindAllLocations(context); break;
            case "spawn": SpawnTrader(args, context); break;
            case "look": SpawnNpcLook(args, context); break;
            case "clearspawned": ClearSpawnedTraders(context); break;
            case "diagnose": DiagnoseNearby(context); break;
            case "items": PrintTreasures(context); break;
            case "give": GiveTreasure(args, context); break;
            case "route": PrintRoute(args, context); break;
            case "task": TaskCommand(args, context); break;
            case "rep": ReputationCommand(args, context); break;
            case "access": AccessCommand(args, context); break;
            case "buff": BuffCommand(args, context); break;
            case "scroll": ScrollCommand(args, context); break;
            case "armorsets": foreach (var line in ArmorSetDump.Run().Split('\n')) context.AddString(line.TrimEnd('\r')); break;
            case "placement": foreach (var line in TraderPlacementCheck.Report()) context.AddString(line); break;
            default: context.AddString($"Unknown ImmersiveTrader command: {args[0]}"); PrintHelp(context); break;
        }
    }

    public override List<string> CommandOptionList() => new() { "help", "list", "find", "goto", "findall", "spawn", "look", "diagnose", "clearspawned", "items", "give", "route", "task", "rep", "access", "buff", "scroll", "placement", "armorsets" };

    private static bool Eq(string a, string b) => a.Equals(b, StringComparison.OrdinalIgnoreCase);

    private static void PrintHelp(Terminal c)
    {
        c.AddString("ImmersiveTrader developer commands:");
        c.AddString("  it list");
        c.AddString("  it find <traderId>  - show generated location position and add a map pin");
        c.AddString("  it goto <traderId>  - teleport to a generated trader location");
        c.AddString("  it findall          - add map pins for all generated trader locations");
        c.AddString("  it spawn <traderId>");
        c.AddString("  it look <traderId>");
        c.AddString("  it diagnose");
        c.AddString("  it clearspawned");
        c.AddString("  it items");
        c.AddString("  it give <treasureId> <sourceTraderId>");
        c.AddString("  it route <sourceTraderId> <targetTraderId>");
        c.AddString("  it task <offer|accept|status|turnin> <traderId>");
        c.AddString("  it rep <traderId>  - show this trader reputation");
        c.AddString("  it rep <traderId> add <points>  - add reputation points (max 64)");
        c.AddString("  it rep <traderId> reset  - reset reputation for this trader to 0");
        c.AddString("  it access <traderId> [grant|revoke]  - view or change this character\u0027s trader access");
        c.AddString("  it buff list | it buff <id> [seconds] | it buff clear");
        c.AddString("  it armorsets  - dump all armor sets with recipes to BepInEx/config/ImmersiveTrader-armor-sets.txt");
        c.AddString("  it placement  - check Midka/Troldad distances and dry camps");
        c.AddString("  it scroll list | it scroll all | it scroll <id> [count]  - give physical enhancement scrolls");
    }

    private static void ScrollCommand(string[] args, Terminal c)
    {
        var player = Player.m_localPlayer;
        if (player == null) { c.AddString("Enter a world first."); return; }
        if (args.Length < 2 || Eq(args[1], "list"))
        {
            c.AddString("Registered enhancement scrolls: " + string.Join(", ", EnhancementScrollItems.RegisteredIds));
            return;
        }
        if (Eq(args[1], "all"))
        {
            int given = 0;
            foreach (string sid in EnhancementScrollItems.RegisteredIds.ToList())
            {
                var p = ObjectDB.instance?.GetItemPrefab(EnhancementScrollItems.PrefabName(sid));
                if (p != null && player.GetInventory().CanAddItem(p, 1) && player.GetInventory().AddItem(p, 1)) given++;
            }
            c.AddString($"Given {given} enhancement scrolls (one of each).");
            return;
        }
        string id = args[1].ToLowerInvariant();
        if (!EnhancementScrollItems.IsRegistered(id)) { c.AddString("Scroll not registered. Use: it scroll list"); return; }
        int count = 1;
        if (args.Length >= 3 && (!int.TryParse(args[2], out count) || count < 1 || count > 10))
        {
            c.AddString("Usage: it scroll <id> [count 1-10]");
            return;
        }
        var prefab = ObjectDB.instance?.GetItemPrefab(EnhancementScrollItems.PrefabName(id));
        if (prefab == null) { c.AddString("Scroll prefab not found in ObjectDB."); return; }
        if (!player.GetInventory().CanAddItem(prefab, count)) { c.AddString("Not enough inventory space."); return; }
        if (!player.GetInventory().AddItem(prefab, count)) { c.AddString("Could not add scroll."); return; }
        c.AddString($"Given {count}x Scroll of {EnhancementStatusRegistry.DisplayName(id)}.");
    }

    private static void BuffCommand(string[] args, Terminal c)
    {
        var player = Player.m_localPlayer;
        if (player == null) { c.AddString("Enter a world first."); return; }
        if (args.Length < 2 || Eq(args[1], "list"))
        {
            c.AddString("Enhancement buffs (default 1800s; Rested always 1200s):");
            c.AddString(string.Join(", ", EnhancementStatusRegistry.Ids));
            return;
        }
        if (Eq(args[1], "clear"))
        {
            EnhancementStatusRegistry.Clear(player);
            c.AddString("ImmersiveTrader enhancement buffs cleared.");
            return;
        }
        float seconds = EnhancementStatusRegistry.DefaultDuration;
        if (args.Length >= 3 && (!float.TryParse(args[2], System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out seconds) || seconds <= 0f))
        {
            c.AddString("Usage: it buff <id> [seconds]");
            return;
        }
        EnhancementStatusRegistry.Apply(player, args[1], seconds, out string message);
        c.AddString(message);
    }

    private static void AccessCommand(string[] args, Terminal c)
    {
        var player = Player.m_localPlayer;
        if (player == null) { c.AddString("Enter a world first."); return; }
        if (args.Length != 2 && args.Length != 3)
        {
            c.AddString("Usage: it access <traderId> [grant|revoke]");
            return;
        }
        var trader = FindTrader(args[1]);
        if (trader == null || trader.IsLegendary) { c.AddString("Unknown regular trader. Use 'it list'."); return; }
        if (args.Length == 3)
        {
            if (Eq(args[2], "grant")) ProgressionGate.GrantAccess(player, trader.Id);
            else if (Eq(args[2], "revoke")) ProgressionGate.RevokeAccess(player, trader.Id);
            else { c.AddString("Usage: it access <traderId> [grant|revoke]"); return; }
        }
        bool progress = ProgressionGate.IsRewardTierUnlocked(player, trader.BiomeTier);
        bool overrideAccess = ProgressionGate.HasAccess(player, trader.Id);
        int materialTier = ProgressionGate.HighestKnownMaterialTier(player);
        c.AddString($"{trader.Name}: access={(progress || overrideAccess ? "open" : "locked")}; personal boss key or material progress={(progress ? "recognized" : "unrecognized")}; highest known material tier={materialTier}; manual access={(overrideAccess ? "granted" : "none")}.");
    }

    private static void ReputationCommand(string[] args, Terminal c)
    {
        var player = Player.m_localPlayer;
        if (player == null) { c.AddString("Enter a world first."); return; }
        if (args.Length != 2 && args.Length != 3 && args.Length != 4)
        {
            c.AddString("Usage: it rep <traderId> [add <points>]");
            return;
        }
        var trader = FindTrader(args[1]);
        if (trader == null || trader.IsLegendary) { c.AddString("Unknown regular trader. Use 'it list'."); return; }

        if (args.Length == 3)
        {
            if (!Eq(args[2], "reset")) { c.AddString("Usage: it rep <traderId> reset"); return; }
            int removed = TraderReputation.Reset(player, trader.Id);
            c.AddString($"{trader.Name}: reputation reset to 0 (removed {removed} points).");
        }
        if (args.Length == 4)
        {
            if (!Eq(args[2], "add") ||
                !int.TryParse(args[3], System.Globalization.NumberStyles.None,
                    System.Globalization.CultureInfo.InvariantCulture, out int points) || points <= 0)
            {
                c.AddString("Usage: it rep <traderId> add <positivePoints>");
                return;
            }
            int before = TraderReputation.Get(player, trader.Id);
            int after = TraderReputation.Add(player, trader.Id, points);
            c.AddString($"{trader.Name}: +{after - before} reputation points.");
        }
        c.AddString($"{trader.Name}: {TraderReputation.Describe(player, trader.Id)}");
    }

    private static void PrintTraders(Terminal c)
    {
        foreach (var t in TraderRegistry.Traders) c.AddString($"{t.Id} - {t.Name} [{t.Biome}] tier={t.BiomeTier}");
    }

    private static TraderDefinition? FindTrader(string id) =>
        TraderRegistry.Traders.FirstOrDefault(t => Eq(t.Id, id));

    private static void FindLocation(string[] args, Terminal c)
    {
        if (Player.m_localPlayer == null || ZoneSystem.instance == null) { c.AddString("Enter a world first."); return; }
        if (args.Length < 2) { c.AddString("Usage: it find <traderId>"); return; }
        var trader = FindTrader(args[1]);
        if (trader == null || trader.IsLegendary) { c.AddString("Unknown regular trader."); return; }
        AddLocationPins(c, trader.Id);
    }

    private static void GotoLocation(string[] args, Terminal c)
    {
        var player = Player.m_localPlayer;
        if (player == null || ZoneSystem.instance == null) { c.AddString("Enter a world first."); return; }
        if (args.Length != 2) { c.AddString("Usage: it goto <traderId>"); return; }
        var trader = FindTrader(args[1]);
        if (trader == null || trader.IsLegendary) { c.AddString("Unknown regular trader. Use 'it list'."); return; }

        string wanted = $"ImmersiveTrader_Location_{trader.Id}";
        foreach (var instance in ZoneSystem.instance.m_locationInstances.Values)
        {
            string name = instance.m_location?.m_prefabName ?? string.Empty;
            if (!name.Equals(wanted, StringComparison.OrdinalIgnoreCase) &&
                name.IndexOf(wanted, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            var destination = instance.m_position + new Vector3(0f, 2f, 3f);
            player.TeleportTo(destination, player.transform.rotation, true);
            c.AddString($"Teleporting to {trader.Name} at x={destination.x:0}, z={destination.z:0}.");
            return;
        }

        c.AddString($"No generated location for {trader.Name} is visible in this world. Try 'it find {trader.Id}'.");
    }

    private static void FindAllLocations(Terminal c)
    {
        if (Player.m_localPlayer == null || ZoneSystem.instance == null) { c.AddString("Enter a world first."); return; }
        c.AddString($"ZoneSystem location instances visible here: {ZoneSystem.instance.m_locationInstances.Count}; server={ZNet.instance?.IsServer()}.");
        var traders = TraderRegistry.Traders.Where(t => !t.IsLegendary).ToList();
        int placed = 0;
        int total = 0;
        var missing = new List<string>();
        foreach (var trader in traders)
        {
            int count = AddLocationPins(c, trader.Id, false);
            total += count;
            if (count > 0) placed++;
            else
            {
                missing.Add(trader.Id);
                c.AddString($"MISSING: {trader.Name} [{trader.Id}] ({trader.Biome}) has no generated location.");
            }
            if (PrefabManager.Instance.GetPrefab($"ImmersiveTrader_NPCLOOK_{trader.Id}") == null)
                c.AddString($"MISSING PREFAB: {trader.Id} has no NPCLOOK shell.");
        }
        c.AddString($"ImmersiveTrader locations: {placed}/{traders.Count} traders placed; {total} locations pinned.");
        if (missing.Count > 0)
        {
            c.AddString($"Missing trader IDs: {string.Join(", ", missing)}");
            foreach (var sample in ZoneSystem.instance.m_locationInstances.Values
                         .Where(x => x.m_location != null)
                         .Select(x => x.m_location.m_prefabName)
                         .Where(x => x != null && x.IndexOf("ImmersiveTrader", StringComparison.OrdinalIgnoreCase) >= 0)
                         .Distinct().Take(8))
                c.AddString($"Found location name: {sample}");
            c.AddString("If this is a multiplayer client, this local list may be incomplete; check the host log before concluding locations are missing.");
        }
    }

    private static int AddLocationPins(Terminal c, string traderId, bool reportMissing = true)
    {
        string wanted = $"ImmersiveTrader_Location_{traderId}";
        int count = 0;

        foreach (var pair in ZoneSystem.instance.m_locationInstances)
        {
            var instance = pair.Value;
            string name = instance.m_location?.m_prefabName ?? string.Empty;
            if (!name.Equals(wanted, StringComparison.OrdinalIgnoreCase) &&
                name.IndexOf(wanted, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            Vector3 pos = instance.m_position;
            var trader = FindTrader(traderId);
            string label = $"IT: {trader?.Name ?? traderId}";
            Minimap.instance?.AddPin(pos, Minimap.PinType.Icon3, label, true, false, 0L);
            c.AddString($"{label} at x={pos.x:0}, z={pos.z:0}, distance={Vector3.Distance(Player.m_localPlayer.transform.position, pos):0}m");
            count++;
        }

        if (count == 0 && reportMissing)
            c.AddString($"Location {wanted} is not present in ZoneSystem location instances.");
        return count;
    }

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
                var offers = TraderActivityRegistry.Activities.Where(x => x.TraderId == trader.Id).ToArray();
                if (offers.Length == 0) c.AddString("No contracts offered by this trader.");
                else foreach (var offer in offers)
                    c.AddString($"{offer.Title}: {offer.Description} Target={offer.TargetPrefab} x{offer.RequiredAmount}, reward=+{offer.RewardSkillLevels:0} levels {offer.RewardSkill}");
                break;
            case "accept":
                c.AddString("Take a physical contract scroll from this trader's shop to accept a task.");
                break;
            case "status":
                c.AddString(TraderActivityService.GetStatus(player, trader.Id));
                break;
            case "turnin":
                c.AddString(TraderActivityService.TryTurnInPhysical(player, trader.Id) ? "Task interaction handled." : "No active task.");
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
        float mult = RewardScaling.GetMultiplier(source.BiomeTier, target.BiomeTier);
        c.AddString($"{source.Name} (tier {source.BiomeTier}) -> {target.Name} (tier {target.BiomeTier}) = x{mult:0.##}");
    }
}
