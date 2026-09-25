using System;
using System.Collections.Generic;
using System.Linq;
using Jotunn.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

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
        ApplyTestOutfit(copy.transform);
    }
    private static readonly Dictionary<string, (string Chest, string Legs)> TestOutfits = new()
    {
        ["midka"] = ("ArmorLeatherChest", "ArmorLeatherLegs"),
        ["grimvald"] = ("ArmorTrollLeatherChest", "ArmorTrollLeatherLegs"),
        ["rudy_warg"] = ("ArmorBronzeChest", "ArmorBronzeLegs"),
        ["mokra_dzika"] = ("ArmorRootChest", "ArmorRootLegs"),
        ["encek"] = ("ArmorIronChest", "ArmorIronLegs"),
        ["hrothgar"] = ("ArmorWolfChest", "ArmorWolfLegs"),
        ["ylva_frost"] = ("ArmorFenringChest", "ArmorFenringLegs"),
        ["bjarki_goldtooth"] = ("ArmorPaddedCuirass", "ArmorPaddedGreaves"),
        ["ragnar_turnipson"] = ("ArmorFenringChest", "ArmorFenringLegs"),
        ["cmok"] = ("ArmorMageChest", "ArmorMageLegs"),
        ["grelka"] = ("ArmorCarapaceChest", "ArmorCarapaceLegs"),
        ["spalony_zenek"] = ("ArmorFlametalChest", "ArmorFlametalLegs"),
        ["skjold_cinderborn"] = ("ArmorMageChest_Ashlands", "ArmorMageLegs_Ashlands")
    };

    private void ApplyTestOutfit(Transform visual)
    {
        if (!TestOutfits.TryGetValue(TraderId, out var outfit)) return;

        var bones = visual.GetComponentsInChildren<Transform>(true)
            .GroupBy(bone => bone.name, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

        int attached = 0;
        if (AttachSkin(outfit.Chest, visual, bones)) attached++;
        if (AttachSkin(outfit.Legs, visual, bones)) attached++;
        bool helmet = AttachHelmet(TraderId, bones);
        Plugin.Log.LogInfo($"Test outfit {TraderId}: {attached}/2 armor pieces, helmet={helmet} ({outfit.Chest}, {outfit.Legs}).");
    }

    private static readonly Dictionary<string, string> TestHelmets = new()
    {
        ["midka"] = "HelmetLeather", ["grimvald"] = "HelmetTrollLeather",
        ["rudy_warg"] = "HelmetBronze", ["mokra_dzika"] = "HelmetRoot",
        ["encek"] = "HelmetIron", ["hrothgar"] = "HelmetDrake",
        ["ylva_frost"] = "HelmetFenring", ["bjarki_goldtooth"] = "HelmetPadded",
        ["ragnar_turnipson"] = "HelmetPadded", ["cmok"] = "HelmetMage",
        ["grelka"] = "HelmetCarapace", ["spalony_zenek"] = "HelmetFlametal",
        ["skjold_cinderborn"] = "HelmetMage_Ashlands"
    };

    private static bool AttachHelmet(string traderId, Dictionary<string, Transform> bones)
    {
        if (!TestHelmets.TryGetValue(traderId, out var name)) return false;
        if (!bones.TryGetValue("Head", out var head))
        {
            Plugin.Log.LogWarning($"Test outfit head bone unavailable for {traderId}.");
            return false;
        }

        var item = PrefabManager.Instance.GetPrefab(name);
        var attachment = item == null ? null : item.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(child => child.name == "attach");
        if (attachment == null)
        {
            Plugin.Log.LogWarning($"Test outfit helmet attachment unavailable: {name}");
            return false;
        }

        // Native equipment attaches the wearable child to the animated head socket.
        // The item-drop hierarchy's local transform belongs to its inventory/world
        // model, so copying that transform or recentering world bounds breaks alignment.
        var helmet = Object.Instantiate(attachment.gameObject, head, false);
        helmet.name = $"ImmersiveTrader_Outfit_{name}";
        helmet.transform.localPosition = Vector3.zero;
        helmet.transform.localRotation = Quaternion.identity;
        helmet.transform.localScale = Vector3.one;

        foreach (var part in helmet.GetComponentsInChildren<Transform>(true))
            part.gameObject.SetActive(true);

        var renderers = helmet.GetComponentsInChildren<Renderer>(true)
            .Where(renderer => renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
            .ToArray();
        if (renderers.Length == 0)
        {
            Object.Destroy(helmet);
            Plugin.Log.LogWarning($"Test helmet has no wearable mesh: {name}");
            return false;
        }

        foreach (var renderer in renderers)
            renderer.enabled = true;
        Plugin.Log.LogInfo($"Test helmet {traderId}: {name} attached to animated Head bone.");
        return true;
    }

    private static bool AttachSkin(string prefabName, Transform visual,
        Dictionary<string, Transform> bones)
    {
        var item = PrefabManager.Instance.GetPrefab(prefabName);
        var skin = item == null ? null : item.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(child => child.name == "attach_skin");
        if (skin == null)
        {
            Plugin.Log.LogWarning($"Test outfit prefab/attach_skin unavailable: {prefabName}");
            return false;
        }

        // Equipment prefabs contain networked dropped-item roots. Copy only their
        // attachment mesh, then bind its skinned meshes to the NPC's visual bones.
        var mounted = Object.Instantiate(skin.gameObject, visual);
        mounted.name = $"ImmersiveTrader_Outfit_{prefabName}";
        mounted.transform.localPosition = Vector3.zero;
        mounted.transform.localRotation = Quaternion.identity;
        mounted.transform.localScale = skin.localScale;

        var meshes = mounted.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (meshes.Length == 0)
        {
            Object.Destroy(mounted);
            Plugin.Log.LogWarning($"Test outfit has no skinned mesh: {prefabName}");
            return false;
        }

        foreach (var mesh in meshes)
        {
            var original = mesh.bones;
            var mapped = new Transform[original.Length];
            for (int i = 0; i < original.Length; i++)
            {
                if (original[i] == null || !bones.TryGetValue(original[i].name, out mapped[i]))
                {
                    Object.Destroy(mounted);
                    Plugin.Log.LogWarning($"Test outfit bone unavailable: {prefabName} / {original[i]?.name}");
                    return false;
                }
            }

            mesh.bones = mapped;
            if (mesh.rootBone != null && bones.TryGetValue(mesh.rootBone.name, out var root))
                mesh.rootBone = root;
            mesh.enabled = true;
        }

        // Vanilla equipment attachment objects are disabled inside item prefabs.
        // Activating only their root leaves the actual mesh hidden.
        foreach (var part in mounted.GetComponentsInChildren<Transform>(true))
            part.gameObject.SetActive(true);
        foreach (var mesh in mounted.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            mesh.enabled = true;
            mesh.updateWhenOffscreen = true;
        }
        return true;
    }

}
