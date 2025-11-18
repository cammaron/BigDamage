using UnityEngine;
using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BigDamage.Patches
{
	[HarmonyPatch(typeof(Stats), "TickEffects")]
	public class Stats_TickEffects
	{


		/*
		This patch is to fix an issue with Stats.TickEffects not passing in the attacker parameter to DamageMe and BleedDamageMe calls.
		Because damage number popups only spawn for the player/party members, without knowing who to credit the damage ticks to,
		damage popups for DoTs never showed.
		 */
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);
			MethodInfo damageMe = AccessTools.Method(typeof(Character), nameof(Character.DamageMe));
			FieldInfo getCreditDPS = AccessTools.Field(typeof(StatusEffect), nameof(StatusEffect.CreditDPS));
			MethodInfo getStatusEffectsArray = AccessTools.PropertyGetter(typeof(Stats), "StatusEffects");

			matcher.MatchStartForward(new CodeMatch(OpCodes.Callvirt, damageMe)).ThrowIfNotMatch("Unable to find DamageMe call in TickEffects.") // NOTE: As of 10/11/2025, no longer sure this is needed. Source code appears to pass relevant params in. But BleedDamageMe still passes null, so below still needed.
				.MatchBack(false, new CodeMatch(OpCodes.Ldnull)).ThrowIfNotMatch("Unable to find ldnull argument insertion prior to DamageMe.")
				.RemoveInstruction()
				.Insert(
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Call, getStatusEffectsArray),
					new CodeInstruction(OpCodes.Ldloc_0),
					new CodeInstruction(OpCodes.Ldelem_Ref),
					new CodeInstruction(OpCodes.Ldfld, getCreditDPS)
			);

			MethodInfo bleedDamageMe = AccessTools.Method(typeof(Character), nameof(Character.BleedDamageMe));
			matcher.MatchStartForward(new CodeMatch(OpCodes.Callvirt, bleedDamageMe)).ThrowIfNotMatch("Unable to find BleedDamageMe call in TickEffects.")
				.MatchBack(false, new CodeMatch(OpCodes.Ldnull)).ThrowIfNotMatch("Unable to find ldnull argument insertion prior to BleedDamageMe.")
				.RemoveInstruction()
				.Insert(
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Call, getStatusEffectsArray),
					new CodeInstruction(OpCodes.Ldloc_0),
					new CodeInstruction(OpCodes.Ldelem_Ref),
					new CodeInstruction(OpCodes.Ldfld, getCreditDPS)
			);

			return matcher.Instructions();
		}

	}

	/* Per SpellVessel_ResolveSpell patch, override isCritical argument if the static is set */
	[HarmonyPatch(typeof(Stats), nameof(Stats.ReduceHP))]
	public class Stats_ReduceHP
	{
		public static void Prefix(Stats __instance, ref bool _isCritical)
		{
			if (SpellVessel_ResolveSpell.isMagicCritReady)
				_isCritical = true;
		}
	}
}
