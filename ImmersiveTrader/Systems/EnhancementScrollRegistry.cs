using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

internal static class EnhancementScrollRegistry
{
    internal const string Prefix = "ImmersiveTrader_EnhancementScroll_";
    private static readonly Dictionary<string, string> PrefabToEffect = new(StringComparer.OrdinalIgnoreCase);
    private static Texture2D? InventoryAtlas;
    private static readonly Dictionary<string, int> IconIndex = new(StringComparer.OrdinalIgnoreCase)
    {
        ["embers"]=0, ["frost"]=1, ["storm"]=2, ["venom"]=3, ["spirit"]=4,
        ["lumberjack"]=5, ["miner"]=6, ["burden"]=7, ["vitality"]=8, ["endurance"]=9,
        ["focus"]=10, ["craftsman"]=11, ["wanderer"]=12, ["pathfinder"]=13, ["hunter"]=14, ["rested"]=15
    };

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

            Sprite? icon = LoadInventoryIcon(id);
            if (icon != null) shared.m_icons = new[] { icon };
            shared.m_name = $"Scroll of {EnhancementStatusRegistry.DisplayName(id)}";
            shared.m_description = Descriptions[id];
            shared.m_weight = 0.1f;
            shared.m_maxStackSize = 1;
            shared.m_teleportable = true;
            shared.m_itemType = ItemDrop.ItemData.ItemType.Consumable;
            shared.m_consumeStatusEffect = EnhancementStatusRegistry.Template(id);

            // Temporary shared 3D model. It intentionally matches hunting contracts for now.
            ContractWorldModel.Attach(custom.ItemPrefab, icon);
            ItemManager.Instance.AddItem(custom);
            PrefabToEffect[prefabName] = id;
        }

        Plugin.Log.LogInfo($"Enhancement scrolls registered: {PrefabToEffect.Count}.");
    }


    private static Sprite? LoadInventoryIcon(string id)
    {
        if (!IconIndex.TryGetValue(id, out int index)) return null;
        InventoryAtlas ??= LoadTexture("ImmersiveTrader.Assets.EnhancementScrollInventoryIcons.png");
        if (InventoryAtlas == null || InventoryAtlas.width < (index + 1) * 128 || InventoryAtlas.height < 128) return null;
        var sprite = Sprite.Create(InventoryAtlas, new Rect(index * 128, 0, 128, 128), new Vector2(0.5f, 0.5f));
        sprite.name = "ImmersiveTrader_ScrollInventoryIcon_" + id;
        return sprite;
    }

    private static Texture2D? LoadTexture(string resource)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
        if (stream == null) { Plugin.Log.LogWarning("Enhancement scroll inventory icon atlas missing."); return null; }
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(memory.ToArray())) return null;
        texture.filterMode = FilterMode.Bilinear;
        return texture;
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
