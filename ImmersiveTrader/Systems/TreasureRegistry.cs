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
        new("midka_field_medicine","Skrzynia leków polowych","YmirRemains",45f),
        new("midka_healing_honey","Beczka miodu leczniczego","BarleyWineBase",55f),
        new("midka_bandages","Paczka bandaży","YmirRemains",40f),
        new("midka_dried_herbs","Suszone jagody i zioła","YmirRemains",50f),
        new("midka_medical_resin","Skrzynia żywicy medycznej","YmirRemains",60f),
        // Meadows - Troldad
        new("troldad_corewood","Paczka drewna rdzeniowego","YmirRemains",70f),
        new("troldad_troll_hides","Skrzynia skór trolla","YmirRemains",65f),
        new("troldad_meat","Beczka mięsa","BarleyWineBase",60f),
        new("troldad_wood","Wiązka drewna","YmirRemains",80f),
        new("troldad_amber","Skrzynia bursztynu","YmirRemains",45f),
        // Black Forest
        new("grimvald_copper","Skrzynia miedzi","YmirRemains",80f),
        new("grimvald_tin","Skrzynia cyny","YmirRemains",70f),
        new("grimvald_amber","Worek bursztynu","YmirRemains",45f),
        new("grimvald_cores","Paczka rdzeni Surtlinga","YmirRemains",50f),
        new("grimvald_resin","Beczka żywicy","BarleyWineBase",60f),
        new("rudy_arrows","Paczka strzał","YmirRemains",45f),
        new("rudy_deer_hides","Skrzynia skór jelenia","YmirRemains",55f),
        new("rudy_honey","Beczka miodu","BarleyWineBase",60f),
        new("rudy_corewood","Wiązka drewna rdzeniowego","YmirRemains",75f),
        new("rudy_rubies","Skrzynia rubinów","YmirRemains",40f),
        // Swamp
        new("mokra_thistle","Kosz ostu","YmirRemains",45f),
        new("mokra_entrails","Beczka wnętrzności","BarleyWineBase",65f),
        new("mokra_scrap_iron","Skrzynia żelaznego złomu","YmirRemains",80f),
        new("mokra_roots","Paczka korzeni","YmirRemains",60f),
        new("mokra_bloodbags","Skrzynia krwawych worków","YmirRemains",55f),
        new("encek_iron","Skrzynia żelaza","YmirRemains",80f),
        new("encek_guck","Beczka gucku","BarleyWineBase",65f),
        new("encek_chains","Paczka łańcuchów","YmirRemains",55f),
        new("encek_sausages","Beczka kiełbas","BarleyWineBase",60f),
        new("encek_bark","Skrzynia starożytnej kory","YmirRemains",75f),
        // Mountains
        new("hrothgar_obsidian","Skrzynia obsydianu","YmirRemains",75f),
        new("hrothgar_silver","Paczka srebra","YmirRemains",80f),
        new("hrothgar_wolf_meat","Beczka mięsa wilka","BarleyWineBase",60f),
        new("hrothgar_crystal","Skrzynia kryształów","YmirRemains",55f),
        new("hrothgar_wolf_pelts","Paczka skór wilka","YmirRemains",50f),
        new("ylva_onions","Skrzynia cebuli","YmirRemains",50f),
        new("ylva_onion_seeds","Paczka nasion cebuli","YmirRemains",40f),
        new("ylva_frost_mead","Beczka miodu mrozoodpornego","BarleyWineBase",55f),
        new("ylva_silver","Skrzynia srebra","YmirRemains",80f),
        new("ylva_crystal","Paczka kryształów","YmirRemains",60f),
        // Plains
        new("bjarki_barley","Beczka jęczmienia","BarleyWineBase",70f),
        new("bjarki_flax","Skrzynia lnu","YmirRemains",65f),
        new("bjarki_cloudberries","Kosz moroszek","YmirRemains",50f),
        new("bjarki_blackmetal","Paczka czarnego metalu","YmirRemains",80f),
        new("bjarki_coins","Skrzynia monet","YmirRemains",45f),
        new("ragnar_barley","Beczka jęczmienia","BarleyWineBase",75f),
        new("ragnar_flour","Skrzynia mąki jęczmiennej","YmirRemains",60f),
        new("ragnar_flax","Paczka lnu","YmirRemains",65f),
        new("ragnar_blackmetal","Skrzynia czarnego metalu","YmirRemains",80f),
        new("ragnar_lox_meat","Beczka mięsa Loxa","BarleyWineBase",70f),
        // Mistlands
        new("cmok_puffs","Kosz Jotun Puffs","YmirRemains",50f),
        new("cmok_softtissue","Skrzynia miękkiej tkanki","YmirRemains",60f),
        new("cmok_yggwood","Paczka Yggdrasil Wood","YmirRemains",75f),
        new("cmok_marble","Skrzynia czarnego marmuru","YmirRemains",80f),
        new("cmok_sap","Pojemnik z sapem","BarleyWineBase",65f),
        new("grelka_magecaps","Kosz Magecaps","YmirRemains",50f),
        new("grelka_eitr","Skrzynia Eitr","YmirRemains",55f),
        new("grelka_marble","Paczka czarnego marmuru","YmirRemains",80f),
        new("grelka_sap","Beczka sapu","BarleyWineBase",70f),
        new("grelka_jelly","Skrzynia królewskiej galaretki","YmirRemains",45f),
        // Ashlands
        new("zenek_grausten","Skrzynia Grausten","YmirRemains",80f),
        new("zenek_ashwood","Wiązka Ashwood","YmirRemains",75f),
        new("zenek_flametal","Skrzynia Flametalu","YmirRemains",80f),
        new("zenek_smoke","Paczka dymnych bomb","YmirRemains",45f),
        new("zenek_spicy_food","Beczka pikantnej żywności","BarleyWineBase",60f),
        new("skjold_flametal","Skrzynia Flametalu","YmirRemains",80f),
        new("skjold_ashwood","Paczka Ashwood","YmirRemains",75f),
        new("skjold_grausten","Skrzynia Grausten","YmirRemains",80f),
        new("skjold_fortification","Paczka materiałów fortyfikacyjnych","YmirRemains",70f),
        new("skjold_fire_medicine","Skrzynia leków ognioodpornych","YmirRemains",50f)
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
            shared.m_name = def.DisplayName;
            shared.m_description = $"Zapieczętowany ładunek: {def.DisplayName}. Towar transportowy, nie pojedynczy surowiec. Dostarcz go innemu handlarzowi. Nie przechodzi przez portale.";
            shared.m_weight = def.Weight;
            shared.m_teleportable = false;
            shared.m_maxStackSize = 1;
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
