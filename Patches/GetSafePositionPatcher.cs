using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace BCMHQModule.Patches
{
    [HarmonyPatch("BrutalCompanyMinus.Minus.Functions, BrutalCompanyMinus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "GetSafePosition")]
    public class GetSafePositionPatcher
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> GetSafePositionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            BCMHQModule.Logger.LogDebug("Transpiler called for GetSafePosition");

            var codes = new List<CodeInstruction>(instructions);

            // Vector3 randomPositionInRadius = RoundManager.Instance.GetRandomPositionInRadius(vector, 0f, radius, random);
            var originalMethod = AccessTools.Method(typeof(RoundManager), "GetRandomPositionInRadius", new[]
            {
                typeof(Vector3),
                typeof(float),
                typeof(float),
                typeof(System.Random)
            });

            var replacementMethod = AccessTools.Method(typeof(GetSafePositionPatcher), nameof(GetSafePosition_Replacement));

            if (originalMethod == null || replacementMethod == null)
            {
                BCMHQModule.Logger.LogDebug("OriginalMethod or ReplacementMethod were null.");
                return codes;
            }

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Calls(originalMethod))
                {
                    BCMHQModule.Logger.LogDebug($"Replacing call at index {i} with {nameof(RoundManager.GetRandomNavMeshPositionInRadius)}.");
                    codes[i] = new CodeInstruction(OpCodes.Call, replacementMethod);
                }
            }
            return codes;
        }
        public static Vector3 GetSafePosition_Replacement(Vector3 vector, float minRadius, float radius, System.Random random)
        {
            return RoundManager.Instance.GetRandomNavMeshPositionInRadius(vector, radius);
        }
    }
}
