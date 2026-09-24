using System.Collections.Generic;
using System.Linq;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class TraderActivityRegistry
{
    public static readonly IReadOnlyList<TraderActivityDefinition> Activities = new List<TraderActivityDefinition>
    {
        // One fixed difficulty per contract. Existing issued scrolls retain their stored targets.
        H("midka","midka_1","Szept szamana w lesie","Greydwarf_Shaman",30,Skills.SkillType.Knives,15f,"Pokonaj 30 x Greydwarf Shaman."),
        H("midka","midka_2","Zasadzka brutalów na trakcie","Greydwarf_Elite",30,Skills.SkillType.Cooking,15f,"Pokonaj 30 x Greydwarf Brute."),
        H("midka","midka_3","Troll pod mostem Midki","Troll",12,Skills.SkillType.Crafting,18f,"Pokonaj 12 x Troll."),
        H("midka","midka_4","Wielkie łowy na trolla","Troll",15,Skills.SkillType.Run,18f,"Pokonaj 15 x Troll."),
        H("midka","midka_5","Ostatni brutal przy lecznicy","Greydwarf_Elite",30,Skills.SkillType.Blocking,20f,"Pokonaj 30 x Greydwarf Brute."),
        H("troldad","troldad_1","Szaman w drwalskim borze","Greydwarf_Shaman",30,Skills.SkillType.WoodCutting,15f,"Pokonaj 30 x Greydwarf Shaman."),
        H("troldad","troldad_2","Brutal niszczący wyrąb","Greydwarf_Elite",30,Skills.SkillType.Axes,15f,"Pokonaj 30 x Greydwarf Brute."),
        H("troldad","troldad_3","Troll między pniami","Troll",12,Skills.SkillType.Bows,18f,"Pokonaj 12 x Troll."),
        H("troldad","troldad_4","Polowanie na leśnego olbrzyma","Troll",15,Skills.SkillType.Clubs,18f,"Pokonaj 15 x Troll."),
        H("troldad","troldad_5","Zemsta drwali na brutalu","Greydwarf_Elite",30,Skills.SkillType.Sneak,20f,"Pokonaj 30 x Greydwarf Brute."),
        H("grimvald","grimvald_1","Szaman przy miedzianej żyle","Greydwarf_Shaman",30,Skills.SkillType.Pickaxes,50f,"Pokonaj 30 x Greydwarf Shaman."),
        H("grimvald","grimvald_2","Brutal w kopalni Grimvalda","Greydwarf_Elite",30,Skills.SkillType.WoodCutting,50f,"Pokonaj 30 x Greydwarf Brute."),
        H("grimvald","grimvald_3","Troll spod rudnej skały","Troll",12,Skills.SkillType.Crafting,70f,"Pokonaj 12 x Troll."),
        H("grimvald","grimvald_4","Cień trolla nad kuźnią","Troll",15,Skills.SkillType.Clubs,70f,"Pokonaj 15 x Troll."),
        H("grimvald","grimvald_5","Strażnik miedzi i cyny","Greydwarf_Elite",30,Skills.SkillType.Blocking,90f,"Pokonaj 30 x Greydwarf Brute."),
        H("rudy_warg","rudy_1","Szaman na tropie Warga","Greydwarf_Shaman",30,Skills.SkillType.Bows,50f,"Pokonaj 30 x Greydwarf Shaman."),
        H("rudy_warg","rudy_2","Brutal w jelenim rewirze","Greydwarf_Elite",30,Skills.SkillType.Spears,50f,"Pokonaj 30 x Greydwarf Brute."),
        H("rudy_warg","rudy_3","Troll na szlaku łowców","Troll",12,Skills.SkillType.Sneak,70f,"Pokonaj 12 x Troll."),
        H("rudy_warg","rudy_4","Warg kontra górski troll","Troll",15,Skills.SkillType.Crossbows,70f,"Pokonaj 15 x Troll."),
        H("rudy_warg","rudy_5","Ostatni łup brutalów","Greydwarf_Elite",30,Skills.SkillType.Run,90f,"Pokonaj 30 x Greydwarf Brute."),
        H("mokra_dzika","dzika_1","Upiór nad czarną wodą","Wraith",10,Skills.SkillType.Farming,140f,"Pokonaj 10 x Wraith."),
        H("mokra_dzika","dzika_2","Elitarny blob w bagnie","BlobElite",25,Skills.SkillType.Cooking,140f,"Pokonaj 25 x Blob Elite."),
        H("mokra_dzika","dzika_3","Abominacja z zatopionego lasu","Abomination",10,Skills.SkillType.Swim,180f,"Pokonaj 10 x Abomination."),
        H("mokra_dzika","dzika_4","Nocne łowy na upiora","Wraith",15,Skills.SkillType.Spears,180f,"Pokonaj 15 x Wraith."),
        H("mokra_dzika","dzika_5","Błotne gniazdo blobów","BlobElite",30,Skills.SkillType.Clubs,220f,"Pokonaj 30 x Blob Elite."),
        H("encek","encek_1","Widmo przy żelaznym szlaku","Wraith",10,Skills.SkillType.Fishing,140f,"Pokonaj 10 x Wraith."),
        H("encek","encek_2","Blob w zardzewiałej krypcie","BlobElite",25,Skills.SkillType.Crafting,140f,"Pokonaj 25 x Blob Elite."),
        H("encek","encek_3","Korzenie abominacji","Abomination",10,Skills.SkillType.Polearms,180f,"Pokonaj 10 x Abomination."),
        H("encek","encek_4","Upiór z opuszczonej kuźni","Wraith",15,Skills.SkillType.Blocking,180f,"Pokonaj 15 x Wraith."),
        H("encek","encek_5","Bagienna zaraza Encka","BlobElite",30,Skills.SkillType.Axes,220f,"Pokonaj 30 x Blob Elite."),
        H("hrothgar","hrothgar_1","Fenring na lodowej grani","Fenring",10,Skills.SkillType.Pickaxes,230f,"Pokonaj 10 x Fenring."),
        H("hrothgar","hrothgar_2","Smocze pisklę nad przełęczą","Hatchling",25,Skills.SkillType.Bows,230f,"Pokonaj 25 x Hatchling."),
        H("hrothgar","hrothgar_3","Golem strzegący srebra","StoneGolem",10,Skills.SkillType.Swords,300f,"Pokonaj 10 x Stone Golem."),
        H("hrothgar","hrothgar_4","Noc fenringów Hrothgara","Fenring",18,Skills.SkillType.Jump,300f,"Pokonaj 18 x Fenring."),
        H("hrothgar","hrothgar_5","Niebo pełne smoczych piskląt","Hatchling",30,Skills.SkillType.Ride,380f,"Pokonaj 30 x Hatchling."),
        H("ylva_frost","ylva_1","Fenring u wrót Ylvy","Fenring",10,Skills.SkillType.Spears,230f,"Pokonaj 10 x Fenring."),
        H("ylva_frost","ylva_2","Pisklę na śnieżnym szczycie","Hatchling",25,Skills.SkillType.Knives,230f,"Pokonaj 25 x Hatchling."),
        H("ylva_frost","ylva_3","Kamienny golem Ylvy","StoneGolem",10,Skills.SkillType.Blocking,300f,"Pokonaj 10 x Stone Golem."),
        H("ylva_frost","ylva_4","Tropem zimowego fenringa","Fenring",18,Skills.SkillType.Dodge,300f,"Pokonaj 18 x Fenring."),
        H("ylva_frost","ylva_5","Smocze skrzydła nad mrozem","Hatchling",30,Skills.SkillType.Cooking,380f,"Pokonaj 30 x Hatchling."),
        H("bjarki_goldtooth","bjarki_1","Komary śmierci nad złotem","Deathsquito",20,Skills.SkillType.Swords,350f,"Pokonaj 20 x Deathsquito."),
        H("bjarki_goldtooth","bjarki_2","Brutal Fulingów przy targu","GoblinBrute",15,Skills.SkillType.Spears,350f,"Pokonaj 15 x Goblin Brute."),
        H("bjarki_goldtooth","bjarki_3","Lox z równin Bjarkiego","Lox",12,Skills.SkillType.Blocking,450f,"Pokonaj 12 x Lox."),
        H("bjarki_goldtooth","bjarki_4","Rój komarów nad jęczmieniem","Deathsquito",30,Skills.SkillType.Polearms,450f,"Pokonaj 30 x Deathsquito."),
        H("bjarki_goldtooth","bjarki_5","Złoty szlak i Fulingowie","GoblinBrute",20,Skills.SkillType.Ride,550f,"Pokonaj 20 x Goblin Brute."),
        H("ragnar_turnipson","ragnar_1","Komar śmierci w rzepie","Deathsquito",20,Skills.SkillType.Farming,350f,"Pokonaj 20 x Deathsquito."),
        H("ragnar_turnipson","ragnar_2","Fuling Brute przy młynie","GoblinBrute",15,Skills.SkillType.Cooking,350f,"Pokonaj 15 x Goblin Brute."),
        H("ragnar_turnipson","ragnar_3","Lox depczący pola Ragnara","Lox",12,Skills.SkillType.Axes,450f,"Pokonaj 12 x Lox."),
        H("ragnar_turnipson","ragnar_4","Ostatni rój na równinach","Deathsquito",30,Skills.SkillType.Clubs,450f,"Pokonaj 30 x Deathsquito."),
        H("ragnar_turnipson","ragnar_5","Ragnar kontra brutal Fulingów","GoblinBrute",20,Skills.SkillType.Run,550f,"Pokonaj 20 x Goblin Brute."),
        H("cmok","cmok_1","Kleszcze w gęstej mgle","Tick",25,Skills.SkillType.BloodMagic,500f,"Pokonaj 25 x Tick."),
        H("cmok","cmok_2","Seeker Brute spod korzeni","SeekerBrute",10,Skills.SkillType.Crossbows,500f,"Pokonaj 10 x Seeker Brute."),
        H("cmok","cmok_3","Gjall nad warsztatem Cmoka","Gjall",8,Skills.SkillType.Polearms,650f,"Pokonaj 8 x Gjall."),
        H("cmok","cmok_4","Plaga kleszczy w ruinach","Tick",30,Skills.SkillType.Swim,650f,"Pokonaj 30 x Tick."),
        H("cmok","cmok_5","Wielki poszukiwacz we mgle","SeekerBrute",15,Skills.SkillType.Jump,800f,"Pokonaj 15 x Seeker Brute."),
        H("grelka","grelka_1","Kleszcz przy grzybni Grelki","Tick",25,Skills.SkillType.Swords,500f,"Pokonaj 25 x Tick."),
        H("grelka","grelka_2","Seeker Brute na szlaku magów","SeekerBrute",10,Skills.SkillType.ElementalMagic,500f,"Pokonaj 10 x Seeker Brute."),
        H("grelka","grelka_3","Gjall nad czarnym marmurem","Gjall",8,Skills.SkillType.Crafting,650f,"Pokonaj 8 x Gjall."),
        H("grelka","grelka_4","Noc kleszczy w Mglistych Ziemiach","Tick",30,Skills.SkillType.Sneak,650f,"Pokonaj 30 x Tick."),
        H("grelka","grelka_5","Grelka i łowca poszukiwaczy","SeekerBrute",15,Skills.SkillType.Unarmed,800f,"Pokonaj 15 x Seeker Brute."),
        H("spalony_zenek","zenek_1","Mag spopielonych pod fortem","Charred_Mage",20,Skills.SkillType.Swords,750f,"Pokonaj 20 x Charred Mage."),
        H("spalony_zenek","zenek_2","Walkiria nad obozem Zenka","FallenValkyrie",6,Skills.SkillType.ElementalMagic,750f,"Pokonaj 6 x Fallen Valkyrie."),
        H("spalony_zenek","zenek_3","Morgen w popielnej dolinie","Morgen",10,Skills.SkillType.Crossbows,850f,"Pokonaj 10 x Morgen."),
        H("spalony_zenek","zenek_4","Ognisty mag przy drodze","Charred_Mage",30,Skills.SkillType.Spears,850f,"Pokonaj 30 x Charred Mage."),
        H("spalony_zenek","zenek_5","Ostatni lot upadłej walkirii","FallenValkyrie",8,Skills.SkillType.Run,950f,"Pokonaj 8 x Fallen Valkyrie."),
        H("skjold_cinderborn","skjold_1","Mag przy murach Skjolda","Charred_Mage",20,Skills.SkillType.WoodCutting,750f,"Pokonaj 20 x Charred Mage."),
        H("skjold_cinderborn","skjold_2","Walkiria nad fortyfikacją","FallenValkyrie",6,Skills.SkillType.Polearms,750f,"Pokonaj 6 x Fallen Valkyrie."),
        H("skjold_cinderborn","skjold_3","Morgen kruszący bastion","Morgen",10,Skills.SkillType.Blocking,850f,"Pokonaj 10 x Morgen."),
        H("skjold_cinderborn","skjold_4","Spopieleni magowie na warcie","Charred_Mage",30,Skills.SkillType.Crafting,850f,"Pokonaj 30 x Charred Mage."),
        H("skjold_cinderborn","skjold_5","Obrona przed upadłą walkirią","FallenValkyrie",8,Skills.SkillType.Dodge,950f,"Pokonaj 8 x Fallen Valkyrie."),
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
