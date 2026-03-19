using HarmonyLib;

namespace BCMHQModule.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class EndOfGamePatcherBackup
    {
        [HarmonyPatch("EndOfGame")]
        [HarmonyPrefix]
        static void EndOfGamePrefix(StartOfRound __instance)
        {
            if (!__instance.currentLevel.planetHasTime && TimeOfDay.Instance.daysUntilDeadline >= 1)
            {
                __instance.gameStats.daysSpent--;
            }
        }
    }
}
