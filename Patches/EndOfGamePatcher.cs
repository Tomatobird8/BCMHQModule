using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BCMHQModule.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class EndOfGamePatcher
    {
        [HarmonyPatch("EndOfGame", MethodType.Enumerator)]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> EndOfGameTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            FieldInfo? playersDeadFieldInfo = null;

            foreach (CodeInstruction code in instructions)
            {
                if ((code.operand as FieldInfo)?.Name?.Contains("<playersDead>") == true && (code.operand as FieldInfo)?.ReflectedType?.Name.Contains("<EndOfGame>") == true)
                {
                    playersDeadFieldInfo = code.operand as FieldInfo;
                    break;
                }
            }

            if (playersDeadFieldInfo == null)
            {
                BCMHQModule.Logger.LogWarning("Could not find playersDead in EndOfGame.");
                BCMHQModule.EndOfGameBackupPatch();
                return instructions;
            }

            IEnumerable<CodeInstruction> newInstructions = instructions;

            FieldInfo gameStatsField = AccessTools.Field(typeof(StartOfRound), "gameStats");
            FieldInfo daysSpentField = AccessTools.Field(typeof(EndOfGameStats), "daysSpent");

            FieldInfo currentLevelField = AccessTools.Field(typeof(StartOfRound), "currentLevel");
            FieldInfo planetHasTimeField = AccessTools.Field(typeof(SelectableLevel), "planetHasTime");

            MethodInfo TimeOfDayInstance = AccessTools.PropertyGetter(typeof(TimeOfDay), "Instance");
            FieldInfo daysUntilDeadlineField = AccessTools.Field(typeof(TimeOfDay), "daysUntilDeadline");

            try
            {
                newInstructions = new CodeMatcher(instructions, generator)
                    .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, gameStatsField),
                    new CodeMatch(OpCodes.Dup),
                    new CodeMatch(OpCodes.Ldfld, daysSpentField),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Add),
                    new CodeMatch(OpCodes.Stfld, daysSpentField))
                    .ThrowIfNotMatch("Couldnt match codes.")
                    .Advance(1)
                    .CreateLabel(out Label End)
                    .MatchBack(false,
                    new CodeMatch(OpCodes.Ldfld, gameStatsField),
                    new CodeMatch(OpCodes.Dup),
                    new CodeMatch(OpCodes.Ldfld, daysSpentField),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Add),
                    new CodeMatch(OpCodes.Stfld, daysSpentField))
                    .ThrowIfNotMatch("Failed to MatchBack.")
                    .Advance(-1)
                    .CreateLabel(out Label Start)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, currentLevelField))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, planetHasTimeField))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Brtrue_S, Start))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, TimeOfDayInstance))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, daysUntilDeadlineField))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Bge_S, End))
                    .InstructionEnumeration();
            }
            catch (Exception ex)
            {
                BCMHQModule.Logger.LogWarning($"Could not set new codeinstructions to EndOfGame. Exception {ex}");
                BCMHQModule.EndOfGameBackupPatch();
            }

            return newInstructions;
        }
    }
}
