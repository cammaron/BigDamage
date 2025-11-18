using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BigDamage.Patches
{
	/* Character.MagicDamageMe doesn't take isCrit as a param in order to pass that info to Stats.ReduceHp and thus to the DmgPop.
	 * Work around this for now by setting a static field whenever a crit is occurring in ResolveSpell, which will be checked
	 in a separate patch for ReduceHp, and always set back to false in the ResolveSpell postfix so it doesn't interfere with 
	any other damage calls.*/
	[HarmonyPatch(typeof(SpellVessel), "ResolveSpell")]
	public class SpellVessel_ResolveSpell
	{
		public static bool isMagicCritReady = false;

		/*
		 By default, GenPopup will only show a DmgPop if it's less than 20 units from the player. This patch increases that range to 55.
		 */
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return new CodeMatcher(instructions)
				.MatchStartForward(new CodeInstruction(OpCodes.Ldstr, "8312964")) // checking if caster has the skill allowing spells to crit
				.MatchStartForward(new CodeInstruction(OpCodes.Ldc_I4_1))
				.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1))
				.Insert(CodeInstruction.StoreField(typeof(SpellVessel_ResolveSpell), nameof(isMagicCritReady)))
				.Instructions();
		}

		static void Postfix()
        {
			isMagicCritReady = false;
        }
	}
}
