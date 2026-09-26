using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Keeps a trader/companion NPC on dry land near its camp.
/// Movement is clamped to the leash edge instead of teleporting to the camp centre.
/// While this NPC owns the currently open trader window, its movement is held in place.
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
    private Vector3 _shopPosition;
    private bool _shopHeld;

    public void SetHome(Vector3 home) { _home = home; _homeSet = true; }

    private void Start()
    {
        _character = GetComponent<Character>();
        _view = GetComponent<ZNetView>();
        _body = GetComponent<Rigidbody>();
        if (!_homeSet) SetHome(transform.position);
        _next = Time.time + 0.5f;
    }

    private void Update()
    {
        var active = NativeTraderWindow.ActiveNpc;
        bool shopOpen = active != null &&
            (active == gameObject || active.transform.IsChildOf(transform) || transform.IsChildOf(active.transform));

        if (shopOpen)
        {
            if (!_shopHeld)
            {
                _shopPosition = transform.position;
                _shopHeld = true;
            }
            HoldAt(_shopPosition);
            return;
        }
        _shopHeld = false;

        if (Time.time < _next) return;
        _next = Time.time + 0.5f;
        if (_view != null && _view.IsValid() && !_view.IsOwner()) return;
        if (ZoneSystem.instance == null) return;

        float water = ZoneSystem.instance.m_waterLevel;
        if (IsWet(_home, water) && TryFindDry(_home, water, out var dry)) _home = dry;

        Vector3 current = transform.position;
        Vector3 flatDelta = Flat(current) - Flat(_home);
        float distance = flatDelta.magnitude;
        bool inWater = (_character != null && _character.InWater()) || current.y < water + 0.2f;

        if (!inWater && distance <= LeashRadius) return;

        Vector3 target;
        if (inWater || distance < 0.01f)
        {
            target = _home;
        }
        else
        {
            Vector3 edge = Flat(_home) + flatDelta.normalized * Mathf.Max(0.5f, LeashRadius - 0.5f);
            float ground = ZoneSystem.instance.GetGroundHeight(edge);
            target = new Vector3(edge.x, ground, edge.z);
        }

        target.y += 0.2f;
        HoldAt(target);
    }

    private void HoldAt(Vector3 target)
    {
        transform.position = target;
        if (_body != null)
        {
            _body.position = target;
            _body.velocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
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
