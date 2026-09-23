using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Experimental human-looking presentation for NPCLOOK shells. The shell remains Hildir
/// and therefore owns interaction/networking; this component only mounts the vanilla
/// Player visual hierarchy as a non-controlling presentation object.
/// </summary>
public sealed class PlayerLikeNpcVisual : MonoBehaviour
{
    public string TraderId = string.Empty;

    private void Start()
    {
        if (transform.Find("ImmersiveTrader_PlayerVisual") != null) return;
        var playerPrefab = PrefabManager.Instance.GetPrefab(NpcPrefabRegistry.PlayerStyleVisualSource);
        if (playerPrefab == null) return;

        // Never instantiate the Player root: it contains input, inventory and player
        // networking. Copy only its visual child when one can be identified safely.
        Transform? visual = playerPrefab.transform.Find("Visual");
        if (visual == null)
        {
            Plugin.Log.LogWarning($"Player visual child not found; player-like look skipped for {TraderId}.");
            return;
        }

        var copy = Object.Instantiate(visual.gameObject, transform);
        copy.name = "ImmersiveTrader_PlayerVisual";
        copy.transform.localPosition = Vector3.zero;
        copy.transform.localRotation = Quaternion.identity;
        copy.transform.localScale = Vector3.one;

        foreach (var nview in copy.GetComponentsInChildren<ZNetView>(true))
            Object.DestroyImmediate(nview);
        foreach (var player in copy.GetComponentsInChildren<Player>(true))
            Object.DestroyImmediate(player);
    }
}
