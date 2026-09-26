using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Handles only what the native Consumable flow cannot:
///  - scroll read while its buff is active: refresh to full duration (no stacking), consume 1;
///  - Scroll of Rested: native Rested with a fixed 20 minutes (vanilla would use comfort-based time).
/// A fresh non-Rested scroll returns true here and runs the untouched vanilla ConsumeItem.
/// </summary>
[HarmonyPatch]
internal static class EnhancementScrollConsumePatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        typeof(Player).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.Name == "ConsumeItem" && m.ReturnType == typeof(bool) &&
                        m.GetParameters().Any(p => p.ParameterType == typeof(ItemDrop.ItemData)) &&
                        m.GetParameters().Any(p => p.ParameterType == typeof(Inventory)));

    private static bool Prepare()
    {
        if (TargetMethods().Any()) return true;
        Plugin.Log.LogWarning("Enhancement scrolls: Player.ConsumeItem not found; refresh/Rested handling disabled.");
        return false;
    }

    private static bool Prefix(Player __instance, object[] __args, ref bool __result)
    {
        try
        {
            if (!ReferenceEquals(__instance, Player.m_localPlayer)) return true;
            var item = __args.OfType<ItemDrop.ItemData>().FirstOrDefault();
            var inventory = __args.OfType<Inventory>().FirstOrDefault();
            string? id = EnhancementScrollItems.IdFor(item);
            if (id == null || item == null || inventory == null) return true;

            bool handled;
            if (EnhancementStatusRegistry.TryRefresh(__instance, id))
                handled = true;
            else if (id == "rested")
                handled = EnhancementStatusRegistry.Apply(__instance, "rested", EnhancementStatusRegistry.RestedDuration, out _);
            else
                return true; // fresh buff: native flow

            if (!handled) { __result = false; return false; }
            inventory.RemoveOneItem(item);
            __instance.Message(MessageHud.MessageType.TopLeft,
                $"{EnhancementStatusRegistry.DisplayName(id)}: {(id == "rested" ? "20" : "30")} min");
            __result = true;
            return false;
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning("Enhancement scroll consume handling failed, using vanilla: " + e.Message);
            return true;
        }
    }
}
