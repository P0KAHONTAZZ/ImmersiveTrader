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
    public float GetHoverOffset() => 1.8f;

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
        TraderInteraction.Handle(player, Definition, alt);
        return true;
    }

    public bool UseItem(Humanoid user, ItemDrop.ItemData item) => false;
}