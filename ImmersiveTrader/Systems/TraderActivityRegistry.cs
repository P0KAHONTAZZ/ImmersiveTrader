using System.Collections.Generic;
using System.Linq;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class TraderActivityRegistry
{
    public static readonly IReadOnlyList<TraderActivityDefinition> Activities = new List<TraderActivityDefinition>
    {
        // The 70 Normal entries mirror the Misje worksheet. Rare rolls halve the
        // required kills while keeping the skill and EXP reward.
        H("midka","midka_1","Kontrakt 1 - Greydwarf Shaman","Greydwarf_Shaman",40,Skills.SkillType.Knives,50f,"Pokonaj 40 x Greydwarf Shaman."),
        H("midka","midka_2","Kontrakt 2 - Greydwarf Brute","Greydwarf_Elite",50,Skills.SkillType.Run,50f,"Pokonaj 50 x Greydwarf Brute."),
        H("midka","midka_3","Kontrakt 3 - Troll","Troll",60,Skills.SkillType.Blocking,100f,"Pokonaj 60 x Troll."),
        H("midka","midka_4","Kontrakt 4 - Troll","Troll",80,Skills.SkillType.Bows,100f,"Pokonaj 80 x Troll."),
        H("midka","midka_5","Kontrakt 5 - Greydwarf Brute","Greydwarf_Elite",100,Skills.SkillType.Jump,150f,"Pokonaj 100 x Greydwarf Brute."),
        H("troldad","troldad_1","Kontrakt 1 - Greydwarf Shaman","Greydwarf_Shaman",40,Skills.SkillType.Run,50f,"Pokonaj 40 x Greydwarf Shaman."),
        H("troldad","troldad_2","Kontrakt 2 - Greydwarf Brute","Greydwarf_Elite",50,Skills.SkillType.Blocking,50f,"Pokonaj 50 x Greydwarf Brute."),
        H("troldad","troldad_3","Kontrakt 3 - Troll","Troll",60,Skills.SkillType.Bows,100f,"Pokonaj 60 x Troll."),
        H("troldad","troldad_4","Kontrakt 4 - Troll","Troll",80,Skills.SkillType.Jump,100f,"Pokonaj 80 x Troll."),
        H("troldad","troldad_5","Kontrakt 5 - Greydwarf Brute","Greydwarf_Elite",100,Skills.SkillType.Sneak,150f,"Pokonaj 100 x Greydwarf Brute."),
        H("grimvald","grimvald_1","Kontrakt 1 - Greydwarf Shaman","Greydwarf_Shaman",40,Skills.SkillType.Axes,100f,"Pokonaj 40 x Greydwarf Shaman."),
        H("grimvald","grimvald_2","Kontrakt 2 - Greydwarf Brute","Greydwarf_Elite",50,Skills.SkillType.Clubs,100f,"Pokonaj 50 x Greydwarf Brute."),
        H("grimvald","grimvald_3","Kontrakt 3 - Troll","Troll",60,Skills.SkillType.Swords,160f,"Pokonaj 60 x Troll."),
        H("grimvald","grimvald_4","Kontrakt 4 - Troll","Troll",80,Skills.SkillType.Sneak,160f,"Pokonaj 80 x Troll."),
        H("grimvald","grimvald_5","Kontrakt 5 - Greydwarf Brute","Greydwarf_Elite",100,Skills.SkillType.Blocking,220f,"Pokonaj 100 x Greydwarf Brute."),
        H("rudy_warg","rudy_1","Kontrakt 1 - Greydwarf Shaman","Greydwarf_Shaman",40,Skills.SkillType.Bows,100f,"Pokonaj 40 x Greydwarf Shaman."),
        H("rudy_warg","rudy_2","Kontrakt 2 - Greydwarf Brute","Greydwarf_Elite",50,Skills.SkillType.Spears,100f,"Pokonaj 50 x Greydwarf Brute."),
        H("rudy_warg","rudy_3","Kontrakt 3 - Troll","Troll",60,Skills.SkillType.Knives,160f,"Pokonaj 60 x Troll."),
        H("rudy_warg","rudy_4","Kontrakt 4 - Troll","Troll",80,Skills.SkillType.Run,160f,"Pokonaj 80 x Troll."),
        H("rudy_warg","rudy_5","Kontrakt 5 - Greydwarf Brute","Greydwarf_Elite",100,Skills.SkillType.Sneak,220f,"Pokonaj 100 x Greydwarf Brute."),
        H("mokra_dzika","dzika_1","Kontrakt 1 - Wraith","Wraith",40,Skills.SkillType.Spears,160f,"Pokonaj 40 x Wraith."),
        H("mokra_dzika","dzika_2","Kontrakt 2 - Blob Elite","BlobElite",50,Skills.SkillType.Swords,160f,"Pokonaj 50 x Blob Elite."),
        H("mokra_dzika","dzika_3","Kontrakt 3 - Abomination","Abomination",60,Skills.SkillType.Blocking,240f,"Pokonaj 60 x Abomination."),
        H("mokra_dzika","dzika_4","Kontrakt 4 - Wraith","Wraith",80,Skills.SkillType.Clubs,240f,"Pokonaj 80 x Wraith."),
        H("mokra_dzika","dzika_5","Kontrakt 5 - Blob Elite","BlobElite",100,Skills.SkillType.Axes,320f,"Pokonaj 100 x Blob Elite."),
        H("encek","encek_1","Kontrakt 1 - Wraith","Wraith",40,Skills.SkillType.Polearms,160f,"Pokonaj 40 x Wraith."),
        H("encek","encek_2","Kontrakt 2 - Blob Elite","BlobElite",50,Skills.SkillType.Run,160f,"Pokonaj 50 x Blob Elite."),
        H("encek","encek_3","Kontrakt 3 - Abomination","Abomination",60,Skills.SkillType.Clubs,240f,"Pokonaj 60 x Abomination."),
        H("encek","encek_4","Kontrakt 4 - Wraith","Wraith",80,Skills.SkillType.Blocking,240f,"Pokonaj 80 x Wraith."),
        H("encek","encek_5","Kontrakt 5 - Blob Elite","BlobElite",100,Skills.SkillType.Sneak,320f,"Pokonaj 100 x Blob Elite."),
        H("hrothgar","hrothgar_1","Kontrakt 1 - Fenring","Fenring",40,Skills.SkillType.Bows,220f,"Pokonaj 40 x Fenring."),
        H("hrothgar","hrothgar_2","Kontrakt 2 - Hatchling","Hatchling",50,Skills.SkillType.Blocking,220f,"Pokonaj 50 x Hatchling."),
        H("hrothgar","hrothgar_3","Kontrakt 3 - Stone Golem","StoneGolem",60,Skills.SkillType.Pickaxes,320f,"Pokonaj 60 x Stone Golem."),
        H("hrothgar","hrothgar_4","Kontrakt 4 - Fenring","Fenring",80,Skills.SkillType.Swords,320f,"Pokonaj 80 x Fenring."),
        H("hrothgar","hrothgar_5","Kontrakt 5 - Hatchling","Hatchling",100,Skills.SkillType.Run,420f,"Pokonaj 100 x Hatchling."),
        H("ylva_frost","ylva_1","Kontrakt 1 - Fenring","Fenring",40,Skills.SkillType.Spears,220f,"Pokonaj 40 x Fenring."),
        H("ylva_frost","ylva_2","Kontrakt 2 - Hatchling","Hatchling",50,Skills.SkillType.Knives,220f,"Pokonaj 50 x Hatchling."),
        H("ylva_frost","ylva_3","Kontrakt 3 - Stone Golem","StoneGolem",60,Skills.SkillType.Blocking,320f,"Pokonaj 60 x Stone Golem."),
        H("ylva_frost","ylva_4","Kontrakt 4 - Fenring","Fenring",80,Skills.SkillType.Bows,320f,"Pokonaj 80 x Fenring."),
        H("ylva_frost","ylva_5","Kontrakt 5 - Hatchling","Hatchling",100,Skills.SkillType.Jump,420f,"Pokonaj 100 x Hatchling."),
        H("bjarki_goldtooth","bjarki_1","Kontrakt 1 - Deathsquito","Deathsquito",40,Skills.SkillType.Swords,300f,"Pokonaj 40 x Deathsquito."),
        H("bjarki_goldtooth","bjarki_2","Kontrakt 2 - Goblin Brute","GoblinBrute",50,Skills.SkillType.Spears,300f,"Pokonaj 50 x Goblin Brute."),
        H("bjarki_goldtooth","bjarki_3","Kontrakt 3 - Lox","Lox",60,Skills.SkillType.Blocking,420f,"Pokonaj 60 x Lox."),
        H("bjarki_goldtooth","bjarki_4","Kontrakt 4 - Deathsquito","Deathsquito",80,Skills.SkillType.Bows,420f,"Pokonaj 80 x Deathsquito."),
        H("bjarki_goldtooth","bjarki_5","Kontrakt 5 - Goblin Brute","GoblinBrute",100,Skills.SkillType.Polearms,540f,"Pokonaj 100 x Goblin Brute."),
        H("ragnar_turnipson","ragnar_1","Kontrakt 1 - Deathsquito","Deathsquito",40,Skills.SkillType.Axes,300f,"Pokonaj 40 x Deathsquito."),
        H("ragnar_turnipson","ragnar_2","Kontrakt 2 - Goblin Brute","GoblinBrute",50,Skills.SkillType.Bows,300f,"Pokonaj 50 x Goblin Brute."),
        H("ragnar_turnipson","ragnar_3","Kontrakt 3 - Lox","Lox",60,Skills.SkillType.Blocking,420f,"Pokonaj 60 x Lox."),
        H("ragnar_turnipson","ragnar_4","Kontrakt 4 - Deathsquito","Deathsquito",80,Skills.SkillType.Clubs,420f,"Pokonaj 80 x Deathsquito."),
        H("ragnar_turnipson","ragnar_5","Kontrakt 5 - Goblin Brute","GoblinBrute",100,Skills.SkillType.Run,540f,"Pokonaj 100 x Goblin Brute."),
        H("cmok","cmok_1","Kontrakt 1 - Tick","Tick",40,Skills.SkillType.Polearms,400f,"Pokonaj 40 x Tick."),
        H("cmok","cmok_2","Kontrakt 2 - Seeker Brute","SeekerBrute",50,Skills.SkillType.Blocking,400f,"Pokonaj 50 x Seeker Brute."),
        H("cmok","cmok_3","Kontrakt 3 - Gjall","Gjall",60,Skills.SkillType.Knives,550f,"Pokonaj 60 x Gjall."),
        H("cmok","cmok_4","Kontrakt 4 - Tick","Tick",80,Skills.SkillType.Bows,550f,"Pokonaj 80 x Tick."),
        H("cmok","cmok_5","Kontrakt 5 - Seeker Brute","SeekerBrute",100,Skills.SkillType.Run,700f,"Pokonaj 100 x Seeker Brute."),
        H("grelka","grelka_1","Kontrakt 1 - Tick","Tick",40,Skills.SkillType.Swords,400f,"Pokonaj 40 x Tick."),
        H("grelka","grelka_2","Kontrakt 2 - Seeker Brute","SeekerBrute",50,Skills.SkillType.Clubs,400f,"Pokonaj 50 x Seeker Brute."),
        H("grelka","grelka_3","Kontrakt 3 - Gjall","Gjall",60,Skills.SkillType.Spears,550f,"Pokonaj 60 x Gjall."),
        H("grelka","grelka_4","Kontrakt 4 - Tick","Tick",80,Skills.SkillType.Sneak,550f,"Pokonaj 80 x Tick."),
        H("grelka","grelka_5","Kontrakt 5 - Seeker Brute","SeekerBrute",100,Skills.SkillType.Jump,700f,"Pokonaj 100 x Seeker Brute."),
        H("spalony_zenek","zenek_1","Kontrakt 1 - Charred Mage","Charred_Mage",40,Skills.SkillType.Swords,520f,"Pokonaj 40 x Charred Mage."),
        H("spalony_zenek","zenek_2","Kontrakt 2 - Fallen Valkyrie","FallenValkyrie",50,Skills.SkillType.Blocking,520f,"Pokonaj 50 x Fallen Valkyrie."),
        H("spalony_zenek","zenek_3","Kontrakt 3 - Morgen","Morgen",60,Skills.SkillType.Bows,700f,"Pokonaj 60 x Morgen."),
        H("spalony_zenek","zenek_4","Kontrakt 4 - Charred Mage","Charred_Mage",80,Skills.SkillType.Spears,700f,"Pokonaj 80 x Charred Mage."),
        H("spalony_zenek","zenek_5","Kontrakt 5 - Fallen Valkyrie","FallenValkyrie",100,Skills.SkillType.Run,900f,"Pokonaj 100 x Fallen Valkyrie."),
        H("skjold_cinderborn","skjold_1","Kontrakt 1 - Charred Mage","Charred_Mage",40,Skills.SkillType.Axes,520f,"Pokonaj 40 x Charred Mage."),
        H("skjold_cinderborn","skjold_2","Kontrakt 2 - Fallen Valkyrie","FallenValkyrie",50,Skills.SkillType.Polearms,520f,"Pokonaj 50 x Fallen Valkyrie."),
        H("skjold_cinderborn","skjold_3","Kontrakt 3 - Morgen","Morgen",60,Skills.SkillType.Blocking,700f,"Pokonaj 60 x Morgen."),
        H("skjold_cinderborn","skjold_4","Kontrakt 4 - Charred Mage","Charred_Mage",80,Skills.SkillType.Bows,700f,"Pokonaj 80 x Charred Mage."),
        H("skjold_cinderborn","skjold_5","Kontrakt 5 - Fallen Valkyrie","FallenValkyrie",100,Skills.SkillType.Sneak,900f,"Pokonaj 100 x Fallen Valkyrie."),
    };


    public static void Validate()
    {
        var duplicateIds = Activities.GroupBy(x => x.Id).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
        foreach (string id in duplicateIds)
            Plugin.Log.LogWarning($"Duplicate hunting contract id: {id}");

        foreach (var trader in TraderRegistry.Traders.Where(x => !x.IsLegendary))
        {
            int count = Activities.Count(x => x.TraderId == trader.Id);
            if (count != 5)
                Plugin.Log.LogWarning($"Hunting contracts for {trader.Id}: expected 5, found {count}");
        }

        Plugin.Log.LogInfo($"Hunting contracts ready: {Activities.Count}; unique ids: {Activities.Select(x => x.Id).Distinct().Count()}");
    }

    private static TraderActivityDefinition H(string trader,string id,string title,string target,int count,Skills.SkillType skill,float exp,string text) =>
        new(trader,id,title,TraderActivityType.Hunt,target,count,skill,exp,text);
}
