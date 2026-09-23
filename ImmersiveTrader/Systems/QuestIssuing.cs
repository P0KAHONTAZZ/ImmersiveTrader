using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class QuestIssuing
{
    public static bool TryGiveTreasure(Player player, TraderDefinition source)
    {
        if (InventoryTreasureService.Count(player) >= Plugin.MaxCarriedTreasures.Value)
            return false;

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
            return false;
        }

        var item = inventory.AddItem(prefab, 1);
        if (item == null) return false;

        TreasureMetadata.Stamp(item, source.Id, source.BiomeTier);
        player.Message(MessageHud.MessageType.Center,
            $"{source.Name}: Take {def.DisplayName}. Deliver it to another trader.");
        return true;
    }
}