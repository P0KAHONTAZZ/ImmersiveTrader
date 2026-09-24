using BepInEx;
using BepInEx.Configuration;
using Jotunn;
using Jotunn.Managers;
using ImmersiveTrader.Commands;
using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader;

[BepInPlugin(ModGuid, ModName, ModVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
public sealed class Plugin : BaseUnityPlugin
{
    private Harmony? _harmony;
    private bool _rewardValidationPending;
    internal static BepInEx.Logging.ManualLogSource Log = null!;
    public const string ModGuid = "p0kahontazz.immersivetrader";
    public const string ModName = "ImmersiveTrader";
    public const string ModVersion = "0.5.0";

    internal static ConfigEntry<float> TreasureWeight = null!;
    internal static ConfigEntry<int> MaxCarriedTreasures = null!;
    internal static ConfigEntry<int> MidkaHealingMeadPrice = null!;
    internal static ConfigEntry<bool> ProgressionLock = null!;
    internal static ConfigEntry<int> TraderCooldownWorldDays = null!;
    internal static ConfigEntry<int> MietegActiveWorldDays = null!;
    internal static ConfigEntry<float> MietegRevealDistance = null!;

    private void Awake()
    {
        Log = Logger;
        TreasureWeight = Config.Bind("Treasures", "Weight", 80f, "Weight of each quest treasure.");
        MaxCarriedTreasures = Config.Bind("Treasures", "MaxCarried", 2, "Maximum number of active quest treasures.");
        MidkaHealingMeadPrice = Config.Bind("Midka", "MinorHealingMeadPrice", 70, "Legacy configuration value. Midka's native shop always sells Minor Healing Mead for 70 coins.");
        ProgressionLock = Config.Bind("Progression", "Enabled", true, "Prevent rewards from biomes not unlocked by boss progression.");
        TraderCooldownWorldDays = Config.Bind("Quests", "TraderCooldownWorldDays", 3, "World days before the same trader can issue another shipment to a player.");
        MietegActiveWorldDays = Config.Bind("LegendaryMieteg", "ActiveWorldDays", 2, "How many world days Mieteg remains active.");
        MietegRevealDistance = Config.Bind("LegendaryMieteg", "RevealDistance", 600f, "Distance in metres at which Mieteg becomes discoverable.");

        _harmony = new Harmony(ModGuid);
        _harmony.PatchAll();

        CommandManager.Instance.AddConsoleCommand(new ImmersiveTraderCommand());
        PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;
        ZoneManager.OnVanillaLocationsAvailable += OnVanillaLocationsAvailable;
        Logger.LogInfo($"{ModName} {ModVersion} loaded.");
    }

    private void OnDestroy()
    {
        PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;
        ZoneManager.OnVanillaLocationsAvailable -= OnVanillaLocationsAvailable;
        _harmony?.UnpatchSelf();
    }

    private void OnVanillaPrefabsAvailable()
    {
        TreasureRegistry.Register();
        ContractRegistry.Register();
        TraderRegistry.Initialize();
        TraderActivityRegistry.Validate();
        RewardRegistry.Initialize();
        _rewardValidationPending = true;
        NpcPrefabRegistry.Register();
        LegendaryMietegRegistry.RegisterPrefab();
        PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;
    }

    private void Update()
    {
        if (!_rewardValidationPending || ObjectDB.instance == null)
            return;

        // PrefabManager's vanilla-prefab event fires before ObjectDB's runtime item
        // lookup is guaranteed to be populated. Wait until a known vanilla item resolves
        // before validating all 910 reward routes, otherwise every reward is a false miss.
        if (ObjectDB.instance.GetItemPrefab("Coins") == null)
            return;

        RewardRegistry.Validate();
        _rewardValidationPending = false;
    }

    private void OnVanillaLocationsAvailable()
    {
        TraderLocationRegistry.Register();
        LegendaryMietegRegistry.RegisterLocations();
        ZoneManager.OnVanillaLocationsAvailable -= OnVanillaLocationsAvailable;
    }
}
