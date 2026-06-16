using BrutalCompanyMinus.Minus.Handlers;
using HarmonyLib;

namespace BCMHQModule.Patches
{
    /// <summary>
    /// Patch for reverting level modifications when lobby was quit by using the quit button in the quick menu.
    /// </summary>
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
