using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Keeps Jackie as an ambient wolf companion around Troldad.
/// Native Wolf/MonsterAI remains responsible for locomotion and animation.
/// This component only maintains a home anchor and performs a rare safety reset
/// if the vanilla AI ever carries Jackie far outside Troldad's camp.
/// </summary>
public sealed class JackieCompanion : MonoBehaviour
{
    public Transform? Home;
    public float HardRadius = 12f;
    public float CheckInterval = 2f;

    private MonsterAI? _ai;
    private ZNetView? _nview;
    private Vector3 _spawnHome;
    private float _nextCheck;
    private Character? _character;

    private void Awake()
    {
        _ai = GetComponent<MonsterAI>();
        _nview = GetComponent<ZNetView>();
        _character = GetComponent<Character>();
        _spawnHome = transform.position;

        // Jackie is scenery/companionship, not a tameable pet that can be
        // commanded or taken away by a player.
        var tameable = GetComponent<Tameable>();
        if (tameable != null)
            Destroy(tameable);
    }

    public void SetHome(Transform home)
    {
        Home = home;
        _spawnHome = home.position;
        // Native wolf AI remains in control of idle/wander movement.
    }

    private void Start()
    {
        // Intentionally empty: vanilla MonsterAI owns ordinary locomotion.
    }

    private void Update()
    {
        // Jackie is an ambient camp companion, not a combat/survival target.
        // Camp fires and other environmental damage must never kill him.
        if (_character != null && _character.GetHealth() < _character.GetMaxHealth())
            _character.SetHealth(_character.GetMaxHealth());
        if (Time.time < _nextCheck) return;
        _nextCheck = Time.time + CheckInterval;

        Vector3 home = Home != null ? Home.position : _spawnHome;
        if (Vector3.Distance(transform.position, home) <= HardRadius) return;

        // Network owner is authoritative. This is only a failsafe; ordinary
        // movement inside the camp remains entirely vanilla AI.
        if (_nview != null && _nview.IsValid() && !_nview.IsOwner()) return;

        transform.position = home + new Vector3(1.8f, 0.2f, 1.2f);
        // After a safety reset, native AI resumes ordinary movement.
    }
}
