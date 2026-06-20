using BrutalCompanyMinus.Minus;
using HarmonyLib;
using BrutalCompanyMinus.Minus.Events;

namespace BCMHQModule.Patches
{
    /// <summary>
    /// Fix for SafeOutside event leaving enemiesToSspawnOutside list populated.
    /// </summary>
    [HarmonyPatch(typeof(Manager.Spawn))]
    public class DoSpawnOutsideEnemiesPatcher
    {
        [HarmonyPatch("DoSpawnOutsideEnemies")]
        [HarmonyPrefix]
        public static void DoSpawnOutsideEnemies_Prefix()
        {
            if (SafeOutside.Active)
            {
                Manager.enemiesToSpawnOutside.Clear();
            }
        }
    }
}
