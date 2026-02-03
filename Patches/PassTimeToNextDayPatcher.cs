using HarmonyLib;
using UnityEngine;

namespace BCMHQModule.Patches
{
    [HarmonyPatch]
    public class PassTimeToNextDayPatcher
    {
        [HarmonyPatch(typeof(StartOfRound), "PassTimeToNextDay")]
        [HarmonyPrefix]
        public static void PassTimeToNextDayPatch()
        {
            if (!TimeOfDay.Instance.currentLevel.planetHasTime && (TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled) > 0)
            {
                if (TimeOfDay.Instance.daysUntilDeadline > 0)
                {
                    if (TimeOfDay.Instance.daysUntilDeadline == 1)
                    {
                        TimeOfDay.Instance.globalTimeAtEndOfDay = 0;
                    }
                    TimeOfDay.Instance.timeUntilDeadline -= 1050f;
                    TimeOfDay.Instance.OnDayChanged();
                    HUDManager.Instance.DisplayDaysLeft((int)Mathf.Floor(TimeOfDay.Instance.timeUntilDeadline / TimeOfDay.Instance.totalTime));
                }
                else
                {
                    TimeOfDay.Instance.timeUntilDeadline -= 1050f;
                }
            }
        }
    }
}

