using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Physical enhancement scrolls (v2, built from 3c8560e).
/// Uses Valheim's native Consumable flow: using the item consumes exactly one scroll
/// and applies m_consumeStatusEffect. No UseItem/ConsumeItem patches.
/// Independent of the contract system; only the static ContractWorldModel builder is reused
/// for the temporary ground model.
/// </summary>
internal static class EnhancementScrollItems
{
    internal const string PrefabPrefix = "ImmersiveTrader_EnhancementScroll_";
    private const string AtlasResource = "ImmersiveTrader.Assets.EnhancementScrollInventoryIcons.png";
    private const int Cell = 128;

    // Order matches Assets/enhancement-scroll-inventory-icons.png (16 cells).
    private static readonly string[] AtlasOrder =
    {
        "embers","frost","storm","venom","spirit","lumberjack","miner","burden",
        "vitality","endurance","focus","craftsman","wanderer","pathfinder","hunter","rested"
    };

    // Stage 1: only Embers. Add ids here once Embers is verified in game.
    internal static readonly string[] Enabled = { "embers" };

    private static Texture2D? atlas;
    private static bool atlasLoaded;
    private static readonly HashSet<string> Registered = new(StringComparer.OrdinalIgnoreCase);

    internal static string PrefabName(string id) => PrefabPrefix + id.ToLowerInvariant();
    internal static bool IsRegistered(string id) => Registered.Contains(id);
    internal static IEnumerable<string> RegisteredIds => Registered;

    internal static void Register()
    {
        var source = PrefabManager.Instance.GetPrefab("Coins");
        if (source == null) { Plugin.Log.LogError("Enhancement scrolls: base prefab Coins missing; skipping."); return; }

        foreach (string id in Enabled)
        {
            try { RegisterOne(id, source); }
            catch (Exception e) { Plugin.Log.LogError($"Enhancement scroll '{id}' registration failed: {e}"); }
        }
        Plugin.Log.LogInfo($"Enhancement scrolls registered: {Registered.Count}/{Enabled.Length} ({string.Join(", ", Registered)}).");
    }

    private static void RegisterOne(string id, GameObject source)
    {
        id = id.ToLowerInvariant();
        if (id == "rested") { Plugin.Log.LogWarning("Scroll of Rested is not part of stage 1."); return; }

        var status = EnhancementStatusRegistry.Template(id);
        if (status == null) { Plugin.Log.LogError($"Enhancement scroll '{id}': status effect not registered; skipping."); return; }

        // Icon must never be missing: an empty m_icons array crashes InventoryGrid every frame.
        var icon = InventoryIcon(id) ?? FallbackIcon();
        if (icon == null) { Plugin.Log.LogError($"Enhancement scroll '{id}': no icon available; skipping."); return; }

        var custom = new CustomItem(PrefabName(id), source);
        var drop = custom.ItemDrop;
        if (drop == null) { Plugin.Log.LogError($"Enhancement scroll '{id}': clone has no ItemDrop; skipping."); return; }

        var shared = drop.m_itemData.m_shared;
        shared.m_icons = new[] { icon };
        shared.m_name = "Scroll of " + EnhancementStatusRegistry.DisplayName(id);
        shared.m_description = Description(id);
        shared.m_itemType = ItemDrop.ItemData.ItemType.Consumable;
        shared.m_consumeStatusEffect = status;
        shared.m_food = 0f;
        shared.m_foodStamina = 0f;
        shared.m_foodEitr = 0f;
        shared.m_foodBurnTime = 0f;
        shared.m_foodRegen = 0f;
        shared.m_maxStackSize = 10;
        shared.m_weight = 0.1f;
        shared.m_value = 0;
        shared.m_teleportable = true;
        shared.m_questItem = false;
        shared.m_equipStatusEffect = null;
        shared.m_setStatusEffect = null;
        drop.m_itemData.m_variant = 0;

        // Temporary ground model: same look as contracts (built on this clone only).
        ContractWorldModel.Attach(custom.ItemPrefab, ContractIconRegistry.Parchment);

        if (!ItemManager.Instance.AddItem(custom))
        {
            Plugin.Log.LogError($"Enhancement scroll '{id}': ItemManager rejected the item.");
            return;
        }
        Registered.Add(id);
    }

    private static string Description(string id) => id switch
    {
        "embers" => "Use to gain Embers for 30 minutes: +5-10 Fire damage on hit.",
        _ => "Use to gain this enhancement for 30 minutes."
    };

    private static Sprite? InventoryIcon(string id)
    {
        int index = Array.IndexOf(AtlasOrder, id);
        if (index < 0) return null;
        if (!atlasLoaded) { atlas = LoadAtlas(); atlasLoaded = true; }
        if (atlas == null || atlas.width < (index + 1) * Cell || atlas.height < Cell)
        {
            Plugin.Log.LogWarning($"Enhancement scroll inventory atlas unavailable for '{id}'; using fallback icon.");
            return null;
        }
        var sprite = Sprite.Create(atlas, new Rect(index * Cell, 0, Cell, Cell), new Vector2(0.5f, 0.5f));
        sprite.name = "ImmersiveTrader_ScrollIcon_" + id;
        return sprite;
    }

    private static Sprite? FallbackIcon()
    {
        var parchment = ContractIconRegistry.Parchment;
        if (parchment != null) return parchment;
        var coins = PrefabManager.Instance.GetPrefab("Coins")?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_icons;
        return coins != null && coins.Length > 0 ? coins[0] : null;
    }

    private static Texture2D? LoadAtlas()
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(AtlasResource);
        if (stream == null) { Plugin.Log.LogError("Enhancement scroll inventory atlas missing from DLL: " + AtlasResource); return null; }
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(memory.ToArray())) { Plugin.Log.LogError("Enhancement scroll inventory atlas could not be decoded."); return null; }
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.name = "ImmersiveTrader_EnhancementScrollAtlas";
        return texture;
    }
}
