using BCMHQModule.Patches;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

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

            if (isHQoLLoaded)
            {
                foreach (PluginInfo p in Chainloader.PluginInfos.Values)
                {
                    if (p.Metadata.GUID == "Drinkable.BrutalCompanyMinus" && p.Metadata.Version.ToString() != "0.10.12")
                    {
                        Logger.LogDebug($"Patching {nameof(ValueSynchronizer)}");
                        Harmony.PatchAll(typeof(ValueSynchronizer));
                        break;
                    }
                }
            }
            else
            {
                Logger.LogDebug("HQoL was not loaded. Skip patchning...");
            }
            Logger.LogDebug($"Patching {nameof(ShipLeaveOnQuit)}");
            Harmony.PatchAll(typeof(ShipLeaveOnQuit));

            Logger.LogDebug($"Patching {nameof(PassTimeToNextDayPatcher)}");
            Harmony.PatchAll(typeof(PassTimeToNextDayPatcher));

            Logger.LogDebug("Finished patching!");

        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
