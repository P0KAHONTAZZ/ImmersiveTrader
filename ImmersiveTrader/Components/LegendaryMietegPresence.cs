using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Keeps each prepared Mieteg site deterministic while ensuring that only one
/// site is active in a given world-day window. No extra network state is needed:
/// every peer derives the same active slot from network time.
/// </summary>
public sealed class LegendaryMietegPresence : MonoBehaviour
{
    public int SiteIndex;
    public int SiteCount = 8;

    private Renderer[] _renderers = System.Array.Empty<Renderer>();
    private Collider[] _colliders = System.Array.Empty<Collider>();
    private TraderNpc? _trader;
    private bool _active;
    private bool _pinAdded;
    private Minimap.PinData? _pin;
    private float _nextCheck;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
        _colliders = GetComponentsInChildren<Collider>(true);
        _trader = GetComponent<TraderNpc>();
        Refresh(true);
    }

    private void Update()
    {
        if (Time.time < _nextCheck) return;
        _nextCheck = Time.time + 2f;
        Refresh(false);
    }

    private void Refresh(bool force)
    {
        if (ZNet.instance == null) return;

        int activeDays = Mathf.Max(1, Plugin.MietegActiveWorldDays.Value);
        int worldDay = Mathf.Max(0, (int)(ZNet.instance.GetTimeSeconds() / 1800d));
        int window = worldDay / activeDays;
        int activeSite = PositiveModulo(StableWorldSeed() + window, Mathf.Max(1, SiteCount));
        bool shouldBeActive = SiteIndex == activeSite;

        if (!force && shouldBeActive == _active) return;
        _active = shouldBeActive;

        foreach (var renderer in _renderers) renderer.enabled = _active;
        foreach (var collider in _colliders) collider.enabled = _active;
        if (_trader != null) _trader.enabled = _active;
        if (!_active) RemovePin();
        else RefreshDiscoveryPin();
    }

    private void RefreshDiscoveryPin()
    {
        if (_pinAdded || Minimap.instance == null || Player.m_localPlayer == null) return;
        float distance = Vector3.Distance(Player.m_localPlayer.transform.position, transform.position);
        if (distance > Plugin.MietegRevealDistance.Value) return;

        _pin = Minimap.instance.AddPin(transform.position, Minimap.PinType.Icon3, "✦ Legendary Mieteg", false, false);
        _pinAdded = _pin != null;
    }

    private void RemovePin()
    {
        if (!_pinAdded || _pin == null || Minimap.instance == null) return;
        Minimap.instance.RemovePin(_pin);
        _pin = null;
        _pinAdded = false;
    }

    private void OnDestroy() => RemovePin();

    private static int StableWorldSeed()
    {
        // ZNet network time makes the rotation synchronized. World name hash adds
        // deterministic variation between worlds without storing extra state.
        string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "world";
        unchecked
        {
            int hash = 17;
            foreach (char c in worldName) hash = hash * 31 + c;
            return hash;
        }
    }

    private static int PositiveModulo(int value, int modulus)
    {
        int result = value % modulus;
        return result < 0 ? result + modulus : result;
    }
}
