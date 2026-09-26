using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Keeps a trader/companion NPC on dry land near its camp. Every 2 s (owner only):
/// if the NPC is in water, below sea level, or farther than LeashRadius from home,
/// it is moved back home. If home itself is wet (old worlds), the nearest dry ground
/// within 40 m is used as the new home.
/// </summary>
public sealed class NpcStayOnLand : MonoBehaviour
{
    public float LeashRadius = 10f;
    private Vector3 _home;
    private bool _homeSet;
    private float _next;
    private Character? _character;
    private ZNetView? _view;
    private Rigidbody? _body;

    public void SetHome(Vector3 home) { _home = home; _homeSet = true; }

    private void Start()
    {
        _character = GetComponent<Character>();
        _view = GetComponent<ZNetView>();
        _body = GetComponent<Rigidbody>();
        if (!_homeSet) SetHome(transform.position);
        _next = Time.time + 1f;
    }

    private void Update()
    {
        if (Time.time < _next) return;
        _next = Time.time + 2f;
        if (_view != null && _view.IsValid() && !_view.IsOwner()) return;
        if (ZoneSystem.instance == null) return;

        float water = ZoneSystem.instance.m_waterLevel;
        if (IsWet(_home, water) && TryFindDry(_home, water, out var dry)) _home = dry;

        bool inWater = (_character != null && _character.InWater()) || transform.position.y < water + 0.2f;
        bool tooFar = Vector3.Distance(Flat(transform.position), Flat(_home)) > LeashRadius;
        if (!inWater && !tooFar) return;

        var target = _home + Vector3.up * 0.2f;
        transform.position = target;
        if (_body != null)
        {
            _body.position = target;
            _body.velocity = Vector3.zero;
        }
    }

    private static Vector3 Flat(Vector3 v) => new(v.x, 0f, v.z);

    private static bool IsWet(Vector3 p, float water)
        => ZoneSystem.instance.GetGroundHeight(p) < water + 0.5f;

    private static bool TryFindDry(Vector3 around, float water, out Vector3 result)
    {
        for (float r = 4f; r <= 40f; r += 4f)
            for (int i = 0; i < 12; i++)
            {
                float a = i * Mathf.PI / 6f;
                var p = around + new Vector3(Mathf.Cos(a) * r, 0f, Mathf.Sin(a) * r);
                float h = ZoneSystem.instance.GetGroundHeight(p);
                if (h >= water + 1f) { result = new Vector3(p.x, h, p.z); return true; }
            }
        result = around;
        return false;
    }
}
