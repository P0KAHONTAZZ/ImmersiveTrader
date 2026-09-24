using System.Collections.Generic;
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
        new("midka_field_medicine","Skrzynia leków polowych","BarleyWineBase",45f),
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
        new("grimvald_copper","Skrzynia miedzi","Copper",80f),
        new("grimvald_tin","Skrzynia cyny","Tin",70f),
        new("grimvald_amber","Worek bursztynu","Amber",45f),
        new("grimvald_cores","Paczka rdzeni Surtlinga","SurtlingCore",50f),
        new("grimvald_resin","Beczka żywicy","Resin",60f),
        new("rudy_arrows","Paczka strzał","ArrowFire",45f),
        new("rudy_deer_hides","Skrzynia skór jelenia","DeerHide",55f),
        new("rudy_honey","Beczka miodu","Honey",60f),
        new("rudy_corewood","Wiązka drewna rdzeniowego","RoundLog",75f),
        new("rudy_rubies","Skrzynia rubinów","Ruby",40f),
        // Swamp
        new("mokra_thistle","Kosz ostu","Thistle",45f),
        new("mokra_entrails","Beczka wnętrzności","Entrails",65f),
        new("mokra_scrap_iron","Skrzynia żelaznego złomu","IronScrap",80f),
        new("mokra_roots","Paczka korzeni","Root",60f),
        new("mokra_bloodbags","Skrzynia krwawych worków","Bloodbag",55f),
        new("encek_iron","Skrzynia żelaza","Iron",80f),
        new("encek_guck","Beczka gucku","Guck",65f),
        new("encek_chains","Paczka łańcuchów","Chain",55f),
        new("encek_sausages","Beczka kiełbas","Sausages",60f),
        new("encek_bark","Skrzynia starożytnej kory","ElderBark",75f),
        // Mountains
        new("hrothgar_obsidian","Skrzynia obsydianu","Obsidian",75f),
        new("hrothgar_silver","Paczka srebra","Silver",80f),
        new("hrothgar_wolf_meat","Beczka mięsa wilka","WolfMeat",60f),
        new("hrothgar_crystal","Skrzynia kryształów","Crystal",55f),
        new("hrothgar_wolf_pelts","Paczka skór wilka","WolfPelt",50f),
        new("ylva_onions","Skrzynia cebuli","Onion",50f),
        new("ylva_onion_seeds","Paczka nasion cebuli","OnionSeeds",40f),
        new("ylva_frost_mead","Beczka miodu mrozoodpornego","MeadFrostResist",55f),
        new("ylva_silver","Skrzynia srebra","Silver",80f),
        new("ylva_crystal","Paczka kryształów","Crystal",60f),
        // Plains
        new("bjarki_barley","Beczka jęczmienia","Barley",70f),
        new("bjarki_flax","Skrzynia lnu","Flax",65f),
        new("bjarki_cloudberries","Kosz moroszek","Cloudberry",50f),
        new("bjarki_blackmetal","Paczka czarnego metalu","BlackMetalScrap",80f),
        new("bjarki_coins","Skrzynia monet","Coins",45f),
        new("ragnar_barley","Beczka jęczmienia","Barley",75f),
        new("ragnar_flour","Skrzynia mąki jęczmiennej","BarleyFlour",60f),
        new("ragnar_flax","Paczka lnu","Flax",65f),
        new("ragnar_blackmetal","Skrzynia czarnego metalu","BlackMetalScrap",80f),
        new("ragnar_lox_meat","Beczka mięsa Loxa","LoxMeat",70f),
        // Mistlands
        new("cmok_puffs","Kosz Jotun Puffs","MushroomJotunPuffs",50f),
        new("cmok_softtissue","Skrzynia miękkiej tkanki","Softtissue",60f),
        new("cmok_yggwood","Paczka Yggdrasil Wood","YggdrasilWood",75f),
        new("cmok_marble","Skrzynia czarnego marmuru","BlackMarble",80f),
        new("cmok_sap","Pojemnik z sapem","Sap",65f),
        new("grelka_magecaps","Kosz Magecaps","MushroomMagecap",50f),
        new("grelka_eitr","Skrzynia Eitr","Eitr",55f),
        new("grelka_marble","Paczka czarnego marmuru","BlackMarble",80f),
        new("grelka_sap","Beczka sapu","Sap",70f),
        new("grelka_jelly","Skrzynia królewskiej galaretki","RoyalJelly",45f),
        // Ashlands
        new("zenek_grausten","Skrzynia Grausten","Grausten",80f),
        new("zenek_ashwood","Wiązka Ashwood","Ashwood",75f),
        new("zenek_flametal","Skrzynia Flametalu","FlametalOre",80f),
        new("zenek_smoke","Paczka dymnych bomb","SmokeBomb",45f),
        new("zenek_spicy_food","Beczka pikantnej żywności","SpicyMarmalade",60f),
        new("skjold_flametal","Skrzynia Flametalu","FlametalOre",80f),
        new("skjold_ashwood","Paczka Ashwood","Ashwood",75f),
        new("skjold_grausten","Skrzynia Grausten","Grausten",80f),
        new("skjold_fortification","Paczka materiałów fortyfikacyjnych","Grausten",70f),
        new("skjold_fire_medicine","Skrzynia leków ognioodpornych","MeadFireResist",50f)
    };

    public static void Register()
    {
        foreach (var def in Treasures)
        {
            var custom = new CustomItem($"ImmersiveTrader_{def.Id}", def.BasePrefabName);
            var shared = custom.ItemDrop.m_itemData.m_shared;
            shared.m_name = def.DisplayName;
            shared.m_description = $"Zapieczętowany ładunek: {def.DisplayName}. Towar transportowy, nie pojedynczy surowiec. Dostarcz go innemu handlarzowi. Nie przechodzi przez portale.";
            shared.m_weight = def.Weight;
            shared.m_teleportable = false;
            shared.m_maxStackSize = 1;
            ItemManager.Instance.AddItem(custom);
        }
    }
}
