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

    // Hoverable.GetHoverOffset is a UI/world-space offset, not a collider-height probe.
    // Creature prefabs have very different collider hierarchies; using Collider.bounds
    // made labels from Troll/Draugr/Dverger based traders jump or stack on screen.
    // Keep the interaction anchor deterministic per visual type instead.
    public float GetHoverOffset() => TraderId switch
    {
        "troldad" => 1.35f,
        "hrothgar" => 1.65f,
        "ragnar_turnipson" => 1.75f,
        "bjarki_goldtooth" => 1.55f,
        "encek" => 1.65f,
        "mokra_dzika" => 1.75f,
        _ => 1.65f
    };

    public string GetHoverText()
    {
        if (Definition == null) return string.Empty;
        var localPlayer = Player.m_localPlayer;
        if (!ProgressionGate.IsRewardTierUnlocked(Definition.BiomeTier))
            return $"{Definition.Name}\nTrading locked: defeat the previous biome boss.";
        string reputation = localPlayer == null ? "" : $"\nReputation: {TraderReputation.DescribeStanding(localPlayer, TraderId)}";
        return $"{Definition.Name}\n[<color=yellow><b>E</b></color>] Talk\n{Definition.Theme}{reputation}";
    }

    public bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (hold || user is not Player player || Definition == null) return false;
        if (!ProgressionGate.IsRewardTierUnlocked(Definition.BiomeTier))
        {
            player.Message(MessageHud.MessageType.Center,
                $"{Definition.Name}: defeat the previous biome boss before trading.");
            return true;
        }
        // Delivery has priority over opening StoreGui. A player carrying cargo for
        // this trader can therefore hand it in with the same normal E interaction.
        // Without this check the native shop consumed every primary interaction first,
        // making QuestDelivery unreachable for both Midka and Troldad.
        if (!alt && QuestDelivery.TryDeliverAny(player, Definition, transform.position))
            return true;

        if (!alt && TraderActivityService.TryTurnInPhysical(player, Definition.Id))
            return true;

        // With no deliverable cargo, primary E remains the familiar native Valheim shop.
        if (!alt && NativeTraderWindow.TryOpen(player, Definition, gameObject))
            return true;

        TraderInteraction.Handle(player, Definition, alt, transform.position);
        return true;
    }

    public bool UseItem(Humanoid user, ItemDrop.ItemData item) => false;
}
