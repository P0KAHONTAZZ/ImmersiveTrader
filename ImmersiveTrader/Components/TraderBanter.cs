using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>Local trader speech; never changes trader identity or save data.</summary>
public sealed class TraderBanter : MonoBehaviour
{
    public string TraderId = "";
    internal static bool Emitting;
    private float _nextCheck;
    private float _nextIdle;
    private float _lastLine = -100f;
    private bool _greeted;
    private int _previousIdle = -1;

    private sealed record Lines(string[] Greeting, string[] Idle, string[] Sale);
    private static readonly Dictionary<string, Lines> Scripts = new()
    {
        ["midka"] = new(
            new[] { "przed wyruszeniem w droge należy zebrać drużynę", "Stay close. I have bandages for everyone.", "A healer checks on the ones who say they're fine." },
            new[] { "Someone always forgets the healing mead.", "A calm camp can save a life.", "Keep your shield up. I can mend the rest.", "I packed herbs for three days. You packed what?", "przed wyruszeniem w droge należy zebrać drużynę" },
            new[] { "Take care of yourself out there.", "Good. Keep the others alive too.", "Take one for the road. You look like you need it." }),
        ["troldad"] = new(
            new[] { "yyyyy... no.. yyyy... tylko, że ten...", "Oh... hello. Jackie saw you first.", "Back again? I was just... thinking." },
            new[] { "yyyyy... no.. yyyy... tylko, że ten...", "Jackie, where did I put that crate?", "I had a thought. Lost it somewhere in the woods.", "Was I telling you a story? No? Good.", "The forest feels familiar today. That's probably good." },
            new[] { "Good choice. I think.", "Jackie approves. Probably.", "I meant to put that on the shelf." }),
        ["grimvald"] = new(
            new[] { "Watch the roots. The forest claims careless feet.", "You came through the pines in one piece. Impressive." },
            new[] { "Every road through these trees has a price.", "The greydwarfs took my last delivery.", "A quiet forest is never empty.", "Don't trade with the shadows. They never pay." },
            new[] { "Keep it dry on the road.", "May the trees let you pass." }),
        ["rudy_warg"] = new(
            new[] { "Tracks are fresh. Something passed here at dawn.", "Walk lightly. The forest is listening." },
            new[] { "Never turn your back on a troll.", "A good bow is quieter than a good excuse.", "That branch didn't snap itself.", "Even hunters need a warm fire." },
            new[] { "Aim true, traveler.", "Bring it back in one piece." }),
        ["mokra_dzika"] = new(
            new[] { "The swamp gives medicine and poison in equal measure.", "I found the dry ground. Stay close." },
            new[] { "Those roots are useful. Mind the leeches.", "My herbs dislike bright sunlight.", "The marsh has a cure for most things. The rest are dinner.", "I trust my remedies more than a clear sky." },
            new[] { "Keep the lid shut until you leave the marsh.", "I prepared this myself. Use it wisely." }),
        ["encek"] = new(
            new[] { "sprawdzałeś Dreadborne Dungeon? Te dungeony to sanatorium...", "One more run. What could possibly go wrong?", "You here for loot or another wipe?" },
            new[] { "sprawdzałeś Dreadborne Dungeon? Te dungeony to sanatorium...", "One more run. This time, no one dies.", "I know every shortcut. Most of them are traps.", "If it glows, don't touch it until I say so.", "The last dungeon was easy. Ignore the gravestones." },
            new[] { "Good purchase. Your build is getting somewhere.", "Take that. We pull on my mark.", "Now you're almost ready for the next dungeon." }),
        ["hrothgar"] = new(
            new[] { "The mountain remembers every careless step.", "Come in before the snow takes your tracks." },
            new[] { "Storm's coming. I can smell it.", "Wolves make better neighbors than drakes.", "Never trust a clear sky up here.", "Snow hides every mistake until spring." },
            new[] { "Stay warm on the climb.", "Keep your footing out there." }),
        ["ylva_frost"] = new(
            new[] { "Come to the fire before the frost finds you.", "I saved a place by the fire for you." },
            new[] { "Even crystal cracks in this cold.", "The onions survived. So will we.", "I've weathered colder nights than this.", "The mountain can keep its silence. I prefer company." },
            new[] { "Wrap it well. The wind is cruel today.", "Take this and get home before dark." }),
        ["bjarki_goldtooth"] = new(
            new[] { "A full cart is a happy cart.", "Coins or stories? I'll take either." },
            new[] { "Never trust a lox with your lunch.", "The barley grows faster than my debts.", "That cart's worth more than my hut.", "If it shines, someone wants to steal it." },
            new[] { "Gold well spent. Come back with the cart.", "A fair deal is a deal we'll both remember." }),
        ["ragnar_turnipson"] = new(
            new[] { "Mind the lox. It thinks the farm is its own.", "Fresh from the fields. Mostly." },
            new[] { "Barley today, bread tomorrow.", "Nothing grows without a little stubbornness.", "A lox can smell fear and fresh turnips.", "I grew that myself. Don't ask what it ate." },
            new[] { "Fresh stock for a hungry journey.", "Bring me news when you return." }),
        ["cmok"] = new(
            new[] { "No kurwa... znowu wszystkie dungi zajęte", "Dungeon Hunter reporting for the next run.", "You're late. The dungeon isn't going to clear itself." },
            new[] { "Ja pulluje, wy walicie, nie rozumiem o co chodzi?", "No kurwa... znowu wszystkie dungi zajęte", "Who marked that room as cleared?", "If you hear the gjall, that's the start signal.", "I brought a shield. Did you bring a plan?" },
            new[] { "Gear up. There won't be time to shop inside.", "Good. Now let's find a dungeon.", "Keep the shield close. I'm pulling first." }),
        ["grelka"] = new(
            new[] { "If you hear a gjall, do not look up. Run.", "Follow my voice through the mist." },
            new[] { "The mist hides the best mushrooms.", "I can find a road with my eyes closed.", "I marked the path. The mist moved the marks.", "Trust my map. I drew it this morning." },
            new[] { "May the mist let you pass.", "I picked this out for you. Stay safe." }),
        ["spalony_zenek"] = new(
            new[] { "Everything burns here. Some things burn twice.", "Welcome. Keep your boots out of the embers." },
            new[] { "I have survived worse. Probably.", "The ash gets into every lock.", "The ground is warmer than my bed.", "If it starts smoking, leave it outside." },
            new[] { "Do not leave that beside the fire.", "Carry it carefully. Ashlands won't." }),
        ["skjold_cinderborn"] = new(
            new[] { "I forge shelter where the earth is fire.", "Stand near the forge, away from the lava." },
            new[] { "A shield is only as good as the hand behind it.", "The forge does not sleep.", "Steel tells me when it's ready.", "Cinders are part of the trade." },
            new[] { "Put it to work before the ash settles.", "If it breaks, bring me the pieces." })
    };

    private void Awake()
    {
        // Also handle existing persisted NPC instances restored from older prefab data.
        foreach (var speaker in GetComponentsInChildren<MonoBehaviour>(true))
            if (speaker != null && speaker != this &&
                (speaker.GetType().Name == "NpcTalk" || speaker.GetType().Name == "RandomSpeak"))
                UnityEngine.Object.Destroy(speaker);
    }

    private void Update()
    {
        if (Time.time < _nextCheck) return;
        _nextCheck = Time.time + 1f;
        var player = Player.m_localPlayer;
        if (player == null || !Scripts.TryGetValue(TraderId, out var lines)) return;
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > 20f)
        {
            _greeted = false;
            return;
        }
        if (distance < 9f && !_greeted)
        {
            _greeted = true;
            _nextIdle = Time.time + UnityEngine.Random.Range(17f, 28f);
            Speak(lines.Greeting);
        }
        else if (distance < 15f && _greeted && Time.time >= _nextIdle)
        {
            _nextIdle = Time.time + UnityEngine.Random.Range(24f, 38f);
            var next = UnityEngine.Random.Range(0, lines.Idle.Length);
            if (lines.Idle.Length > 1 && next == _previousIdle)
                next = (next + 1) % lines.Idle.Length;
            _previousIdle = next;
            Speak(lines.Idle[next]);
        }
    }

    public void Purchase()
    {
        if (Scripts.TryGetValue(TraderId, out var lines))
            Speak(lines.Sale);
    }

    public static void PurchaseAtShop()
    {
        var npc = NativeTraderWindow.ActiveNpc;
        if (npc != null)
            npc.GetComponent<TraderBanter>()?.Purchase();
    }

    private void Speak(string[] options)
    {
        if (options.Length != 0)
            Speak(options[UnityEngine.Random.Range(0, options.Length)]);
    }

    private void Speak(string line)
    {
        if (Chat.instance == null || Time.time - _lastLine < 3f) return;
        _lastLine = Time.time;
        Emitting = true;
        try
        {
            Chat.instance.SetNpcText(gameObject, new Vector3(0f, 2f, 0f), 22f, 7f, "", line, false);
        }
        finally { Emitting = false; }
    }
}

/// <summary>Reject inherited game dialogue on our trader shells, but preserve every other NPC.</summary>
[HarmonyPatch(typeof(Chat), "SetNpcText", new[] { typeof(GameObject), typeof(Vector3), typeof(float),
    typeof(float), typeof(string), typeof(string), typeof(bool) })]
internal static class TraderVanillaDialoguePatch
{
    private static bool Prefix(GameObject __0)
    {
        if (TraderBanter.Emitting || __0 == null) return true;
        return __0.GetComponent<TraderNpc>() == null &&
            __0.GetComponentInParent<TraderNpc>() == null;
    }
}
