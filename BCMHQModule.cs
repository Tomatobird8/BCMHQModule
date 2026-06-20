using BCMHQModule.Patches;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace BCMHQModule
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("Drinkable.BrutalCompanyMinus", "0.10.12")]
    [BepInDependency("OreoM.HQoL.73", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("OreoM.HQoL.72", BepInDependency.DependencyFlags.SoftDependency)]
    public class BCMHQModule : BaseUnityPlugin
    {
        public static BCMHQModule Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        public static ConfigEntry<bool> internalNames = null!, sdcMode = null!, debugMode = null!;

        internal static bool correctVersion = false;
        internal static string bcmVersionString = "";

        internal static List<VersionDefinition> versionDefinitions =
        [
            new VersionDefinition(Versions.v49, "0.10.12", [typeof(NoMasksPatcher), typeof(GetSafePositionPatcher), typeof(DelayedExecutionPatcher_v49), typeof(ShipLeaveOnQuit)]),
            new VersionDefinition(Versions.v50, "0.13.9", [typeof(DelayedExecutionPatcher), typeof(ShipLeaveOnQuit), typeof(DoSpawnOutsideEnemiesPatcher)]),
            new VersionDefinition(Versions.v56, "0.13.10", [typeof(DelayedExecutionPatcher), typeof(ShipLeaveOnQuit), typeof(GetScrapInShipPatcher), typeof(DoSpawnOutsideEnemiesPatcher)]),
            new VersionDefinition(Versions.v72, "0.13.13", [typeof(DelayedExecutionPatcher), typeof(ShipLeaveOnQuit), typeof(UpdateEnemyHPPatcher), typeof(GetScrapInShipPatcher), typeof(DoSpawnOutsideEnemiesPatcher)]),
            new VersionDefinition(Versions.v73, "0.13.14", [typeof(DelayedExecutionPatcher), typeof(ShipLeaveOnQuit), typeof(UpdateEnemyHPPatcher), typeof(GetScrapInShipPatcher), typeof(DoSpawnOutsideEnemiesPatcher)]),
            new VersionDefinition(Versions.v81, "0.13.17", [])
        ];

        internal static bool isHQoLLoaded;

        private void Awake()
        {
            Logger = base.Logger;

            if (Instance == null) Instance = this;

            internalNames = Config.Bind("General", "InternalEventNames", true, "Display the internal event names instead of the randomly selected event descriptions.");
            sdcMode = Config.Bind("General", "SdcMode", false, "Set difficulty to 100 for Single Day Clear runs.");
            debugMode = Config.Bind("General", "DebugMode", false, "Enable debug mode?");

            isHQoLLoaded = Chainloader.PluginInfos.ContainsKey("OreoM.HQoL.72") || Chainloader.PluginInfos.ContainsKey("OreoM.HQoL.73");
            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            foreach (PluginInfo p in Chainloader.PluginInfos.Values)
            {
                if (p.Metadata.GUID != "Drinkable.BrutalCompanyMinus")
                {
                    continue;
                }
                Version bcmVersion = p.Metadata.Version;
                bcmVersionString = bcmVersion.ToString();
                foreach (VersionDefinition v in versionDefinitions)
                {
                    if (bcmVersionString != v.bcmVersion)
                    {
                        continue;
                    }
                    PatchType(v.types);
                    if (v.version != Versions.v49)
                    {
                        PatchType(typeof(LockConfigurationPatcher)); 
                        if (isHQoLLoaded) PatchType(typeof(ValueSynchronizer)); // HQoL Support patch
                    }
                    correctVersion = true;
                    break;
                }
                if (!correctVersion)
                {
                    Logger.LogWarning($"Version {bcmVersion} of BCM loaded was NOT contained in the internal version list. Make sure you're using one of the versions listed below!");
                    foreach (VersionDefinition v in versionDefinitions)
                    {
                        Logger.LogWarning($"{v.version}: {v.bcmVersion}");
                    }
                    Logger.LogWarning("Applying generic patches anyway assuming this is a newer version. Things may break.");
                }
                if (bcmVersionString != versionDefinitions[0].bcmVersion) PatchType(typeof(MenuManagerPatcher)); // Add settings for SDC and internal event name display 
                // General patches
                PatchType([
                    typeof(EndOfGamePatcher),
                    typeof(QuotaRackupPatcher)
                ]);
                break;
            }

            Logger.LogDebug("Finished patching!");
        }

        internal static void PatchType(Type[] typeArray)
        {
            foreach (Type type in typeArray)
            {
                PatchType(type);
            }
        }

        internal static void PatchType(Type type)
        {
            Logger.LogDebug($"Patching {type.Name}");
            Harmony?.PatchAll(type);
        }

        internal static void EndOfGameBackupPatch()
        {
            Logger.LogWarning($"Failed to patch {nameof(EndOfGamePatcher)}! Patching {nameof(EndOfGamePatcherBackup)} as backup!");
            PatchType([typeof(EndOfGamePatcherBackup)]);
        }

        internal enum Versions
        {
            v49,
            v50,
            v56,
            v72,
            v73,
            v81
        }
    }
}
