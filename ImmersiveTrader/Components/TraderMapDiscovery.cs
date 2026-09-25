using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>Locally saves a named trader pin when the player discovers the camp.</summary>
public sealed class TraderMapDiscovery : MonoBehaviour
{
    public string TraderId = "";
    private float _nextCheck;
    private object _pin;
    private Minimap _map;
    private bool _styleWarning;
    private static readonly BindingFlags Fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private void Update()
    {
        if (Time.time < _nextCheck) return;
        _nextCheck = Time.time + 0.5f;
        var map = Minimap.instance;
        var player = Player.m_localPlayer;
        if (map == null || player == null || string.IsNullOrEmpty(TraderId)) return;
        var trader = TraderRegistry.Traders.FirstOrDefault(x => x.Id == TraderId);
        if (trader == null || trader.IsLegendary) return;
        if (_map != map) { _map = map; _pin = null; }
        if (_pin == null && Vector3.Distance(player.transform.position, transform.position) <= 35f)
        {
            _pin = FindExisting(map, trader.Name, transform.position) ??
                map.AddPin(transform.position, Minimap.PinType.Icon3, trader.Name, true, false, 0L);
            Plugin.Log.LogInfo($"Trader map discovery: {trader.Name}.");
        }
        if (_pin != null)
        {
            try { StylePin(map, _pin); }
            catch (Exception error)
            {
                if (!_styleWarning) Plugin.Log.LogWarning($"Trader map pin styling unavailable: {error.Message}");
                _styleWarning = true;
            }
        }
    }

    private static object FindExisting(Minimap map, string name, Vector3 position)
    {
        if (Read(map, "m_pins") is not IEnumerable pins) return null;
        foreach (var pin in pins)
        {
            if (pin == null || Read(pin, "m_name") is not string pinName || pinName != name) continue;
            if (Read(pin, "m_pos") is Vector3 pos && Vector3.Distance(pos, position) < 20f)
                return pin;
        }
        return null;
    }

    private static void StylePin(Minimap map, object pin)
    {
        if (Read(pin, "m_icon") is Sprite icon && icon.name != "ImmersiveTraderIcon")
        {
            if (Read(map, "m_locationIcons") is IEnumerable icons)
                foreach (var entry in icons)
                {
                    if (entry == null || Read(entry, "m_name") is not string name ||
                        name.IndexOf("Haldor", StringComparison.OrdinalIgnoreCase) < 0 &&
                        name.IndexOf("Vendor_BlackForest", StringComparison.OrdinalIgnoreCase) < 0 ||
                        Read(entry, "m_icon") is not Sprite haldorIcon) continue;
                    Write(pin, "m_icon", haldorIcon);
                    break;
                }
        }

        // Pin UI is recreated by Valheim as the map opens/closes, so set the
        // appearance again on every map update. These changes affect this pin only.
        var element = Read(pin, "m_iconElement");
        if (element != null)
        {
            var sprite = Read(pin, "m_icon");
            element.GetType().GetProperty("sprite", Fields)?.SetValue(element, sprite);
            element.GetType().GetProperty("color", Fields)?.SetValue(element,
                new Color(0.35f, 0.85f, 0.72f, 1f));
            if (element is Component image)
                image.transform.localScale = Vector3.one * 1.5f;
        }
    }

    private static object Read(object target, string name)
        => target.GetType().GetField(name, Fields)?.GetValue(target);

    private static void Write(object target, string name, object value)
        => target.GetType().GetField(name, Fields)?.SetValue(target, value);
}
