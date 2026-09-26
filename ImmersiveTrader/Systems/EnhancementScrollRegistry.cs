using System;
using System.Collections.Generic;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

internal static class EnhancementScrollRegistry
{
    internal const string Prefix = "ImmersiveTrader_EnhancementScroll_";
    private static readonly Dictionary<string, string> PrefabToEffect = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, string> Descriptions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["embers"] = "Enhancement Scroll: Embers\nAdds 5-10 Fire damage to every hit for 30 minutes.",
        ["frost"] = "Enhancement Scroll: Frost\nAdds 5-10 Frost damage to every hit for 30 minutes.",
        ["storm"] = "Enhancement Scroll: Storm\nAdds 5-10 Lightning damage to every hit for 30 minutes.",
        ["venom"] = "Enhancement Scroll: Venom\nAdds 5-10 Poison damage to every hit for 30 minutes.",
        ["spirit"] = "Enhancement Scroll: Spirit\nAdds 5-10 Spirit damage to every hit for 30 minutes.",
        ["lumberjack"] = "Enhancement Scroll: Lumberjack\nIncreases Chop damage by 20% for 30 minutes.",
        ["miner"] = "Enhancement Scroll: Miner\nIncreases Pickaxe damage by 20% for 30 minutes.",
        ["burden"] = "Enhancement Scroll: Burden\nIncreases maximum carry weight by 10% for 30 minutes.",
        ["vitality"] = "Enhancement Scroll: Vitality\nIncreases maximum Health by 10% for 30 minutes. The bonus scales dynamically with your current food-based maximum Health.",
        ["endurance"] = "Enhancement Scroll: Endurance\nIncreases Stamina regeneration by 20% and reduces melee attack Stamina cost by 10% for 30 minutes.",
        ["focus"] = "Enhancement Scroll: Focus\nIncreases maximum Eitr by 15% for 30 minutes. The bonus scales dynamically with your current food-based maximum Eitr.",
        ["craftsman"] = "Enhancement Scroll: Craftsman\nReduces Stamina used by tools by 20% for 30 minutes.",
        ["wanderer"] = "Enhancement Scroll: Wanderer\nReduces running Stamina drain by 15% for 30 minutes.",
        ["pathfinder"] = "Enhancement Scroll: Pathfinder\nReduces jumping Stamina cost by 15% for 30 minutes.",
        ["hunter"] = "Enhancement Scroll: Hunter\nReduces bow Stamina cost by 10% for 30 minutes.",
        ["rested"] = "Enhancement Scroll: Rested\nGrants the vanilla Rested effect for exactly 20 minutes."
    };

    internal static void Register()
    {
        var source = PrefabManager.Instance.GetPrefab("Coins");
        if (source == null) { Plugin.Log.LogWarning("Enhancement scroll base prefab missing: Coins"); return; }

        PrefabToEffect.Clear();
        foreach (string id in EnhancementStatusRegistry.Ids)
        {
            string prefabName = Prefix + id;
            var custom = new CustomItem(prefabName, source);
            var shared = custom.ItemDrop.m_itemData.m_shared;

            Sprite? icon = id.Equals("rested", StringComparison.OrdinalIgnoreCase)
                ? ObjectDB.instance?.GetStatusEffect("Rested".GetStableHashCode())?.m_icon
                : EnhancementStatusRegistry.Template(id)?.m_icon;

            if (icon != null) shared.m_icons = new[] { icon };
            shared.m_name = $"Scroll of {EnhancementStatusRegistry.DisplayName(id)}";
            shared.m_description = Descriptions[id];
            shared.m_weight = 0.1f;
            shared.m_maxStackSize = 10;
            shared.m_teleportable = true;

            // Temporary shared 3D model. It intentionally matches hunting contracts for now.
            ContractWorldModel.Attach(custom.ItemPrefab, icon);
            ItemManager.Instance.AddItem(custom);
            PrefabToEffect[prefabName] = id;
        }

        Plugin.Log.LogInfo($"Enhancement scrolls registered: {PrefabToEffect.Count}.");
    }

    internal static bool TryGetEffect(ItemDrop.ItemData item, out string effect)
    {
        effect = string.Empty;
        string? name = item.m_dropPrefab?.name;
        if (string.IsNullOrEmpty(name)) return false;
        int clone = name.IndexOf("(Clone)", StringComparison.Ordinal);
        if (clone >= 0) name = name.Substring(0, clone);
        return PrefabToEffect.TryGetValue(name, out effect!);
    }
}

[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UseItem))]
internal static class EnhancementScrollUsePatch
{
    private static bool Prefix(Humanoid __instance, Inventory inventory, ItemDrop.ItemData item, bool fromInventoryGui, ref bool __result)
    {
        if (__instance is not Player player || !EnhancementScrollRegistry.TryGetEffect(item, out string id))
            return true;

        float duration = id.Equals("rested", StringComparison.OrdinalIgnoreCase) ? 1200f : EnhancementStatusRegistry.DefaultDuration;
        if (!EnhancementStatusRegistry.Apply(player, id, duration, out string message))
        {
            player.Message(MessageHud.MessageType.Center, message);
            __result = false;
            return false;
        }

        inventory.RemoveOneItem(item);
        player.Message(MessageHud.MessageType.Center, message);
        __result = true;
        return false;
    }
}
