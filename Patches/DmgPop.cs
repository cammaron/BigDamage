using UnityEngine;
using BepInEx;
using HarmonyLib;

namespace BigDamage.Patches
{
	[HarmonyPatch(typeof(DmgPop), nameof(DmgPop.LoadInfo), new[] { typeof(int), typeof(bool), typeof(GameData.DamageType), typeof(Transform) })]
    public class DmgPop_LoadInfo
    {
		static void Postfix(DmgPop __instance, int _dmg, bool _crit, GameData.DamageType _type, Transform _tar)
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

			Traverse.Create(__instance).Field("sideDir").SetValue(new Vector3(Random.Range(-1f, 1f), 0, 0));

			if (Main.enableColourChange.Value)
            {
				__instance.Num.enableVertexGradient = false;
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
