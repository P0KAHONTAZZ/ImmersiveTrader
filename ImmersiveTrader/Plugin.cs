using BepInEx;
using BepInEx.Configuration;
using Jotunn;
using Jotunn.Managers;

namespace ImmersiveTrader;

[BepInPlugin(ModGuid, ModName, ModVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string ModGuid = "p0kahontazz.immersivetrader";
    public const string ModName = "ImmersiveTrader";
    public const string ModVersion = "0.2.0";

    internal static ConfigEntry<float> TreasureWeight = null!;
    internal static ConfigEntry<int> MaxCarriedTreasures = null!;
    internal static ConfigEntry<int> MidkaHealingMeadPrice = null!;

    private void Awake()
    {
        TreasureWeight = Config.Bind("Treasures", "Weight", 80f, "Weight of each quest treasure.");
        MaxCarriedTreasures = Config.Bind("Treasures", "MaxCarried", 2, "Maximum number of active quest treasures.");
        MidkaHealingMeadPrice = Config.Bind("Midka", "MinorHealingMeadPrice", 50, "Coins required for one Minor Healing Mead.");

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
        PrefabManager.OnVanillaPrefabsAvailable -= OnVanillaPrefabsAvailable;
    }

    private void OnVanillaLocationsAvailable()
    {
        TraderLocationRegistry.Register();
        ZoneManager.OnVanillaLocationsAvailable -= OnVanillaLocationsAvailable;
    }
}