using BrutalCompanyMinus;
using BrutalCompanyMinus.Minus.Events;
using HarmonyLib;
using System;
using System.Reflection;

namespace BCMHQModule.Patches
{
    [HarmonyPatch]
    public class NoMasksPatcher
    {
        [HarmonyPatch("BrutalCompanyMinus.Minus.Events.NoMasks, BrutalCompanyMinus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Execute")]
        [HarmonyPrefix]
        public static void ExecutePatcher()
        {
            BCMHQModule.Logger.LogDebug("Patching NoMasks event to keep a reference to the Comedy and Tragedy mask objects.");
            Type type = typeof(NoMasks);
            int num = RoundManager.Instance.currentLevel.spawnableScrap.FindIndex((SpawnableItemWithRarity s) => s.spawnableItem.name == Assets.ItemNameList[Assets.ItemName.Comedy]);
            if (num != -1)
            {
                FieldInfo comedyMaskField = type.GetField("comedyReference");
                comedyMaskField.SetValue(null, RoundManager.Instance.currentLevel.spawnableScrap[num]);
            }
            num = RoundManager.Instance.currentLevel.spawnableScrap.FindIndex((SpawnableItemWithRarity s) => s.spawnableItem.name == Assets.ItemNameList[Assets.ItemName.Tragedy]);
            if (num != -1)
            {
                FieldInfo tragedyMaskField = type.GetField("tragedyRefernece"); // typo in original
                tragedyMaskField.SetValue(null, RoundManager.Instance.currentLevel.spawnableScrap[num]);
            }
        }
    }
}
