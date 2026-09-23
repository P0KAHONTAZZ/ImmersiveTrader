using BepInEx;
using BepInEx.Configuration;
using Jotunn;
using Jotunn.Managers;
using ImmersiveTrader.Commands;

namespace ImmersiveTrader;

[BepInPlugin(ModGuid, ModName, ModVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string ModGuid = "p0kahontazz.immersivetrader";
    public const string ModName = "ImmersiveTrader";
    public const string ModVersion = "0.4.0";

    internal static ConfigEntry<float> TreasureWeight = null!;
    internal static ConfigEntry<int> MaxCarriedTreasures = null!;
    internal static ConfigEntry<int> MidkaHealingMeadPrice = null!;
    internal static ConfigEntry<bool> ProgressionLock = null!;
    internal static ConfigEntry<int> TraderCooldownWorldDays = null!;\n    internal static ConfigEntry<int> ReputationBonusPerDeliveryPercent = null!;
    internal static ConfigEntry<int> MietegActiveWorldDays = null!;
    internal static ConfigEntry<float> MietegRevealDistance = null!;

    private void Awake()
    {
        TreasureWeight = Config.Bind("Treasures", "Weight", 80f, "Weight of each quest treasure.");
        MaxCarriedTreasures = Config.Bind("Treasures", "MaxCarried", 2, "Maximum number of active quest treasures.");
        MidkaHealingMeadPrice = Config.Bind("Midka", "MinorHealingMeadPrice", 50, "Coins required for one Minor Healing Mead.");
        ProgressionLock = Config.Bind("Progression", "Enabled", true, "Prevent rewards from biomes not unlocked by boss progression.");
        TraderCooldownWorldDays = Config.Bind("Quests", "TraderCooldownWorldDays", 3, "World days before the same trader can issue another shipment to a player.");\n        ReputationBonusPerDeliveryPercent = Config.Bind("Rewards", "ReputationBonusPerDeliveryPercent", 20, "Additive reward bonus gained for each completed delivery.");
        MietegActiveWorldDays = Config.Bind("LegendaryMieteg", "ActiveWorldDays", 2, "How many world days Mieteg remains active.");
        MietegRevealDistance = Config.Bind("LegendaryMieteg", "RevealDistance", 600f, "Distance in metres at which Mieteg becomes discoverable.");

        CommandManager.Instance.AddConsoleCommand(new ImmersiveTraderCommand());

        PrefabManager.OnVanillaPrefabsAvailable += OnVanillaPrefabsAvailable;
        ZoneManager.OnVanillaLocationsAvailable += OnVanillaLocationsAvailable;
        Logger.LogInfo($"{ModName} {ModVersion} loaded.");
    }

    private void OnDestroy()
    {
        PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;
        ZoneManager.OnVanillaLocationsAvailable -= OnVanillaLocationsAvailable;
    }

    private void OnVanillaPrefabsAvailable()
    {
        TreasureRegistry.Register();
        TraderRegistry.Initialize();
        NpcPrefabRegistry.Register();
        LegendaryMietegRegistry.RegisterPrefab();
        PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;
    }

    private void OnVanillaLocationsAvailable()
    {
        TraderLocationRegistry.Register();
        LegendaryMietegRegistry.RegisterLocations();
        ZoneManager.OnVanillaLocationsAvailable -= OnVanillaLocationsAvailable;
    }
}