using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;

namespace BigDamage.Patches
{
	[HarmonyPatch(typeof(DmgPop), "Update")]
	public class DmgPop_Update {

		static readonly Dictionary<DmgPop, float> beginShrinkTimeByDmgPop = new Dictionary<DmgPop, float>();

		static bool Prefix(DmgPop __instance)
        {
			__instance.speed -= Time.deltaTime * Main.speedReduction.Value; // Starting with a higher initial speed, but decreasing speed faster, too
			if (__instance.speed <= 0f)
            {
				__instance.speed = 0f;
				if (!beginShrinkTimeByDmgPop.TryGetValue(__instance, out float beginShrinkTime))
                {
					beginShrinkTime = Time.realtimeSinceStartup + Main.delayBeforeShrinkingAway.Value;
					beginShrinkTimeByDmgPop[__instance] = beginShrinkTime;
				}
				
				if (Time.realtimeSinceStartup < beginShrinkTime) // Don't let the original code run until we're ready to shrink and disappear
					return false;
            }

			return true;
		}
		static void Postfix(DmgPop __instance)
        {
			if (!__instance.gameObject.activeSelf)
				beginShrinkTimeByDmgPop.Remove(__instance);
		}
	}

	[HarmonyPatch(typeof(DmgPop), nameof(DmgPop.LoadInfo), new[] { typeof(int), typeof(bool), typeof(GameData.DamageType), typeof(Transform) })]
    public class DmgPop_LoadInfo
    {
		const float SIDE_FACTOR_MAX = .55f;
		static float sideFactor = -SIDE_FACTOR_MAX;

		static void Postfix(DmgPop __instance, int _dmg, bool _crit, GameData.DamageType _type, Transform _tar, ref Vector3 ___moveDir, ref Vector3 ___sideDir, ref float ___fontDel, ref float ___destSize, ref float ___dest)
        {
			if (__instance.GetComponent<ScaleWithDistance>() == null)
            {
				var scale = __instance.gameObject.AddComponent<ScaleWithDistance>();
				scale.maxScale = 5f;
				scale.extraBaseScale = Main.baseSizeFactor.Value;
			}

			var npc = _tar.GetComponent<NPC>();
			if (npc?.NamePlate != null)
				__instance.transform.position = npc.NamePlate.position;

			___sideDir = Vector3.zero;

			// Vanilla attempts to make DmgPops float to the left or right (so as not to consistently overlap) based on a random factor applied to X direction.
			// The problem is that since they spawn in world space, "X" is a completely arbitrary direction that doesn't necessarly correlate to the left or right of the screen view,
			// which also means that depending on the relative position of the enemy you're fighting, those numbers may go "back" or forward rather than left or right.
			// Addressing this by making their lateral movement relative to camera orientation, and also rather than a random amount of lateral movement,
			// specifically fanning them out in a consistent arc shape so two don't overlap by chance
			sideFactor += .2f;
			___moveDir = (-GameData.CamControl.ActualCam.transform.right) * sideFactor + Vector3.up * .6f;
			if (sideFactor >= SIDE_FACTOR_MAX * 1.1f) sideFactor = -SIDE_FACTOR_MAX;

			__instance.speed = Main.startSpeed.Value;
			__instance.Num.fontSize = 2.8f;
			___fontDel = 0f;
			___destSize = __instance.Num.fontSize * Main.juicyShrinkFactor.Value;
			___dest = 1.2f * 60f;

			if (Main.enableColourChange.Value)
            {
				__instance.Num.enableVertexGradient = false;
				if (_crit) 
					__instance.Num.color = Main.critColour;
				else
                {
					switch (_type)
					{
						case GameData.DamageType.Physical:
							__instance.Num.color = Main.physColour;
							break;
						case GameData.DamageType.Magic:
							__instance.Num.color = Main.magColour;
							return;
						case GameData.DamageType.Elemental:
							__instance.Num.color = Main.eleColour;
							return;
						case GameData.DamageType.Void:
							__instance.Num.color = Main.voidColour;
							return;
						case GameData.DamageType.Poison:
							__instance.Num.color = Main.poisonColour;
							return;
						default:
							return;
					}
				}
			}
		}

    }
}
