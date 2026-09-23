using System.Linq;
using ImmersiveTrader.Models;
using UnityEngine;

namespace ImmersiveTrader.Components;

public sealed class TraderNpc : MonoBehaviour, Hoverable, Interactable
{
    public string TraderId = string.Empty;

    private TraderDefinition? Definition =>
        TraderRegistry.Traders.FirstOrDefault(x => x.Id == TraderId);

    public string GetHoverName() => Definition?.Name ?? "Trader";

    // Valheim positions the hover label from the hovered object's transform plus this
    // offset. Scale-aware offsets keep labels above creature-based trader models.
    public float GetHoverOffset()
    {
        var character = GetComponent<Character>();
        if (character != null)
        {
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                float top = collider.bounds.max.y - transform.position.y;
                if (top > 0.25f && top < 8f)
                    return top + 0.35f;
            }
        }

        float scale = Mathf.Max(0.35f, transform.lossyScale.y);
        return 1.8f * scale;
    }

    public string GetHoverText()
    {
        if (Definition == null) return string.Empty;
        if (TraderId == "midka")
            return $"{Definition.Name}\n[<color=yellow><b>$KEY_Use</b></color>] Talk\nWar healer & field medic";
        return $"{Definition.Name}\n[<color=yellow><b>$KEY_Use</b></color>] Talk";
    }

    public bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (hold || user is not Player player || Definition == null) return false;
        TraderInteraction.Handle(player, Definition, alt, transform.position);
        return true;
    }

    public bool UseItem(Humanoid user, ItemDrop.ItemData item) => false;
}
