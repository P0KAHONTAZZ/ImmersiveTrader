using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Keeps Jackie as a loose ambient companion around Troldad without allowing him
/// to wander away with the player. Vanilla wolf AI still provides movement/animation.
/// </summary>
public sealed class JackieCompanion : MonoBehaviour
{
    public Transform? Home;
    public float SoftRadius = 5f;
    public float HardRadius = 11f;

    private BaseAI? _ai;
    private Vector3 _spawnHome;
    private float _nextCheck;

    private void Awake()
    {
        _ai = GetComponent<BaseAI>();
        _spawnHome = transform.position;
    }

    public void SetHome(Transform home)
    {
        Home = home;
        _spawnHome = home.position;
    }

    private void Update()
    {
        if (Time.time < _nextCheck) return;
        _nextCheck = Time.time + 1f;

        Vector3 home = Home != null ? Home.position : _spawnHome;
        float distance = Vector3.Distance(transform.position, home);

        if (distance > HardRadius)
        {
            transform.position = home + new Vector3(1.8f, 0.2f, 1.2f);
            return;
        }

        if (_ai != null && distance > SoftRadius)
            _ai.MoveTo(Time.fixedDeltaTime, home, 0.6f, false);
    }
}
