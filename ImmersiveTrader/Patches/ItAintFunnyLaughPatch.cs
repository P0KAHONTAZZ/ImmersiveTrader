using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// It ain't funny: while the 20 minute status is active, replay the vanilla laugh emote sound
/// at a fresh random interval of 2-5 seconds after each laugh. The emote is stopped immediately
/// so it contributes its vanilla voice without locking the player into the laugh animation.
/// </summary>
[HarmonyPatch(typeof(Player), nameof(Player.Update))]
internal static class ItAintFunnyLaughPatch
{
    private static float _nextLaugh;
    private static bool _wasActive;

    private static void Postfix(Player __instance)
    {
        if (__instance != Player.m_localPlayer) return;

        bool active = EnhancementStatusRegistry.Has(__instance, "laugh");
        if (!active)
        {
            _wasActive = false;
            _nextLaugh = 0f;
            return;
        }

        if (!_wasActive)
        {
            _wasActive = true;
            _nextLaugh = Time.time + Random.Range(2f, 5f);
            return;
        }

        if (Time.time < _nextLaugh) return;
        _nextLaugh = Time.time + Random.Range(2f, 5f);

        // StartEmote is the vanilla path used by /laugh. Stop it in the same update so
        // movement/combat/building/interactions remain available while its voice is triggered.
        __instance.StartEmote("laugh", false);
        __instance.StopEmote();
    }
}
