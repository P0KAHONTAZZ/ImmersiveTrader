using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>Slow, subtle breathing of a dropped scroll's glow light.</summary>
public sealed class ScrollGlowPulse : MonoBehaviour
{
    private Light? _light;
    private float _base;
    private float _phase;

    private void Awake()
    {
        _light = GetComponent<Light>();
        _base = _light != null ? _light.intensity : 1f;
        _phase = Random.Range(0f, 6.28f);
    }

    private void Update()
    {
        if (_light == null) return;
        _light.intensity = _base * (0.8f + 0.2f * Mathf.Sin(Time.time * 2.2f + _phase));
    }
}
