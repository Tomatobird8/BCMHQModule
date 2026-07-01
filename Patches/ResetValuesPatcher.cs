using BrutalCompanyMinus.Minus;
using BrutalCompanyMinus.Minus.Handlers;
using HarmonyLib;

namespace BCMHQModule.Patches
{
    /// <summary>
    /// Fix for enemiesToSpawn lists not being cleared in ResetValues.
    /// </summary>
    [HarmonyPatch]
    public class ResetValuesPatcher
    {
        [HarmonyPatch(typeof(LevelModifications), nameof(LevelModifications.ResetValues))]
        [HarmonyPostfix]
        public static void ResetValues_Postfix()
        {
            Manager.enemiesToSpawnInside.Clear();
            Manager.enemiesToSpawnOutside.Clear();
        }
    }
}
