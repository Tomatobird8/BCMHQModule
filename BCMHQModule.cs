using BCMHQModule.Patches;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
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

        internal static bool isHQoLLoaded;

        private void Awake()
        {
            Logger = base.Logger;

            if (Instance == null) Instance = this;

            isHQoLLoaded = Chainloader.PluginInfos.ContainsKey("OreoM.HQoL.72") || Chainloader.PluginInfos.ContainsKey("OreoM.HQoL.73");
            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            foreach (PluginInfo p in Chainloader.PluginInfos.Values)
            {
                if (p.Metadata.GUID == "Drinkable.BrutalCompanyMinus")
                {
                    string versionString = p.Metadata.Version.ToString();
                    if (!VersionList.ContainsValue(versionString))
                    {
                        Logger.LogWarning($"Version {versionString} of BCM loaded was NOT contained in the internal version list. Make sure you're using one of the versions listed below.");
                        foreach (KeyValuePair<Versions, string> k in VersionList)
                        {
                            Logger.LogWarning($"{k.Value}");
                        }
                        Logger.LogWarning("Applying v50+ patches anyway assuming this is a newer version. Things may break.");
                    }
                    if (versionString == VersionList[Versions.v49])
                    {
                        Logger.LogDebug($"BCM Version {VersionList[Versions.v49]} is loaded");
                        Logger.LogDebug($"Patching {nameof(NoMasksPatcher)}");
                        Harmony.PatchAll(typeof(NoMasksPatcher));
                        Logger.LogDebug($"Patching {nameof(GetSafePositionPatcher)}");
                        Harmony.PatchAll(typeof(GetSafePositionPatcher));
                    }
                    else
                    {
                        Logger.LogDebug($"BCM Version {versionString} is loaded");
                        if (isHQoLLoaded)
                        {
                            Logger.LogDebug($"Patching {nameof(ValueSynchronizer)}");
                            Harmony.PatchAll(typeof(ValueSynchronizer));
                        }
                        else
                        {
                            Logger.LogWarning($"BCM 50+ loaded without HQoL. ValueSynchronizer won't be used.");
                        }
                    }
                    break;
                }
            }
            Logger.LogDebug($"Patching {nameof(ShipLeaveOnQuit)}");
            Harmony.PatchAll(typeof(ShipLeaveOnQuit));

            Logger.LogDebug($"Patching {nameof(PassTimeToNextDayPatcher)}");
            Harmony.PatchAll(typeof(PassTimeToNextDayPatcher));

            Logger.LogDebug($"Patching {nameof(QuotaRackupPatcher)}");
            Harmony.PatchAll(typeof(QuotaRackupPatcher));

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }

        internal enum Versions
        {
            v49,
            v50,
            v56,
            v72,
            v73
        }

        internal static Dictionary<Versions, string> VersionList = new Dictionary<Versions, string> 
        {
            {
                Versions.v49,
                "0.10.12"
            },
            {
                Versions.v50,
                "0.13.9"
            },
            {
                Versions.v56,
                "0.13.10"
            },
            {
                Versions.v72,
                "0.13.13"
            },
            {
                Versions.v73,
                "0.13.14"
            }
        };
    }
}
