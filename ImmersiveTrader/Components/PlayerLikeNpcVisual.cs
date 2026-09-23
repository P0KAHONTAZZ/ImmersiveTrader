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
    private Transform? _playerVisual;

    private void LateUpdate()
    {
        // Native NPC equipment/visual systems may re-enable shell renderers after Start.
        // Keep every shell/helper renderer hidden and allow only our mounted Player visual.
        if (_playerVisual == null) return;
        foreach (var renderer in GetComponentsInChildren<Renderer>(true))
        {
            if (!renderer.transform.IsChildOf(_playerVisual) && renderer.transform != _playerVisual)
                renderer.enabled = false;
        }
    }

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

        // Hide only Hildir's renderers; keep her root, colliders, animator and
        // interaction/network shell alive. This turns NPCLOOK into one visible body
        // instead of Hildir + Player occupying the same position.
        foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            renderer.enabled = false;

        // CharacterAnimEvent expects a complete Character/Humanoid owner during Awake.
        // It is unsafe on a detached visual hierarchy, so disable/remove those event
        // components on a temporary inactive template before instantiation.
        bool sourceActive = visual.gameObject.activeSelf;
        visual.gameObject.SetActive(false);
        var copy = Object.Instantiate(visual.gameObject, transform);
        visual.gameObject.SetActive(sourceActive);
        copy.name = "ImmersiveTrader_PlayerVisual";
        _playerVisual = copy.transform;
        copy.transform.localPosition = Vector3.zero;
        copy.transform.localRotation = Quaternion.identity;
        copy.transform.localScale = Vector3.one;

        foreach (var animEvent in copy.GetComponentsInChildren<CharacterAnimEvent>(true))
            Object.DestroyImmediate(animEvent);

        foreach (var nview in copy.GetComponentsInChildren<ZNetView>(true))
            Object.DestroyImmediate(nview);
        foreach (var player in copy.GetComponentsInChildren<Player>(true))
            Object.DestroyImmediate(player);

        foreach (var renderer in copy.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = true;

        copy.SetActive(true);
    }
}
