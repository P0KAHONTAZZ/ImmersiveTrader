using System.Collections.Generic;
using System.Linq;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class TraderActivityRegistry
{
    public static readonly IReadOnlyList<TraderActivityDefinition> Activities = new List<TraderActivityDefinition>
    {
        // One fixed difficulty per contract. Existing issued scrolls retain their stored targets.
        H("midka","midka_1","Shaman's Whisper in the Woods","Greydwarf_Shaman",30,Skills.SkillType.Knives,2f,"Defeat 30 Greydwarf Shamans."),
        H("midka","midka_2","Brutes on the Road","Greydwarf_Elite",30,Skills.SkillType.Cooking,2f,"Defeat 30 Greydwarf Brutes."),
        H("midka","midka_3","The Troll Beneath Midka's Bridge","Troll",15,Skills.SkillType.Crafting,2f,"Defeat 15 Trolls."),
        H("midka","midka_4","The Great Troll Hunt","Troll",15,Skills.SkillType.Run,2f,"Defeat 15 Trolls."),
        H("midka","midka_5","The Last Brute at the Infirmary","Greydwarf_Elite",30,Skills.SkillType.Blocking,2f,"Defeat 30 Greydwarf Brutes."),
        H("troldad","troldad_1","Shaman in the Lumberwood","Greydwarf_Shaman",30,Skills.SkillType.WoodCutting,2f,"Defeat 30 Greydwarf Shamans."),
        H("troldad","troldad_2","The Brute at the Logging Camp","Greydwarf_Elite",30,Skills.SkillType.Axes,2f,"Defeat 30 Greydwarf Brutes."),
        H("troldad","troldad_3","Troll Among the Pines","Troll",15,Skills.SkillType.Bows,2f,"Defeat 15 Trolls."),
        H("troldad","troldad_4","Hunt for the Forest Giant","Troll",15,Skills.SkillType.Clubs,2f,"Defeat 15 Trolls."),
        H("troldad","troldad_5","The Lumberjacks' Revenge","Greydwarf_Elite",30,Skills.SkillType.Sneak,2f,"Defeat 30 Greydwarf Brutes."),
        H("grimvald","grimvald_1","Shaman at the Copper Vein","Greydwarf_Shaman",30,Skills.SkillType.Pickaxes,2f,"Defeat 30 Greydwarf Shamans."),
        H("grimvald","grimvald_2","Brute in Grimvald's Mine","Greydwarf_Elite",30,Skills.SkillType.WoodCutting,2f,"Defeat 30 Greydwarf Brutes."),
        H("grimvald","grimvald_3","Troll Beneath the Ore Ridge","Troll",15,Skills.SkillType.Crafting,2f,"Defeat 15 Trolls."),
        H("grimvald","grimvald_4","The Troll's Shadow over the Forge","Troll",15,Skills.SkillType.Clubs,2f,"Defeat 15 Trolls."),
        H("grimvald","grimvald_5","Guardian of Copper and Tin","Greydwarf_Elite",30,Skills.SkillType.Blocking,2f,"Defeat 30 Greydwarf Brutes."),
        H("rudy_warg","rudy_1","Shaman on Warg's Trail","Greydwarf_Shaman",30,Skills.SkillType.Bows,2f,"Defeat 30 Greydwarf Shamans."),
        H("rudy_warg","rudy_2","Brute in the Deer Grounds","Greydwarf_Elite",30,Skills.SkillType.Spears,2f,"Defeat 30 Greydwarf Brutes."),
        H("rudy_warg","rudy_3","Troll on the Hunter's Trail","Troll",15,Skills.SkillType.Sneak,2f,"Defeat 15 Trolls."),
        H("rudy_warg","rudy_4","Warg and the Mountain Troll","Troll",15,Skills.SkillType.Crossbows,2f,"Defeat 15 Trolls."),
        H("rudy_warg","rudy_5","The Brutes' Last Haul","Greydwarf_Elite",30,Skills.SkillType.Run,2f,"Defeat 30 Greydwarf Brutes."),
        H("mokra_dzika","dzika_1","Wraith over Black Water","Wraith",15,Skills.SkillType.Farming,2f,"Defeat 15 Wraiths."),
        H("mokra_dzika","dzika_2","Elite Blob in the Bog","BlobElite",30,Skills.SkillType.Cooking,2f,"Defeat 30 Elite Blobs."),
        H("mokra_dzika","dzika_3","Abomination in the Drowned Woods","Abomination",10,Skills.SkillType.Swim,2f,"Defeat 10 Abominations."),
        H("mokra_dzika","dzika_4","Night Hunt for a Wraith","Wraith",15,Skills.SkillType.Spears,2f,"Defeat 15 Wraiths."),
        H("mokra_dzika","dzika_5","The Muddy Blob Nest","BlobElite",30,Skills.SkillType.Clubs,2f,"Defeat 30 Elite Blobs."),
        H("encek","encek_1","Wraith on the Iron Road","Wraith",15,Skills.SkillType.Fishing,2f,"Defeat 15 Wraiths."),
        H("encek","encek_2","Blob in the Rusted Crypt","BlobElite",30,Skills.SkillType.Crafting,2f,"Defeat 30 Elite Blobs."),
        H("encek","encek_3","Roots of the Abomination","Abomination",10,Skills.SkillType.Polearms,2f,"Defeat 10 Abominations."),
        H("encek","encek_4","Wraith of the Abandoned Forge","Wraith",15,Skills.SkillType.Blocking,2f,"Defeat 15 Wraiths."),
        H("encek","encek_5","Encek's Swamp Plague","BlobElite",30,Skills.SkillType.Axes,2f,"Defeat 30 Elite Blobs."),
        H("hrothgar","hrothgar_1","Fenring on the Icy Ridge","Fenring",18,Skills.SkillType.Pickaxes,2f,"Defeat 18 Fenrings."),
        H("hrothgar","hrothgar_2","Drakes over the Pass","Hatchling",30,Skills.SkillType.Bows,2f,"Defeat 30 Drakes."),
        H("hrothgar","hrothgar_3","The Silver Guardian","StoneGolem",10,Skills.SkillType.Swords,2f,"Defeat 10 Stone Golems."),
        H("hrothgar","hrothgar_4","Hrothgar's Fenring Night","Fenring",18,Skills.SkillType.Jump,2f,"Defeat 18 Fenrings."),
        H("hrothgar","hrothgar_5","Sky Full of Drakes","Hatchling",30,Skills.SkillType.Ride,2f,"Defeat 30 Drakes."),
        H("ylva_frost","ylva_1","Fenring at Ylva's Gate","Fenring",18,Skills.SkillType.Spears,2f,"Defeat 18 Fenrings."),
        H("ylva_frost","ylva_2","Drake on the Snowy Peak","Hatchling",30,Skills.SkillType.Knives,2f,"Defeat 30 Drakes."),
        H("ylva_frost","ylva_3","Ylva's Stone Golem","StoneGolem",10,Skills.SkillType.Blocking,2f,"Defeat 10 Stone Golems."),
        H("ylva_frost","ylva_4","Trail of the Winter Fenring","Fenring",18,Skills.SkillType.Dodge,2f,"Defeat 18 Fenrings."),
        H("ylva_frost","ylva_5","Dragon Wings in the Frost","Hatchling",30,Skills.SkillType.Cooking,2f,"Defeat 30 Drakes."),
        H("bjarki_goldtooth","bjarki_1","Deathsquitos over Gold","Deathsquito",30,Skills.SkillType.Swords,2f,"Defeat 30 Deathsquitos."),
        H("bjarki_goldtooth","bjarki_2","Fuling Brute at the Market","GoblinBrute",20,Skills.SkillType.Spears,2f,"Defeat 20 Fuling Brutes."),
        H("bjarki_goldtooth","bjarki_3","Bjarki's Plains Lox","Lox",15,Skills.SkillType.Blocking,2f,"Defeat 15 Lox."),
        H("bjarki_goldtooth","bjarki_4","Swarm over the Barley","Deathsquito",30,Skills.SkillType.Polearms,2f,"Defeat 30 Deathsquitos."),
        H("bjarki_goldtooth","bjarki_5","Fuling Raiders on the Gold Road","GoblinBrute",20,Skills.SkillType.Ride,2f,"Defeat 20 Fuling Brutes."),
        H("ragnar_turnipson","ragnar_1","Deathsquito in the Turnips","Deathsquito",30,Skills.SkillType.Farming,2f,"Defeat 30 Deathsquitos."),
        H("ragnar_turnipson","ragnar_2","Fuling Brute at the Mill","GoblinBrute",20,Skills.SkillType.Cooking,2f,"Defeat 20 Fuling Brutes."),
        H("ragnar_turnipson","ragnar_3","Lox in Ragnar's Fields","Lox",15,Skills.SkillType.Axes,2f,"Defeat 15 Lox."),
        H("ragnar_turnipson","ragnar_4","The Last Plains Swarm","Deathsquito",30,Skills.SkillType.Clubs,2f,"Defeat 30 Deathsquitos."),
        H("ragnar_turnipson","ragnar_5","Ragnar and the Fuling Brute","GoblinBrute",20,Skills.SkillType.Run,2f,"Defeat 20 Fuling Brutes."),
        H("cmok","cmok_1","Ticks in the Thick Mist","Tick",30,Skills.SkillType.BloodMagic,2f,"Defeat 30 Ticks."),
        H("cmok","cmok_2","Seeker Brute Beneath the Roots","SeekerBrute",15,Skills.SkillType.Crossbows,2f,"Defeat 15 Seeker Brutes."),
        H("cmok","cmok_3","Gjall over Ćmok's Workshop","Gjall",8,Skills.SkillType.Polearms,2f,"Defeat 8 Gjall."),
        H("cmok","cmok_4","Tick Plague in the Ruins","Tick",30,Skills.SkillType.Swim,2f,"Defeat 30 Ticks."),
        H("cmok","cmok_5","Great Seeker in the Mist","SeekerBrute",15,Skills.SkillType.Jump,2f,"Defeat 15 Seeker Brutes."),
        H("grelka","grelka_1","Tick in Grelka's Mycelium","Tick",30,Skills.SkillType.Swords,2f,"Defeat 30 Ticks."),
        H("grelka","grelka_2","Seeker Brute on the Mage's Road","SeekerBrute",15,Skills.SkillType.ElementalMagic,2f,"Defeat 15 Seeker Brutes."),
        H("grelka","grelka_3","Gjall over Black Marble","Gjall",8,Skills.SkillType.Crafting,2f,"Defeat 8 Gjall."),
        H("grelka","grelka_4","Night of Ticks in the Mistlands","Tick",30,Skills.SkillType.Sneak,2f,"Defeat 30 Ticks."),
        H("grelka","grelka_5","Grelka's Seeker Hunt","SeekerBrute",15,Skills.SkillType.Unarmed,2f,"Defeat 15 Seeker Brutes."),
        H("spalony_zenek","zenek_1","Charred Mage Below the Fort","Charred_Mage",30,Skills.SkillType.Swords,2f,"Defeat 30 Charred Mages."),
        H("spalony_zenek","zenek_2","Valkyrie over Zenek's Camp","FallenValkyrie",8,Skills.SkillType.ElementalMagic,2f,"Defeat 8 Fallen Valkyries."),
        H("spalony_zenek","zenek_3","Morgen in the Ashen Valley","Morgen",10,Skills.SkillType.Crossbows,2f,"Defeat 10 Morgen."),
        H("spalony_zenek","zenek_4","Fire Mage on the Road","Charred_Mage",30,Skills.SkillType.Spears,2f,"Defeat 30 Charred Mages."),
        H("spalony_zenek","zenek_5","The Fallen Valkyrie's Last Flight","FallenValkyrie",8,Skills.SkillType.Run,2f,"Defeat 8 Fallen Valkyries."),
        H("skjold_cinderborn","skjold_1","Mage at Skjold's Walls","Charred_Mage",30,Skills.SkillType.WoodCutting,2f,"Defeat 30 Charred Mages."),
        H("skjold_cinderborn","skjold_2","Valkyrie over the Fortifications","FallenValkyrie",8,Skills.SkillType.Polearms,2f,"Defeat 8 Fallen Valkyries."),
        H("skjold_cinderborn","skjold_3","Morgen at the Bastion","Morgen",10,Skills.SkillType.Blocking,2f,"Defeat 10 Morgen."),
        H("skjold_cinderborn","skjold_4","Charred Mages on Watch","Charred_Mage",30,Skills.SkillType.Crafting,2f,"Defeat 30 Charred Mages."),
        H("skjold_cinderborn","skjold_5","Defense Against a Fallen Valkyrie","FallenValkyrie",8,Skills.SkillType.Dodge,2f,"Defeat 8 Fallen Valkyries."),
    };


    public static void Validate()
    {
        var duplicateIds = Activities.GroupBy(x => x.Id).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
        foreach (string id in duplicateIds)
            Plugin.Log.LogWarning($"Duplicate hunting contract id: {id}");

        var duplicateTitles = Activities.GroupBy(x => x.Title).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
        foreach (string title in duplicateTitles)
            Plugin.Log.LogWarning($"Duplicate hunting contract title: {title}");

        foreach (var trader in TraderRegistry.Traders.Where(x => !x.IsLegendary))
        {
            int count = Activities.Count(x => x.TraderId == trader.Id);
            if (count != 5)
                Plugin.Log.LogWarning($"Hunting contracts for {trader.Id}: expected 5, found {count}");
            int distinctSkills = Activities.Where(x => x.TraderId == trader.Id).Select(x => x.RewardSkill).Distinct().Count();
            if (distinctSkills != count)
                Plugin.Log.LogWarning($"Hunting contracts for {trader.Id}: repeated reward skill");
        }

        int coveredSkills = Activities.Select(x => x.RewardSkill).Distinct().Count();
        if (coveredSkills != 24)
            Plugin.Log.LogWarning($"Hunting contracts cover {coveredSkills}/24 expected skills.");
        Plugin.Log.LogInfo($"Hunting contracts ready: {Activities.Count}; unique ids: {Activities.Select(x => x.Id).Distinct().Count()}; reward skills: {coveredSkills}");
    }

    private static TraderActivityDefinition H(string trader,string id,string title,string target,int count,Skills.SkillType skill,float exp,string text) =>
        new(trader,id,title,TraderActivityType.Hunt,target,count,skill,exp,text);
}
