using BrutalCompanyMinus.Minus;
using HarmonyLib;

namespace BCMHQModule.Patches
{
    /// <summary>
    /// Patch LateShip and EarlyShip from affecting time on the next day if level never started.
    /// </summary>
    [HarmonyPatch]
    public class ShipLeaveTimeEffectsPatcher
    {
        [HarmonyPatch(typeof(QuickMenuManager), "LeaveGameConfirm")]
        [HarmonyPrefix]
        public static void OnQuit()
        {
            if (GameNetworkManager.Instance != null && !HUDManager.Instance.retrievingSteamLeaderboard && BrutalCompanyMinus.Net.Instance.receivedSyncedValues) ExecuteOnShipLeave();
        }
        [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
        [HarmonyPrefix]
        public static void OnShipLeave()
        {
            ExecuteOnShipLeave();
        }
        public static void ExecuteOnShipLeave()
        {
            Manager.moveTime = false;
            Manager.timeSpeedMultiplier = 1.0f;
            Manager.moveTimeAmount = 0.0f;
        }
    }
}
