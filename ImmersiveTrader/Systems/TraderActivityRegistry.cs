using System.Collections.Generic;
using ImmersiveTrader.Models;

namespace ImmersiveTrader;

public static class TraderActivityRegistry
{
    public static readonly IReadOnlyList<TraderActivityDefinition> Activities = new List<TraderActivityDefinition>
    {
        // Five contracts per trader. Each trader teaches a different five-skill package.
        H("troldad","troldad_1","Clear the treeline","Greydwarf",8,Skills.SkillType.Run,"Clear Greydwarfs from Troldad's hunting grounds."),
        H("troldad","troldad_2","Big game","Troll",2,Skills.SkillType.Blocking,"Bring down Trolls without becoming their dinner."),
        H("troldad","troldad_3","Forest marksman","Greydwarf_Elite",3,Skills.SkillType.Bows,"Hunt Greydwarf Brutes."),
        H("troldad","troldad_4","Keep moving","Greydwarf_Shaman",4,Skills.SkillType.Jump,"Clear the shamans from the route."),
        H("troldad","troldad_5","Jackie's trail","Skeleton",8,Skills.SkillType.Sneak,"Clean out skeletons near the old trails."),

        H("midka","midka_1","Field clearance","Greydwarf",8,Skills.SkillType.Knives,"Clear the road for Midka's patients."),
        H("midka","midka_2","Emergency response","Skeleton",8,Skills.SkillType.Run,"Move fast and clear skeleton patrols."),
        H("midka","midka_3","Keep your guard","Greydwarf_Elite",3,Skills.SkillType.Blocking,"Practice surviving heavier attackers."),
        H("midka","midka_4","Clean shot","Greydwarf_Shaman",4,Skills.SkillType.Bows,"Remove shamans before they close in."),
        H("midka","midka_5","Footwork","Troll",2,Skills.SkillType.Jump,"Survive a hunt against Trolls."),

        H("grimvald","grimvald_1","Forest patrol","Greydwarf",10,Skills.SkillType.Axes,"Patrol the Black Forest."),
        H("grimvald","grimvald_2","Bone road","Skeleton",10,Skills.SkillType.Clubs,"Clear the burial roads."),
        H("grimvald","grimvald_3","Brute force","Greydwarf_Elite",5,Skills.SkillType.Swords,"Kill Greydwarf Brutes."),
        H("grimvald","grimvald_4","Shaman hunt","Greydwarf_Shaman",5,Skills.SkillType.Sneak,"Track forest shamans."),
        H("grimvald","grimvald_5","Troll contract","Troll",3,Skills.SkillType.Blocking,"Take down Trolls."),

        H("rudy_warg","rudy_1","Warg's mark","Greydwarf",10,Skills.SkillType.Bows,"Hunt the forest packs."),
        H("rudy_warg","rudy_2","Heavy prey","Troll",3,Skills.SkillType.Spears,"Hunt Trolls."),
        H("rudy_warg","rudy_3","Close hunt","Greydwarf_Elite",5,Skills.SkillType.Knives,"Kill Brutes at close range."),
        H("rudy_warg","rudy_4","Long trail","Skeleton",10,Skills.SkillType.Run,"Clear the long hunting trail."),
        H("rudy_warg","rudy_5","Silent trail","Greydwarf_Shaman",5,Skills.SkillType.Sneak,"Track down shamans."),

        H("mokra_dzika","dzika_1","Thin the leeches","Leech",8,Skills.SkillType.Spears,"Reduce the leech population."),
        H("mokra_dzika","dzika_2","Rot walkers","Draugr",8,Skills.SkillType.Swords,"Clear Draugr."),
        H("mokra_dzika","dzika_3","Bog archers","Draugr_Ranged",6,Skills.SkillType.Blocking,"Break the ranged patrols."),
        H("mokra_dzika","dzika_4","Slime work","Blob",8,Skills.SkillType.Clubs,"Clear Blobs."),
        H("mokra_dzika","dzika_5","Swamp giant","Abomination",2,Skills.SkillType.Axes,"Bring down Abominations."),

        H("encek","encek_1","Bog patrol","Draugr",8,Skills.SkillType.Polearms,"Clear Draugr patrols."),
        H("encek","encek_2","Leech run","Leech",8,Skills.SkillType.Run,"Clear the waterways."),
        H("encek","encek_3","Ooze line","BlobElite",4,Skills.SkillType.Clubs,"Destroy Oozers."),
        H("encek","encek_4","Old roots","Abomination",2,Skills.SkillType.Blocking,"Face Abominations."),
        H("encek","encek_5","Silent bog","Wraith",3,Skills.SkillType.Sneak,"Hunt Wraiths."),

        H("hrothgar","hrothgar_1","Clear the peaks","Hatchling",6,Skills.SkillType.Bows,"Drive drakes away."),
        H("hrothgar","hrothgar_2","Wolf line","Wolf",8,Skills.SkillType.Blocking,"Cull mountain wolves."),
        H("hrothgar","hrothgar_3","Stone giant","StoneGolem",2,Skills.SkillType.Pickaxes,"Break Stone Golems."),
        H("hrothgar","hrothgar_4","Night beast","Fenring",3,Skills.SkillType.Swords,"Hunt Fenrings."),
        H("hrothgar","hrothgar_5","Peak runner","Wolf",12,Skills.SkillType.Run,"Complete the long wolf hunt."),

        H("ylva_frost","ylva_1","Drake feathers","Hatchling",6,Skills.SkillType.Spears,"Hunt drakes."),
        H("ylva_frost","ylva_2","White hunt","Wolf",8,Skills.SkillType.Knives,"Hunt wolves."),
        H("ylva_frost","ylva_3","Frozen guard","StoneGolem",2,Skills.SkillType.Blocking,"Face Stone Golems."),
        H("ylva_frost","ylva_4","Moon hunt","Fenring",3,Skills.SkillType.Bows,"Hunt Fenrings."),
        H("ylva_frost","ylva_5","High trail","Hatchling",10,Skills.SkillType.Jump,"Clear the highest route."),

        H("bjarki_goldtooth","bjarki_1","Plains patrol","Goblin",10,Skills.SkillType.Swords,"Cut down Fulings."),
        H("bjarki_goldtooth","bjarki_2","Spear line","Goblin",12,Skills.SkillType.Spears,"Break Fuling patrols."),
        H("bjarki_goldtooth","bjarki_3","Berserker","GoblinBrute",4,Skills.SkillType.Blocking,"Face Berserkers."),
        H("bjarki_goldtooth","bjarki_4","Deathsquito season","Deathsquito",8,Skills.SkillType.Bows,"Shoot down Deathsquitos."),
        H("bjarki_goldtooth","bjarki_5","Lox hunt","Lox",3,Skills.SkillType.Polearms,"Hunt Lox."),

        H("ragnar_turnipson","ragnar_1","Farm defense","Goblin",10,Skills.SkillType.Axes,"Defend the fields."),
        H("ragnar_turnipson","ragnar_2","Needle harvest","Deathsquito",8,Skills.SkillType.Bows,"Hunt Deathsquitos."),
        H("ragnar_turnipson","ragnar_3","Heavy harvest","Lox",3,Skills.SkillType.Blocking,"Survive a Lox hunt."),
        H("ragnar_turnipson","ragnar_4","Berserker tax","GoblinBrute",4,Skills.SkillType.Clubs,"Collect from Berserkers."),
        H("ragnar_turnipson","ragnar_5","Fast delivery","Goblin",15,Skills.SkillType.Run,"Clear a long trade route."),

        H("cmok","cmok_1","Quiet the mist","Seeker",8,Skills.SkillType.Polearms,"Cull Seekers."),
        H("cmok","cmok_2","Soldier contract","SeekerBrute",3,Skills.SkillType.Blocking,"Kill Seeker Soldiers."),
        H("cmok","cmok_3","Brood control","Tick",10,Skills.SkillType.Knives,"Clear Ticks."),
        H("cmok","cmok_4","Hare chase","Hare",8,Skills.SkillType.Bows,"Hunt hares."),
        H("cmok","cmok_5","Mist runner","Seeker",12,Skills.SkillType.Run,"Clear a long route through the mist."),

        H("grelka","grelka_1","Carapace work","Seeker",8,Skills.SkillType.Swords,"Cull Seekers."),
        H("grelka","grelka_2","Heavy shell","SeekerBrute",3,Skills.SkillType.Clubs,"Kill Seeker Soldiers."),
        H("grelka","grelka_3","Tick line","Tick",10,Skills.SkillType.Spears,"Clear Ticks."),
        H("grelka","grelka_4","Mist stalker","Hare",8,Skills.SkillType.Sneak,"Track hares."),
        H("grelka","grelka_5","High ruins","Seeker",12,Skills.SkillType.Jump,"Clear the ruined approaches."),

        H("spalony_zenek","zenek_1","Ash patrol","Charred_Twitcher",10,Skills.SkillType.Swords,"Clear Charred Twitchers."),
        H("spalony_zenek","zenek_2","Charred warriors","Charred_Melee",8,Skills.SkillType.Blocking,"Fight Charred warriors."),
        H("spalony_zenek","zenek_3","Ash archers","Charred_Archer",8,Skills.SkillType.Bows,"Remove Charred archers."),
        H("spalony_zenek","zenek_4","Volture hunt","Volture",6,Skills.SkillType.Spears,"Hunt Voltures."),
        H("spalony_zenek","zenek_5","Burning road","Charred_Twitcher",15,Skills.SkillType.Run,"Clear the burning road."),

        H("skjold_cinderborn","skjold_1","Cinder line","Charred_Twitcher",10,Skills.SkillType.Axes,"Clear Twitchers."),
        H("skjold_cinderborn","skjold_2","Warrior line","Charred_Melee",8,Skills.SkillType.Polearms,"Fight Charred warriors."),
        H("skjold_cinderborn","skjold_3","Bow line","Charred_Archer",8,Skills.SkillType.Blocking,"Break the archer line."),
        H("skjold_cinderborn","skjold_4","Sky hunt","Volture",6,Skills.SkillType.Bows,"Hunt Voltures."),
        H("skjold_cinderborn","skjold_5","Ash stalker","Charred_Twitcher",15,Skills.SkillType.Sneak,"Hunt the outer patrols.")
    };

    private static TraderActivityDefinition H(string trader,string id,string title,string target,int count,Skills.SkillType skill,string text) =>
        new(trader,id,title,TraderActivityType.Hunt,target,count,skill,2f,text);
}
