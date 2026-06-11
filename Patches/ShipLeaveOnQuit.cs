using BrutalCompanyMinus.Minus.Handlers;
using HarmonyLib;

namespace BCMHQModule.Patches
{
    [HarmonyPatch]
    public class ShipLeaveOnQuit
    {
        [HarmonyPatch(typeof(QuickMenuManager), "LeaveGameConfirm")]
        [HarmonyPrefix]
        private static void ShipLeave()
        {
            if (GameNetworkManager.Instance != null && !HUDManager.Instance.retrievingSteamLeaderboard && BrutalCompanyMinus.Net.Instance.receivedSyncedValues)
            {
                BCMHQModule.Logger.LogInfo("ShipLeave called from LeaveGameConfirm to remove extra enemy spawn attributes.");
                LevelModifications.OnShipLeave();
            }
        }
    }
}
