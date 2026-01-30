extern alias HQoL72;
extern alias HQoL73;
using BepInEx.Bootstrap;
using BrutalCompanyMinus.Minus;
using HarmonyLib;
using HQoL72Network = HQoL72::HQoL.Network;
using HQoL73Network = HQoL73::HQoL.Network;

namespace BCMHQModule.Patches
{
    [HarmonyPatch]
    public class ValueSynchronizer
    {
        [HarmonyPatch(typeof(Manager), "GetScrapInShip")]
        [HarmonyPostfix]
        private static void AddStoredScrapValue(ref float __result){
            var hqol73 = HQoL73Network.HQoLNetwork.Instance;
            if (hqol73 != null)
            {
                __result = __result + HQoL73Network.HQoLNetwork.Instance.totalStorageValue.Value;
            }

            var hqol72 = HQoL72Network.HQoLNetwork.Instance;
            if (hqol72 != null)
            {
                __result = __result + HQoL72Network.HQoLNetwork.Instance.totalStorageValue.Value;
            }
        }
    }
}
