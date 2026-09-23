using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class QuestIssuing
{
    public static bool TryGiveTreasure(Player player, TraderDefinition source)
    {
        if (source.IsLegendary) return false;

        long playerId = player.GetPlayerID();
        if (InventoryTreasureService.Count(player) >= Plugin.MaxCarriedTreasures.Value)
            return false;

        if (!TraderCooldown.CanIssue(playerId, source.Id, out int remaining))
        {
            player.Message(MessageHud.MessageType.Center,
                $"{source.Name}: Come back in {remaining} world day(s).");
            return true;
        }

        var options = TreasureRegistry.Treasures.ToArray();
        if (options.Length == 0) return false;

        var def = options[Random.Range(0, options.Length)];
        string prefabName = $"ImmersiveTrader_{def.Id}";
        var prefab = ObjectDB.instance.GetItemPrefab(prefabName);
        if (prefab == null) return false;

        var inventory = player.GetInventory();
        if (!inventory.CanAddItem(prefab, 1))
        {
            player.Message(MessageHud.MessageType.Center, $"{source.Name}: You cannot carry this shipment.");
            return true;
        }

        var before = inventory.GetAllItems().Where(x =>\n            x.m_dropPrefab != null && x.m_dropPrefab.name.StartsWith(prefabName)).ToList();\n\n        if (!inventory.AddItem(prefab, 1)) return false;\n\n        var item = inventory.GetAllItems().LastOrDefault(x =>\n            x.m_dropPrefab != null && x.m_dropPrefab.name.StartsWith(prefabName) && !before.Contains(x));\n        if (item == null)\n        {\n            player.Message(MessageHud.MessageType.Center, $"{source.Name}: Shipment creation failed safely.");\n            return true;\n        }\n\n        TreasureMetadata.Stamp(item, source.Id, source.BiomeTier);
        TraderCooldown.MarkIssued(playerId, source.Id);
        player.Message(MessageHud.MessageType.Center,
            $"{source.Name}: Take {def.DisplayName}. Deliver it to another trader.");
        return true;
    }
}