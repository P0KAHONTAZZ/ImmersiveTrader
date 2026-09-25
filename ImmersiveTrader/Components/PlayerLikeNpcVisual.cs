using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using BepInEx;
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
        if (!TestOutfits.TryGetValue(TraderId, out var defaults)) return;
        var outfit = LoadOutfit(TraderId, defaults);

        var bones = visual.GetComponentsInChildren<Transform>(true)
            .GroupBy(bone => bone.name, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

        int attached = 0;
        if (AttachSkin(outfit.Chest, visual, bones)) attached++;
        if (AttachSkin(outfit.Legs, visual, bones)) attached++;
        bool helmet = EquipNativeHelmet(visual, TraderId, outfit.Helmet);
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

    [Serializable]
    public sealed class TraderOutfit
    {
        public string Helmet = "";
        public string Chest = "";
        public string Legs = "";
    }

    public static void InitializeOutfitFiles()
    {
        // Configuration belongs to the plugin, not to a loaded world zone.
        // Generate all human trader files on plugin startup so players can edit
        // them before traveling to a trader's location.
        foreach (var pair in TestOutfits)
            LoadOutfit(pair.Key, pair.Value);
    }

    private static TraderOutfit LoadOutfit(string traderId, (string Chest, string Legs) defaults)
    {
        TestHelmets.TryGetValue(traderId, out var defaultHelmet);
        var fallback = new TraderOutfit
        {
            Helmet = defaultHelmet ?? "",
            Chest = defaults.Chest,
            Legs = defaults.Legs
        };
        try
        {
            string directory = Path.Combine(Paths.ConfigPath, "ImmersiveTrader", "outfits");
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, traderId + ".json");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonUtility.ToJson(fallback, true));
                Plugin.Log.LogInfo($"Created trader outfit file: {path}");
                return fallback;
            }
            var loaded = JsonUtility.FromJson<TraderOutfit>(File.ReadAllText(path));
            if (loaded == null || loaded.Helmet == null || loaded.Chest == null || loaded.Legs == null)
                throw new InvalidDataException("Outfit must define Helmet, Chest and Legs.");
            return loaded;
        }
        catch (Exception error)
        {
            Plugin.Log.LogWarning($"Outfit configuration for {traderId} could not be read; using defaults: {error.Message}");
            return fallback;
        }
    }

    private static bool EquipNativeHelmet(Transform visual, string traderId, string prefabName)
    {
        if (string.IsNullOrWhiteSpace(prefabName)) return false;
        if (ObjectDB.instance?.GetItemPrefab(prefabName) == null)
        {
            Plugin.Log.LogWarning($"Outfit helmet prefab unavailable for {traderId}: {prefabName}");
            return false;
        }

        // The copied Player Visual may retain its native VisEquipment component.
        // Let Valheim attach the helmet through that component. A hand-mounted
        // mesh cannot be made reliable by adjusting a scale or bone offset.
        var equipment = visual.GetComponentInChildren<VisEquipment>(true);
        if (equipment == null)
        {
            Plugin.Log.LogWarning($"Native VisEquipment unavailable on Player Visual for {traderId}; helmet skipped.");
            return false;
        }
        try
        {
            var method = typeof(VisEquipment).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(candidate => candidate.Name == "SetHelmetItem" &&
                    candidate.GetParameters().Length > 0 &&
                    candidate.GetParameters()[0].ParameterType == typeof(string));
            if (method == null)
                throw new MissingMethodException("VisEquipment", "SetHelmetItem");
            var parameters = method.GetParameters();
            var values = new object[parameters.Length];
            values[0] = prefabName;
            for (int i = 1; i < parameters.Length; i++)
                values[i] = parameters[i].HasDefaultValue ? parameters[i].DefaultValue :
                    parameters[i].ParameterType.IsValueType ? Activator.CreateInstance(parameters[i].ParameterType) : null;
            method.Invoke(equipment, values);
            Plugin.Log.LogInfo($"Native helmet equipped for {traderId}: {prefabName}");
            return true;
        }
        catch (Exception error)
        {
            Plugin.Log.LogWarning($"Native helmet equip failed for {traderId}: {error}");
            return false;
        }
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
