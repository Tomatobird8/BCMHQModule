using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BCMHQModule.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    public class QuotaRackupPatcher
    {
        [HarmonyPatch("rackUpNewQuotaText", MethodType.Enumerator)]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> rackUpNewQuotaTextMoveNext(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo? quotaTextAmountFieldInfo = null;

            foreach (CodeInstruction code in instructions)
            {
                if ((code.operand as FieldInfo)?.Name?.Contains("<quotaTextAmount>") == true && (code.operand as FieldInfo)?.ReflectedType?.Name.Contains("<rackUpNewQuotaText>") == true)
                {
                    quotaTextAmountFieldInfo = code.operand as FieldInfo;
                    break;
                }
            }

            if (quotaTextAmountFieldInfo == null)
            {
                BCMHQModule.Logger.LogWarning("Could not find quotaTextAmount in rackUpNewQuotaText enumerator. Quota rackup will not be sped up.");
                return instructions;
            }

            IEnumerable<CodeInstruction> newInstructions = instructions;

            try
            {
                newInstructions = new CodeMatcher(instructions)
                    .MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 250f), new CodeMatch(OpCodes.Mul))
                    .ThrowIfNotMatch("250f quota increment amount not found")
                    .SetOperandAndAdvance(1500f)
                    .InstructionEnumeration();
            }
            catch (Exception ex)
            {
                BCMHQModule.Logger.LogWarning($"Could not set new codeinstructions to rackUpNewQuotaText. Exception: {ex}");
            }

            return newInstructions;
        }
    }
}
