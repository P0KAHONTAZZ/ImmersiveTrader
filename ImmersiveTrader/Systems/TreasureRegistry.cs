using System.Collections.Generic;
using System.Linq;
using ImmersiveTrader.Models;
using Jotunn.Entities;
using Jotunn.Managers;

namespace ImmersiveTrader;

/// <summary>Physical cargo sold by traders. Base prefab controls the real Valheim icon/model;
/// the cloned item is then made heavy, unique and non-teleportable.</summary>
public static class TreasureRegistry
{
    public static readonly IReadOnlyList<TreasureDefinition> Treasures = new List<TreasureDefinition>
    {
        // Meadows - Midka
        new("midka_field_medicine","Field Medicine Crate","YmirRemains",45f),
        new("midka_healing_honey","Healing Honey Barrel","BarleyWineBase",55f),
        new("midka_bandages","Bandage Bundle","YmirRemains",40f),
        new("midka_dried_herbs","Dried Berries and Herbs","YmirRemains",50f),
        new("midka_medical_resin","Medicinal Resin Crate","YmirRemains",60f),
        // Meadows - Troldad
        new("troldad_corewood","Core Wood Bundle","YmirRemains",70f),
        new("troldad_troll_hides","Troll Hide Crate","YmirRemains",65f),
        new("troldad_meat","Meat Barrel","BarleyWineBase",60f),
        new("troldad_wood","Timber Bundle","YmirRemains",80f),
        new("troldad_amber","Amber Crate","YmirRemains",45f),
        // Black Forest
        new("grimvald_copper","Copper Crate","YmirRemains",80f),
        new("grimvald_tin","Tin Crate","YmirRemains",70f),
        new("grimvald_amber","Amber Sack","YmirRemains",45f),
        new("grimvald_cores","Surtling Core Bundle","YmirRemains",50f),
        new("grimvald_resin","Resin Barrel","BarleyWineBase",60f),
        new("rudy_arrows","Arrow Bundle","YmirRemains",45f),
        new("rudy_deer_hides","Deer Hide Crate","YmirRemains",55f),
        new("rudy_honey","Honey Barrel","BarleyWineBase",60f),
        new("rudy_corewood","Core Wood Bundle","YmirRemains",75f),
        new("rudy_rubies","Ruby Crate","YmirRemains",40f),
        // Swamp
        new("mokra_thistle","Thistle Basket","YmirRemains",45f),
        new("mokra_entrails","Entrails Barrel","BarleyWineBase",65f),
        new("mokra_scrap_iron","Scrap Iron Crate","YmirRemains",80f),
        new("mokra_roots","Root Bundle","YmirRemains",60f),
        new("mokra_bloodbags","Bloodbag Crate","YmirRemains",55f),
        new("encek_iron","Iron Crate","YmirRemains",80f),
        new("encek_guck","Guck Barrel","BarleyWineBase",65f),
        new("encek_chains","Chain Bundle","YmirRemains",55f),
        new("encek_sausages","Sausage Barrel","BarleyWineBase",60f),
        new("encek_bark","Ancient Bark Crate","YmirRemains",75f),
        // Mountains
        new("hrothgar_obsidian","Obsidian Crate","YmirRemains",75f),
        new("hrothgar_silver","Silver Bundle","YmirRemains",80f),
        new("hrothgar_wolf_meat","Wolf Meat Barrel","BarleyWineBase",60f),
        new("hrothgar_crystal","Crystal Crate","YmirRemains",55f),
        new("hrothgar_wolf_pelts","Wolf Pelt Bundle","YmirRemains",50f),
        new("ylva_onions","Onion Crate","YmirRemains",50f),
        new("ylva_onion_seeds","Onion Seed Bundle","YmirRemains",40f),
        new("ylva_frost_mead","Frost Resistance Mead Barrel","BarleyWineBase",55f),
        new("ylva_silver","Silver Crate","YmirRemains",80f),
        new("ylva_crystal","Crystal Bundle","YmirRemains",60f),
        // Plains
        new("bjarki_barley","Barley Barrel","BarleyWineBase",70f),
        new("bjarki_flax","Flax Crate","YmirRemains",65f),
        new("bjarki_cloudberries","Cloudberry Basket","YmirRemains",50f),
        new("bjarki_blackmetal","Black Metal Bundle","YmirRemains",80f),
        new("bjarki_coins","Coin Crate","YmirRemains",45f),
        new("ragnar_barley","Barley Barrel","BarleyWineBase",75f),
        new("ragnar_flour","Barley Flour Crate","YmirRemains",60f),
        new("ragnar_flax","Flax Bundle","YmirRemains",65f),
        new("ragnar_blackmetal","Black Metal Crate","YmirRemains",80f),
        new("ragnar_lox_meat","Lox Meat Barrel","BarleyWineBase",70f),
        // Mistlands
        new("cmok_puffs","Jotun Puff Basket","YmirRemains",50f),
        new("cmok_softtissue","Soft Tissue Crate","YmirRemains",60f),
        new("cmok_yggwood","Yggdrasil Wood Bundle","YmirRemains",75f),
        new("cmok_marble","Black Marble Crate","YmirRemains",80f),
        new("cmok_sap","Sap Container","BarleyWineBase",65f),
        new("grelka_magecaps","Magecap Basket","YmirRemains",50f),
        new("grelka_eitr","Refined Eitr Crate","YmirRemains",55f),
        new("grelka_marble","Black Marble Bundle","YmirRemains",80f),
        new("grelka_sap","Sap Barrel","BarleyWineBase",70f),
        new("grelka_jelly","Royal Jelly Crate","YmirRemains",45f),
        // Ashlands
        new("zenek_grausten","Grausten Crate","YmirRemains",80f),
        new("zenek_ashwood","Ashwood Bundle","YmirRemains",75f),
        new("zenek_flametal","Flametal Crate","YmirRemains",80f),
        new("zenek_smoke","Smoke Bomb Bundle","YmirRemains",45f),
        new("zenek_spicy_food","Spiced Provisions Barrel","BarleyWineBase",60f),
        new("skjold_flametal","Flametal Crate","YmirRemains",80f),
        new("skjold_ashwood","Ashwood Bundle","YmirRemains",75f),
        new("skjold_grausten","Grausten Crate","YmirRemains",80f),
        new("skjold_fortification","Fortification Supplies Bundle","YmirRemains",70f),
        new("skjold_fire_medicine","Fire Resistance Supplies Crate","YmirRemains",50f)
    };

    public static void Register()
    {
        int registered = 0;
        foreach (var def in Treasures)
        {
            if (PrefabManager.Instance.GetPrefab(def.BasePrefabName) == null)
            {
                Plugin.Log.LogWarning($"Cargo package base prefab missing: {def.BasePrefabName} ({def.Id})");
                continue;
            }

            var custom = new CustomItem($"ImmersiveTrader_{def.Id}", def.BasePrefabName);
            var shared = custom.ItemDrop.m_itemData.m_shared;
            var cargoIcon = CargoPresentation.IconFor(def.Id, def.DisplayName);
            if (cargoIcon != null) shared.m_icons = new[] { cargoIcon };
            shared.m_name = def.DisplayName;
            var origin = TraderRegistry.Traders.FirstOrDefault(x => x.Id == GetOwnerTraderId(def.Id));
            shared.m_description = $"Sealed shipment. Deliver it to another trader. Cannot pass through portals.\nOrigin: <color=yellow>{origin?.Name ?? GetOwnerTraderId(def.Id)}</color>";
            shared.m_weight = def.Weight;
            shared.m_teleportable = false;
            shared.m_maxStackSize = 1;
            CargoPresentation.AttachWorldCrate(custom.ItemPrefab, def.Id);
            ItemManager.Instance.AddItem(custom);
            registered++;
        }
        Plugin.Log.LogInfo($"Cargo items registered: {registered}/{Treasures.Count}");
    }
    public static string GetOwnerTraderId(string treasureId)
    {
        if (treasureId.StartsWith("rudy_")) return "rudy_warg";
        if (treasureId.StartsWith("mokra_")) return "mokra_dzika";
        if (treasureId.StartsWith("ylva_")) return "ylva_frost";
        if (treasureId.StartsWith("bjarki_")) return "bjarki_goldtooth";
        if (treasureId.StartsWith("ragnar_")) return "ragnar_turnipson";
        if (treasureId.StartsWith("zenek_")) return "spalony_zenek";
        if (treasureId.StartsWith("skjold_")) return "skjold_cinderborn";

        int separator = treasureId.IndexOf('_');
        return separator > 0 ? treasureId.Substring(0, separator) : string.Empty;
    }

    public static IEnumerable<TreasureDefinition> GetForTrader(string traderId)
        => Treasures.Where(x => GetOwnerTraderId(x.Id) == traderId);

}
