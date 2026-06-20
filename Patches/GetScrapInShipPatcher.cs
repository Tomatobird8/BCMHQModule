using HarmonyLib;
using BrutalCompanyMinus.Minus;

namespace BCMHQModule.Patches
{
    /// <summary>
    /// Replace GetScrapInShip method with GetValueOfAllScrap to also get scrap in cruiser.
    /// </summary>
    [HarmonyPatch(typeof(Manager))]
    public class GetScrapInShipPatcher
    {
        [HarmonyPatch("GetScrapInShip")]
        [HarmonyPrefix]
        public static bool GetScrapInShip_Prefix(ref float __result)
        {
            __result = StartOfRound.Instance.GetValueOfAllScrap(onlyScrapCollected: true, onlyNewScrap: false);
            return false;
        }
    }
}
