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
        H("midka","midka_1","Szept szamana w lesie","Greydwarf_Shaman",40,Skills.SkillType.Knives,50f,"Pokonaj 40 x Greydwarf Shaman."),
        H("midka","midka_2","Zasadzka brutalów na trakcie","Greydwarf_Elite",50,Skills.SkillType.Run,50f,"Pokonaj 50 x Greydwarf Brute."),
        H("midka","midka_3","Troll pod mostem Midki","Troll",60,Skills.SkillType.Blocking,100f,"Pokonaj 60 x Troll."),
        H("midka","midka_4","Wielkie łowy na trolla","Troll",80,Skills.SkillType.Bows,100f,"Pokonaj 80 x Troll."),
        H("midka","midka_5","Ostatni brutal przy lecznicy","Greydwarf_Elite",100,Skills.SkillType.Jump,150f,"Pokonaj 100 x Greydwarf Brute."),
        H("troldad","troldad_1","Szaman w drwalskim borze","Greydwarf_Shaman",40,Skills.SkillType.Run,50f,"Pokonaj 40 x Greydwarf Shaman."),
        H("troldad","troldad_2","Brutal niszczący wyrąb","Greydwarf_Elite",50,Skills.SkillType.Blocking,50f,"Pokonaj 50 x Greydwarf Brute."),
        H("troldad","troldad_3","Troll między pniami","Troll",60,Skills.SkillType.Bows,100f,"Pokonaj 60 x Troll."),
        H("troldad","troldad_4","Polowanie na leśnego olbrzyma","Troll",80,Skills.SkillType.Jump,100f,"Pokonaj 80 x Troll."),
        H("troldad","troldad_5","Zemsta drwali na brutalu","Greydwarf_Elite",100,Skills.SkillType.Sneak,150f,"Pokonaj 100 x Greydwarf Brute."),
        H("grimvald","grimvald_1","Szaman przy miedzianej żyle","Greydwarf_Shaman",40,Skills.SkillType.Axes,100f,"Pokonaj 40 x Greydwarf Shaman."),
        H("grimvald","grimvald_2","Brutal w kopalni Grimvalda","Greydwarf_Elite",50,Skills.SkillType.Clubs,100f,"Pokonaj 50 x Greydwarf Brute."),
        H("grimvald","grimvald_3","Troll spod rudnej skały","Troll",60,Skills.SkillType.Swords,160f,"Pokonaj 60 x Troll."),
        H("grimvald","grimvald_4","Cień trolla nad kuźnią","Troll",80,Skills.SkillType.Sneak,160f,"Pokonaj 80 x Troll."),
        H("grimvald","grimvald_5","Strażnik miedzi i cyny","Greydwarf_Elite",100,Skills.SkillType.Blocking,220f,"Pokonaj 100 x Greydwarf Brute."),
        H("rudy_warg","rudy_1","Szaman na tropie Warga","Greydwarf_Shaman",40,Skills.SkillType.Bows,100f,"Pokonaj 40 x Greydwarf Shaman."),
        H("rudy_warg","rudy_2","Brutal w jelenim rewirze","Greydwarf_Elite",50,Skills.SkillType.Spears,100f,"Pokonaj 50 x Greydwarf Brute."),
        H("rudy_warg","rudy_3","Troll na szlaku łowców","Troll",60,Skills.SkillType.Knives,160f,"Pokonaj 60 x Troll."),
        H("rudy_warg","rudy_4","Warg kontra górski troll","Troll",80,Skills.SkillType.Run,160f,"Pokonaj 80 x Troll."),
        H("rudy_warg","rudy_5","Ostatni łup brutalów","Greydwarf_Elite",100,Skills.SkillType.Sneak,220f,"Pokonaj 100 x Greydwarf Brute."),
        H("mokra_dzika","dzika_1","Upiór nad czarną wodą","Wraith",40,Skills.SkillType.Spears,160f,"Pokonaj 40 x Wraith."),
        H("mokra_dzika","dzika_2","Elitarny blob w bagnie","BlobElite",50,Skills.SkillType.Swords,160f,"Pokonaj 50 x Blob Elite."),
        H("mokra_dzika","dzika_3","Abominacja z zatopionego lasu","Abomination",60,Skills.SkillType.Blocking,240f,"Pokonaj 60 x Abomination."),
        H("mokra_dzika","dzika_4","Nocne łowy na upiora","Wraith",80,Skills.SkillType.Clubs,240f,"Pokonaj 80 x Wraith."),
        H("mokra_dzika","dzika_5","Błotne gniazdo blobów","BlobElite",100,Skills.SkillType.Axes,320f,"Pokonaj 100 x Blob Elite."),
        H("encek","encek_1","Widmo przy żelaznym szlaku","Wraith",40,Skills.SkillType.Polearms,160f,"Pokonaj 40 x Wraith."),
        H("encek","encek_2","Blob w zardzewiałej krypcie","BlobElite",50,Skills.SkillType.Run,160f,"Pokonaj 50 x Blob Elite."),
        H("encek","encek_3","Korzenie abominacji","Abomination",60,Skills.SkillType.Clubs,240f,"Pokonaj 60 x Abomination."),
        H("encek","encek_4","Upiór z opuszczonej kuźni","Wraith",80,Skills.SkillType.Blocking,240f,"Pokonaj 80 x Wraith."),
        H("encek","encek_5","Bagienna zaraza Encka","BlobElite",100,Skills.SkillType.Sneak,320f,"Pokonaj 100 x Blob Elite."),
        H("hrothgar","hrothgar_1","Fenring na lodowej grani","Fenring",40,Skills.SkillType.Bows,220f,"Pokonaj 40 x Fenring."),
        H("hrothgar","hrothgar_2","Smocze pisklę nad przełęczą","Hatchling",50,Skills.SkillType.Blocking,220f,"Pokonaj 50 x Hatchling."),
        H("hrothgar","hrothgar_3","Golem strzegący srebra","StoneGolem",60,Skills.SkillType.Pickaxes,320f,"Pokonaj 60 x Stone Golem."),
        H("hrothgar","hrothgar_4","Noc fenringów Hrothgara","Fenring",80,Skills.SkillType.Swords,320f,"Pokonaj 80 x Fenring."),
        H("hrothgar","hrothgar_5","Niebo pełne smoczych piskląt","Hatchling",100,Skills.SkillType.Run,420f,"Pokonaj 100 x Hatchling."),
        H("ylva_frost","ylva_1","Fenring u wrót Ylvy","Fenring",40,Skills.SkillType.Spears,220f,"Pokonaj 40 x Fenring."),
        H("ylva_frost","ylva_2","Pisklę na śnieżnym szczycie","Hatchling",50,Skills.SkillType.Knives,220f,"Pokonaj 50 x Hatchling."),
        H("ylva_frost","ylva_3","Kamienny golem Ylvy","StoneGolem",60,Skills.SkillType.Blocking,320f,"Pokonaj 60 x Stone Golem."),
        H("ylva_frost","ylva_4","Tropem zimowego fenringa","Fenring",80,Skills.SkillType.Bows,320f,"Pokonaj 80 x Fenring."),
        H("ylva_frost","ylva_5","Smocze skrzydła nad mrozem","Hatchling",100,Skills.SkillType.Jump,420f,"Pokonaj 100 x Hatchling."),
        H("bjarki_goldtooth","bjarki_1","Komary śmierci nad złotem","Deathsquito",40,Skills.SkillType.Swords,300f,"Pokonaj 40 x Deathsquito."),
        H("bjarki_goldtooth","bjarki_2","Brutal Fulingów przy targu","GoblinBrute",50,Skills.SkillType.Spears,300f,"Pokonaj 50 x Goblin Brute."),
        H("bjarki_goldtooth","bjarki_3","Lox z równin Bjarkiego","Lox",60,Skills.SkillType.Blocking,420f,"Pokonaj 60 x Lox."),
        H("bjarki_goldtooth","bjarki_4","Rój komarów nad jęczmieniem","Deathsquito",80,Skills.SkillType.Bows,420f,"Pokonaj 80 x Deathsquito."),
        H("bjarki_goldtooth","bjarki_5","Złoty szlak i Fulingowie","GoblinBrute",100,Skills.SkillType.Polearms,540f,"Pokonaj 100 x Goblin Brute."),
        H("ragnar_turnipson","ragnar_1","Komar śmierci w rzepie","Deathsquito",40,Skills.SkillType.Axes,300f,"Pokonaj 40 x Deathsquito."),
        H("ragnar_turnipson","ragnar_2","Fuling Brute przy młynie","GoblinBrute",50,Skills.SkillType.Bows,300f,"Pokonaj 50 x Goblin Brute."),
        H("ragnar_turnipson","ragnar_3","Lox depczący pola Ragnara","Lox",60,Skills.SkillType.Blocking,420f,"Pokonaj 60 x Lox."),
        H("ragnar_turnipson","ragnar_4","Ostatni rój na równinach","Deathsquito",80,Skills.SkillType.Clubs,420f,"Pokonaj 80 x Deathsquito."),
        H("ragnar_turnipson","ragnar_5","Ragnar kontra brutal Fulingów","GoblinBrute",100,Skills.SkillType.Run,540f,"Pokonaj 100 x Goblin Brute."),
        H("cmok","cmok_1","Kleszcze w gęstej mgle","Tick",40,Skills.SkillType.Polearms,400f,"Pokonaj 40 x Tick."),
        H("cmok","cmok_2","Seeker Brute spod korzeni","SeekerBrute",50,Skills.SkillType.Blocking,400f,"Pokonaj 50 x Seeker Brute."),
        H("cmok","cmok_3","Gjall nad warsztatem Cmoka","Gjall",60,Skills.SkillType.Knives,550f,"Pokonaj 60 x Gjall."),
        H("cmok","cmok_4","Plaga kleszczy w ruinach","Tick",80,Skills.SkillType.Bows,550f,"Pokonaj 80 x Tick."),
        H("cmok","cmok_5","Wielki poszukiwacz we mgle","SeekerBrute",100,Skills.SkillType.Run,700f,"Pokonaj 100 x Seeker Brute."),
        H("grelka","grelka_1","Kleszcz przy grzybni Grelki","Tick",40,Skills.SkillType.Swords,400f,"Pokonaj 40 x Tick."),
        H("grelka","grelka_2","Seeker Brute na szlaku magów","SeekerBrute",50,Skills.SkillType.Clubs,400f,"Pokonaj 50 x Seeker Brute."),
        H("grelka","grelka_3","Gjall nad czarnym marmurem","Gjall",60,Skills.SkillType.Spears,550f,"Pokonaj 60 x Gjall."),
        H("grelka","grelka_4","Noc kleszczy w Mglistych Ziemiach","Tick",80,Skills.SkillType.Sneak,550f,"Pokonaj 80 x Tick."),
        H("grelka","grelka_5","Grelka i łowca poszukiwaczy","SeekerBrute",100,Skills.SkillType.Jump,700f,"Pokonaj 100 x Seeker Brute."),
        H("spalony_zenek","zenek_1","Mag spopielonych pod fortem","Charred_Mage",40,Skills.SkillType.Swords,520f,"Pokonaj 40 x Charred Mage."),
        H("spalony_zenek","zenek_2","Walkiria nad obozem Zenka","FallenValkyrie",50,Skills.SkillType.Blocking,520f,"Pokonaj 50 x Fallen Valkyrie."),
        H("spalony_zenek","zenek_3","Morgen w popielnej dolinie","Morgen",60,Skills.SkillType.Bows,700f,"Pokonaj 60 x Morgen."),
        H("spalony_zenek","zenek_4","Ognisty mag przy drodze","Charred_Mage",80,Skills.SkillType.Spears,700f,"Pokonaj 80 x Charred Mage."),
        H("spalony_zenek","zenek_5","Ostatni lot upadłej walkirii","FallenValkyrie",100,Skills.SkillType.Run,900f,"Pokonaj 100 x Fallen Valkyrie."),
        H("skjold_cinderborn","skjold_1","Mag przy murach Skjolda","Charred_Mage",40,Skills.SkillType.Axes,520f,"Pokonaj 40 x Charred Mage."),
        H("skjold_cinderborn","skjold_2","Walkiria nad fortyfikacją","FallenValkyrie",50,Skills.SkillType.Polearms,520f,"Pokonaj 50 x Fallen Valkyrie."),
        H("skjold_cinderborn","skjold_3","Morgen kruszący bastion","Morgen",60,Skills.SkillType.Blocking,700f,"Pokonaj 60 x Morgen."),
        H("skjold_cinderborn","skjold_4","Spopieleni magowie na warcie","Charred_Mage",80,Skills.SkillType.Bows,700f,"Pokonaj 80 x Charred Mage."),
        H("skjold_cinderborn","skjold_5","Obrona przed upadłą walkirią","FallenValkyrie",100,Skills.SkillType.Sneak,900f,"Pokonaj 100 x Fallen Valkyrie."),
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
        }

        Plugin.Log.LogInfo($"Hunting contracts ready: {Activities.Count}; unique ids: {Activities.Select(x => x.Id).Distinct().Count()}");
    }

    private static TraderActivityDefinition H(string trader,string id,string title,string target,int count,Skills.SkillType skill,float exp,string text) =>
        new(trader,id,title,TraderActivityType.Hunt,target,count,skill,exp,text);
}
