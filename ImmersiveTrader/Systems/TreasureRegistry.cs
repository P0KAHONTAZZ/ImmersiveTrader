using System.Collections.Generic;
using ImmersiveTrader.Models;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

public static class TreasureRegistry
{
    public static readonly IReadOnlyList<TreasureDefinition> Treasures = new List<TreasureDefinition>
    {
        new("ancient_parcel", "Ancient Parcel", "Ruby", 80f),
        new("carved_idol", "Carved Idol", "AmberPearl", 80f),
        new("sealed_mead_cask", "Sealed Mead Cask", "TankardOdin", 80f),
        new("merchants_gem", "Merchant's Gem", "Ruby", 80f),
        new("runic_ledger", "Runic Ledger", "Wishbone", 80f)
    };

    public static void Register()
    {
        foreach (var def in Treasures)
        {
            var custom = new CustomItem($"ImmersiveTrader_{def.Id}", def.BasePrefabName);
            var shared = custom.ItemDrop.m_itemData.m_shared;
            shared.m_name = def.DisplayName;
            shared.m_description = "A heavy trader's treasure. It cannot pass through portals.";
            shared.m_weight = Plugin.TreasureWeight.Value;
            shared.m_teleportable = false;
            shared.m_maxStackSize = 1;

            ItemManager.Instance.AddItem(custom);
        }
    }
}