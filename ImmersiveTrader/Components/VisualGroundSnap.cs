using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Keeps a procedurally built ground model visually resting on the ground.
/// Physics stays untouched (collider on the item root); once the item is still, the visual
/// child is moved down by the measured gap between its bottom and the ground below.
/// </summary>
public sealed class VisualGroundSnap : MonoBehaviour
{
    public string VisualName = "";
    public float VisualBottomLocal;      // bottom of the visual in the visual root's local space

    private static int _mask = -1;
    private Transform? _visual;
    private Rigidbody? _body;
    private Vector3 _baseLocal;
    private float _nextCheck;
    private Vector3 _lastPos;

    private void Start()
    {
        _visual = transform.Find(VisualName);
        _body = GetComponent<Rigidbody>();
        if (_visual != null) _baseLocal = _visual.localPosition;
        if (_mask == -1) _mask = LayerMask.GetMask("terrain", "Default", "static_solid", "piece", "Default_small");
        _nextCheck = Time.time + 0.6f;
    }

    private void Update()
    {
        if (_visual == null || Time.time < _nextCheck) return;
        _nextCheck = Time.time + 0.5f;

        bool still = _body == null || _body.IsSleeping() || _body.velocity.sqrMagnitude < 0.0004f;
        if (!still || (transform.position - _lastPos).sqrMagnitude < 1e-6f && _lastPos != Vector3.zero) return;
        _lastPos = transform.position;

        _visual.localPosition = _baseLocal;
        float bottomWorld = _visual.TransformPoint(new Vector3(0f, VisualBottomLocal, 0f)).y;
        var origin = new Vector3(_visual.position.x, bottomWorld + 0.6f, _visual.position.z);
        if (!Physics.Raycast(origin, Vector3.down, out var hit, 1.6f, _mask, QueryTriggerInteraction.Ignore)) return;

        float gap = bottomWorld - hit.point.y;
        if (gap > 0.01f && gap < 0.6f)
        {
            // Convert world gap to the parent's local Y (handles scaled parents).
            float scaleY = Mathf.Max(0.0001f, transform.lossyScale.y);
            _visual.localPosition = _baseLocal - transform.InverseTransformDirection(Vector3.up) * (gap / scaleY);
        }
    }
}
