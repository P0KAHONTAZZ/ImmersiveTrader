using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader;

public static class QuestIssuing
{
    public static bool TryGiveTreasure(Player player, TraderDefinition source, Vector3 sourcePosition)
    {
        if (source.IsLegendary) return false;

        if (InventoryTreasureService.Count(player) >= Plugin.MaxCarriedTreasures.Value)
            return false;

        if (!TraderCooldown.CanIssue(player, source.Id, out int remaining))
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

        var before = inventory.GetAllItems()
            .Where(x => x.m_dropPrefab != null && x.m_dropPrefab.name.StartsWith(prefabName))
            .ToList();

        if (!inventory.AddItem(prefab, 1)) return false;

        var item = inventory.GetAllItems().LastOrDefault(x =>
            x.m_dropPrefab != null &&
            x.m_dropPrefab.name.StartsWith(prefabName) &&
            !before.Contains(x));

        if (item == null)
        {
            player.Message(MessageHud.MessageType.Center, $"{source.Name}: Shipment creation failed safely.");
            return true;
        }

        TreasureMetadata.Stamp(item, source.Id, source.BiomeTier, sourcePosition.x, sourcePosition.z);
        TraderCooldown.MarkIssued(player, source.Id);
        player.Message(MessageHud.MessageType.Center,
            $"{source.Name}: Take {def.DisplayName}. Deliver it to another trader.");
        return true;
    }

    public static bool TryBuyTreasure(Player player, TraderDefinition source, Vector3 sourcePosition, string treasureId, int price)
    {
        if (source.IsLegendary) return false;
        var def = TreasureRegistry.Treasures.FirstOrDefault(x => x.Id == treasureId);
        if (def == null) return false;

        string prefabName = $"ImmersiveTrader_{def.Id}";
        var prefab = ObjectDB.instance.GetItemPrefab(prefabName);
        var coins = ObjectDB.instance.GetItemPrefab("Coins");
        if (prefab == null || coins == null) return false;

        var inventory = player.GetInventory();
        if (inventory.CountItems(coins.GetComponent<ItemDrop>().m_itemData.m_shared.m_name) < price)
        {
            player.Message(MessageHud.MessageType.Center, $"{source.Name}: This shipment costs {price} coins.");
            return true;
        }
        if (!inventory.CanAddItem(prefab, 1))
        {
            player.Message(MessageHud.MessageType.Center, $"{source.Name}: You cannot carry this shipment.");
            return true;
        }

        inventory.RemoveItem(coins.GetComponent<ItemDrop>().m_itemData.m_shared.m_name, price);
        var before = inventory.GetAllItems().ToList();
        if (!inventory.AddItem(prefab, 1)) return true;
        var item = inventory.GetAllItems().LastOrDefault(x => x.m_dropPrefab != null && x.m_dropPrefab.name.StartsWith(prefabName) && !before.Contains(x));
        if (item == null) return true;

        TreasureMetadata.Stamp(item, source.Id, source.BiomeTier, sourcePosition.x, sourcePosition.z);
        player.Message(MessageHud.MessageType.Center, $"{source.Name}: Deliver {def.DisplayName} to the other trader.");
        return true;
    }
}
