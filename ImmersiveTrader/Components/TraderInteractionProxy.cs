using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Dedicated hover/use surface for creature-based traders whose native hit colliders
/// do not reliably resolve an Interactable on the prefab root (notably Troll).
/// </summary>
public sealed class TraderInteractionProxy : MonoBehaviour, Hoverable, Interactable
{
    public TraderNpc? Owner;

    public string GetHoverName() => Owner?.GetHoverName() ?? string.Empty;
    public string GetHoverText() => Owner?.GetHoverText() ?? string.Empty;
    public float GetHoverOffset() => 0f;

    public bool Interact(Humanoid user, bool hold, bool alt)
        => Owner != null && Owner.Interact(user, hold, alt);

    public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        => Owner != null && Owner.UseItem(user, item);
}
