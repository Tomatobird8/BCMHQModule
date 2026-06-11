using BrutalCompanyMinus;
using BrutalCompanyMinus.Minus;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BCMHQModule.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    public class UpdateEnemyHPPatcher
    {
        [HarmonyPatch(nameof(EnemyAI.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(EnemyAI __instance)
        {
            __instance.StartCoroutine(UpdateHP(__instance));
        }

        private static IEnumerator UpdateHP(EnemyAI ai)
        {
            yield return new WaitUntil(() => Net.Instance.receivedSyncedValues);
            ai.enemyHP = (int)Mathf.Clamp(ai.enemyHP + Manager.bonusEnemyHp, 1.1f, 99999999.0f);
        }
    }
}
