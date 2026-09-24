using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Keeps wild creatures outside an open trader camp. Runs only on the world host;
/// the existing location anchor controls its lifetime with the location.
/// </summary>
public sealed class TraderSanctuary : MonoBehaviour
{
    private const float SafeRadius = 11f;
    private const float ExitRadius = 15f;
    private readonly Collider[] _nearby = new Collider[128];
    private readonly HashSet<Character> _seen = new HashSet<Character>();
    private float _nextCheck;

    private void Update()
    {
        if (ZNet.instance == null || !ZNet.instance.IsServer() || Time.time < _nextCheck)
            return;

        _nextCheck = Time.time + 0.5f;
        _seen.Clear();
        var center = transform.position;
        int count = Physics.OverlapSphereNonAlloc(center, SafeRadius, _nearby);
        for (int i = 0; i < count; i++)
        {
            var character = _nearby[i] == null ? null : _nearby[i].GetComponentInParent<Character>();
            if (character == null || character.IsPlayer() ||
                character.GetComponent<BaseAI>() == null || !_seen.Add(character))
                continue;

            var offset = character.transform.position - center;
            offset.y = 0f;
            if (offset.sqrMagnitude >= SafeRadius * SafeRadius)
                continue;

            if (offset.sqrMagnitude < 0.01f)
                offset = Vector3.forward;
            var destination = center + offset.normalized * ExitRadius;
            destination.y = character.transform.position.y;
            character.transform.position = destination;
        }
    }
}
