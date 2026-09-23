using HarmonyLib;

namespace ImmersiveTrader.Patches;

/// <summary>
/// Counts a hunt kill only when Valheim reports the local player as the last attacker.
/// Prefix is used because Character death cleanup can clear attacker state afterwards.
/// </summary>
[HarmonyPatch(typeof(Character), "OnDeath")]
internal static class TraderActivityKillPatch
{
    private static void Prefix(Character __instance)
    {
        var local = Player.m_localPlayer;
        if (local == null || __instance == null || __instance.IsPlayer()) return;

        Character attacker = __instance.GetLastAttacker();
        if (attacker != local) return;

        string prefabName = Utils.GetPrefabName(__instance.gameObject);
        TraderActivityService.RegisterKill(local, prefabName);
    }
}
