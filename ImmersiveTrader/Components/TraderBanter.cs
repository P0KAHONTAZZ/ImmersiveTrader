using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>Client-side, local NPC dialogue. The trader shell owns interaction and networking.</summary>
public sealed class TraderBanter : MonoBehaviour
{
    public string TraderId = "";
    private float _nextCheck;
    private float _nextIdle;
    private float _lastLine;
    private bool _greeted;

    private sealed record Lines(string[] Greeting, string[] Idle, string[] Sale);
    private static readonly Dictionary<string, Lines> Scripts = new()
    {
        ["midka"] = new(
            new[] { "przed wyruszeniem w droge należy zebrać drużynę", "Stay close. I have bandages for everyone." },
            new[] { "Someone always forgets the healing mead.", "A calm camp can save a life." },
            new[] { "Take care of yourself out there.", "Good. Keep the others alive too." }),
        ["troldad"] = new(
            new[] { "yyyyy... no.. yyyy... tylko, że ten...", "No... już jestem, już jestem." },
            new[] { "yyyyy... no.. yyyy... tylko, że ten...", "Jackie, gdzie ja odłożyłem tę skrzynię?" },
            new[] { "Ten... dobry wybór. Chyba." }),
        ["grimvald"] = new(
            new[] { "Watch the roots. The forest claims careless feet." },
            new[] { "Every road through these trees has a price.", "The greydwarfs took my last delivery." },
            new[] { "Keep it dry on the road." }),
        ["rudy_warg"] = new(
            new[] { "Tracks are fresh. Something passed here at dawn." },
            new[] { "Never turn your back on a troll.", "A good bow is quieter than a good excuse." },
            new[] { "Aim true, traveler." }),
        ["mokra_dzika"] = new(
            new[] { "The swamp gives medicine and poison in equal measure." },
            new[] { "Those roots are useful. Mind the leeches.", "My herbs dislike bright sunlight." },
            new[] { "Keep the lid shut until you leave the marsh." }),
        ["encek"] = new(
            new[] { "sprawdzałeś Dreadborne Dungeon? Te dungeony to sanatorium..." },
            new[] { "Jeszcze jeden run. Tym razem na pewno bez zgona.", "sprawdzałeś Dreadborne Dungeon? Te dungeony to sanatorium..." },
            new[] { "Dobry zakup. Build zaczyna mieć sens." }),
        ["hrothgar"] = new(
            new[] { "The mountain remembers every careless step." },
            new[] { "Storm's coming. I can smell it.", "Wolves make better neighbors than drakes." },
            new[] { "Stay warm on the climb." }),
        ["ylva_frost"] = new(
            new[] { "Come to the fire before the frost finds you." },
            new[] { "Even crystal cracks in this cold.", "The onions survived. So will we." },
            new[] { "Wrap it well. The wind is cruel today." }),
        ["bjarki_goldtooth"] = new(
            new[] { "A full cart is a happy cart." },
            new[] { "Never trust a lox with your lunch.", "The barley grows faster than my debts." },
            new[] { "Gold well spent. Come back with the cart." }),
        ["ragnar_turnipson"] = new(
            new[] { "Mind the lox. It thinks the farm is its own." },
            new[] { "Barley today, bread tomorrow.", "Nothing grows without a little stubbornness." },
            new[] { "Fresh stock for a hungry journey." }),
        ["cmok"] = new(
            new[] { "No kurwa... znowu wszystkie dungi zajęte" },
            new[] { "Ja pulluje, wy walicie, nie rozumiem o co chodzi?", "No kurwa... znowu wszystkie dungi zajęte" },
            new[] { "Bierz sprzęt. W środku nie będzie czasu na zakupy." }),
        ["grelka"] = new(
            new[] { "If you hear a gjall, do not look up. Run." },
            new[] { "The mist hides the best mushrooms.", "I can find a road with my eyes closed." },
            new[] { "May the mist let you pass." }),
        ["spalony_zenek"] = new(
            new[] { "Everything burns here. Some things burn twice." },
            new[] { "I have survived worse. Probably.", "The ash gets into every lock." },
            new[] { "Do not leave that beside the fire." }),
        ["skjold_cinderborn"] = new(
            new[] { "I forge shelter where the earth is fire." },
            new[] { "A shield is only as good as the hand behind it.", "The forge does not sleep." },
            new[] { "Put it to work before the ash settles." })
    };

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
            _nextIdle = Time.time + UnityEngine.Random.Range(55f, 105f);
            Speak(lines.Greeting);
        }
        else if (distance < 15f && _greeted && Time.time >= _nextIdle)
        {
            _nextIdle = Time.time + UnityEngine.Random.Range(70f, 125f);
            Speak(lines.Idle);
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
        if (options.Length == 0 || Chat.instance == null || Time.time - _lastLine < 3f) return;
        _lastLine = Time.time;
        Chat.instance.SetNpcText(gameObject, new Vector3(0f, 2f, 0f),
            20f, 6f, "", options[UnityEngine.Random.Range(0, options.Length)], false);
    }
}
