using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class QuestIssuing
{
    public static bool TryGiveTreasure(Player player, TraderDefinition source)
    {
        long playerId = player.GetPlayerID();
        if (!QuestState.CanTake(playerId))
            return false;

        var options = TreasureRegistry.Treasures.ToArray();
        if (options.Length == 0) return false;

        var def = options[Random.Range(0, options.Length)];
        string prefabName = $"ImmersiveTrader_{def.Id}";
        var prefab = ObjectDB.instance.GetItemPrefab(prefabName);
        if (prefab == null)
        {
            player.Message(MessageHud.MessageType.Center, $"{source.Name}: My shipment is not ready.");
            return false;
        }

        var inventory = player.GetInventory();
        if (!inventory.CanAddItem(prefab, 1))
        {
            player.Message(MessageHud.MessageType.Center, $"{source.Name}: You cannot carry this shipment.");
            return false;
        }

        var item = inventory.AddItem(prefab, 1);
        if (item == null) return false;

        QuestState.Add(playerId, new QuestState.ActiveTreasure(def.Id, source.Id, source.BiomeTier));
        player.Message(MessageHud.MessageType.Center,
            $"{source.Name}: Take {def.DisplayName}. Deliver it to another trader.");
        return true;
    }
}
