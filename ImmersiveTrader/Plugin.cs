using BepInEx;
using BepInEx.Configuration;
using Jotunn;
using Jotunn.Managers;
using Jotunn.Utils;
using ImmersiveTrader.Commands;
using HarmonyLib;
using UnityEngine;

namespace ImmersiveTrader;

[BepInPlugin(ModGuid, ModName, ModVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
public sealed class Plugin : BaseUnityPlugin
{
    private Harmony? _harmony;
    private bool _rewardValidationPending;
    internal static BepInEx.Logging.ManualLogSource Log = null!;
    public const string ModGuid = "p0kahontazz.immersivetrader";
    public const string ModName = "ImmersiveTrader";
    public const string ModVersion = "0.6.0";

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
        Components.PlayerLikeNpcVisual.InitializeOutfitFiles();
        var serverSynced = new ConfigurationManagerAttributes { IsAdminOnly = true };
        TreasureWeight = Config.Bind("Treasures", "Weight", 80f,
            new ConfigDescription("Weight of each quest treasure.", null, serverSynced));
        MaxCarriedTreasures = Config.Bind("Treasures", "MaxCarried", 2,
            new ConfigDescription("Maximum number of active quest treasures.", null, serverSynced));
        MidkaHealingMeadPrice = Config.Bind("Midka", "MinorHealingMeadPrice", 70,
            new ConfigDescription("Legacy configuration value. Midka's native shop always sells Minor Healing Mead for 70 coins.", null, serverSynced));
        ProgressionLock = Config.Bind("Progression", "Enabled", true,
            new ConfigDescription("Prevent rewards from biomes not unlocked by boss progression.", null, serverSynced));
        TraderCooldownWorldDays = Config.Bind("Quests", "TraderCooldownWorldDays", 3,
            new ConfigDescription("World days before the same trader can issue another shipment to a player.", null, serverSynced));
        MietegActiveWorldDays = Config.Bind("LegendaryMieteg", "ActiveWorldDays", 2,
            new ConfigDescription("How many world days Mieteg remains active.", null, serverSynced));
        MietegRevealDistance = Config.Bind("LegendaryMieteg", "RevealDistance", 600f,
            new ConfigDescription("Distance in metres at which Mieteg becomes discoverable.", null, serverSynced));

        MultiplayerNetwork.Register();

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
        EnhancementStatusRegistry.Register();
        EnhancementScrollItems.Register();
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
        TraderShopRegistry.Validate();
        _rewardValidationPending = false;
    }

    private void OnVanillaLocationsAvailable()
    {
        TraderLocationRegistry.Register();
        LegendaryMietegRegistry.RegisterLocations();
        ZoneManager.OnVanillaLocationsAvailable -= OnVanillaLocationsAvailable;
    }
}
