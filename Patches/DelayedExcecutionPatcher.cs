using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace BCMHQModule.Patches
{
    [HarmonyPatch(typeof(BrutalCompanyMinus.Minus.Manager), nameof(BrutalCompanyMinus.Minus.Manager.DelayedExecution), MethodType.Enumerator)]
    public class DelayedExcecutionPatcher
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> DelayedExcecutionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Newobj, AccessTools.Constructor(typeof(WaitForSeconds), new[] { typeof(float) }))
                )
                .ThrowIfNotMatch("Could not match for WaitForSeconds(float) in DelayedExcecution")
                .MatchForward(false,
                    new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(BrutalCompanyMinus.Net), nameof(BrutalCompanyMinus.Net.Instance)))
                )
                .ThrowIfNotMatch("Could not match for BrutalCompanyMinus.Net.Instance in DelayedExcecution")
                .Insert(
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(StartOfRound), nameof(StartOfRound.Instance))),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StartOfRound), nameof(StartOfRound.randomMapSeed))),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(BrutalCompanyMinus.Net), nameof(BrutalCompanyMinus.Net.Instance))),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BrutalCompanyMinus.Net), "_seed")),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(BrutalCompanyMinus.Minus.Manager.Spawn), nameof(BrutalCompanyMinus.Minus.Manager.Spawn.randomSeedValue)))
                )
                .InstructionEnumeration();
        }
    }
}
