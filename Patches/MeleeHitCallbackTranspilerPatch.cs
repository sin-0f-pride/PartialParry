using System.Collections.Generic;
using System.Reflection.Emit;

using TaleWorlds.MountAndBlade;

using HarmonyLib;

namespace PartialParry.Patches
{
    [HarmonyPatch]
    public static class MeleeHitCallbackTranspilerPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Mission), "MeleeHitCallback")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            for (int i = 1; i < code.Count - 1; i++)
            {
                // we search for the first local set local variable (which is a "flag" who he used to define if the attack as been blocked)
                if (code[i].opcode == OpCodes.Stloc_0)
                {
                    // we make sure that it will be set at false by changing the instruction where the code jump when true
                    // so that the registerBlow will always be called since the blow is not parried
                    if (code[i - 1].opcode == OpCodes.Ldc_I4_1)
                    {
                        code[i - 1].opcode = OpCodes.Ldc_I4_0;
                        return code;
                    }
                    break;
                }
            }
            return code;
        }
    }
}