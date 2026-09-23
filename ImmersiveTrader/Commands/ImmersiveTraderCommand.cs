using System;
using System.Collections.Generic;
using System.Linq;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader.Commands;

public sealed class ImmersiveTraderCommand : ConsoleCommand
{
    public override string Name => "it";

    public override string Help => "ImmersiveTrader tools: it help | it list | it spawn <traderId> | it items";

    public override void Run(string[] args, Terminal context)
    {
        if (args.Length == 0 || args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            PrintHelp(context);
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "list":
                PrintTraders(context);
                break;
            case "spawn":
                SpawnTrader(args, context);
                break;
            case "items":
                PrintTreasures(context);
                break;
            default:
                context.AddString($"Unknown ImmersiveTrader command: {args[0]}");
                PrintHelp(context);
                break;
        }
    }

    public override List<string> CommandOptionList() => new()
    {
        "help",
        "list",
        "spawn",
        "items"
    };

    private static void PrintHelp(Terminal context)
    {
        context.AddString("ImmersiveTrader developer commands:");
        context.AddString("  it list                 - list trader IDs");
        context.AddString("  it spawn <traderId>     - spawn a trader 2 m in front of you");
        context.AddString("  it items                - show carried ImmersiveTrader treasures");
    }

    private static void PrintTraders(Terminal context)
    {
        foreach (var trader in TraderRegistry.Traders)
            context.AddString($"{trader.Id} - {trader.Name} [{trader.Biome}]");
    }

    private static void SpawnTrader(string[] args, Terminal context)
    {
        if (Player.m_localPlayer == null)
        {
            context.AddString("This command can only be used after entering a world.");
            return;
        }

        if (args.Length < 2)
        {
            context.AddString("Usage: it spawn <traderId>");
            return;
        }

        var traderId = args[1].Trim().ToLowerInvariant();
        var trader = TraderRegistry.Traders.FirstOrDefault(t =>
            t.Id.Equals(traderId, StringComparison.OrdinalIgnoreCase));

        if (trader == null)
        {
            context.AddString($"Unknown trader ID: {traderId}. Use 'it list'.");
            return;
        }

        var prefabName = $"ImmersiveTrader_NPC_{trader.Id}";
        var prefab = PrefabManager.Instance.GetPrefab(prefabName);
        if (prefab == null)
        {
            context.AddString($"Prefab is not registered: {prefabName}");
            return;
        }

        var player = Player.m_localPlayer;
        var position = player.transform.position + player.transform.forward * 2f;
        var spawned = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
        context.AddString(spawned != null
            ? $"Spawned {trader.Name} ({prefabName})."
            : $"Failed to spawn {trader.Name}.");
    }

    private static void PrintTreasures(Terminal context)
    {
        if (Player.m_localPlayer == null)
        {
            context.AddString("This command can only be used after entering a world.");
            return;
        }

        var inventory = Player.m_localPlayer.GetInventory();
        var found = 0;

        foreach (var item in inventory.GetAllItems())
        {
            if (!item.m_dropPrefab || !item.m_dropPrefab.name.StartsWith("ImmersiveTrader_", StringComparison.Ordinal))
                continue;

            context.AddString($"{item.m_dropPrefab.name} x{item.m_stack}");
            found += item.m_stack;
        }

        context.AddString($"ImmersiveTrader items carried: {found}");
    }
}
