using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using HarmonyLib.Tools;
using System.Collections.Generic;

namespace BigDamage
{
	[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
	class Main : BaseUnityPlugin
	{
		public const string PLUGIN_GUID = "com.raventhe.erenshor.BigDamage";
		public const string PLUGIN_NAME = "BigDamage";
		public const string PLUGIN_VERSION = "1.0.4";

		internal static ConfigEntry<float> maxDistance;
		internal static ConfigEntry<float> baseSizeFactor;
		internal static ConfigEntry<float> distanceToDoubleSize;
		internal static ConfigEntry<bool> enableColourChange;

		internal static ConfigEntry<float> startSpeed;
		internal static ConfigEntry<float> speedReduction;
		internal static ConfigEntry<float> juicyShrinkFactor;
		internal static ConfigEntry<float> delayBeforeShrinkingAway;

		internal static Color physColour;
		internal static Color magColour;
		internal static Color eleColour;
		internal static Color voidColour;
		internal static Color poisonColour;
		internal static Color critColour;

		internal static ManualLogSource Log;

		public void Awake()
		{
			Log = base.Logger;
			string general = "0_General";
			string colours = "1_Damage_Colours";
			string dynamics = "2_Dynamics";

			Logger.LogInfo("Initialising.");
			maxDistance = Config.Bind(general, "maxDistance", 55f, $"The distance a damage popup can be at before being hidden from the player. Higher = view hits further away. Vanilla default is 20.");
			baseSizeFactor = Config.Bind(general, "baseSizeFactor", 2.5f, $"An initial multiplier to damage popup font size. Just makin' it bigger regardless.");
			distanceToDoubleSize = Config.Bind(general, "distanceToDoubleSize", 17f, $"Every X units of distance, the damage popup text scale is doubled. Scaling over distance is the main purpose of this mod, as damage occurring further away needs to be larger in world space to be visible.");
			enableColourChange = Config.Bind(general, "enableColourChange", true, $"{PLUGIN_NAME} attempts to increase damage readability by removing the colour gradient and choosing simple, bold colours. Set this to false if you prefer the original style.");

			ColorUtility.TryParseHtmlString(Config.Bind(colours, "physical", "#8c8c8c", $"Text colour for PHYSICAL damage. Default is medium grey.").Value, out physColour);
			ColorUtility.TryParseHtmlString(Config.Bind(colours, "magical", "#b500a6", $"Text colour for MAGIC damage. Default is light magenta.").Value, out magColour);
			ColorUtility.TryParseHtmlString(Config.Bind(colours, "elemental", "#54ffeb", $"Text colour for ELEMENTAL damage. Default is sky blue.").Value, out eleColour);
			ColorUtility.TryParseHtmlString(Config.Bind(colours, "void", "#35026b", $"Text colour for VOID damage. Default is deep purple.").Value, out voidColour);
			ColorUtility.TryParseHtmlString(Config.Bind(colours, "poison", "#8cff50", $"Text colour for POISON damage. Default is acid green.").Value, out poisonColour);
			ColorUtility.TryParseHtmlString(Config.Bind(colours, "crit", "#ffe300", $"Text colour for critical hits (regardless of damage type). I'm gonna call the default 'banana yellow.'").Value, out critColour);

			startSpeed = Config.Bind(dynamics, "startSpeed", 16f, $"Initial fly-out speed of DmgPop. BigDamage stylistically sets a higher base speed with a sharper speed drop-off (via speedReduction setting). For vanilla, set this to 3.");
			speedReduction = Config.Bind(dynamics, "speedReduction", 18f, $"DmgPop speed reduces by this extra amount per second (on top of vanilla). For vanilla, set this to 0.");
			juicyShrinkFactor = Config.Bind(dynamics, "juicyShrinkFactor", .5f, $"DmgPops spawn at full size and will immediately begin progressively shrinking to this size factor. Seems to be for juiciness. Vanilla is different across different damage types. Set to 1 if you don't want any juicy shrink.");
			delayBeforeShrinkingAway = Config.Bind(dynamics, "delayBeforeShrinkingAway", .5f, $"An extra period of gentle stillness (after fly-out finishes) before we begin fading away, to ensure you get to grok those numbers with those big meaty eyeballs of yours.");

#if DEBUG
			HarmonyFileLog.FileWriterPath = "D:\\Games\\HarmonyLog.txt";
			HarmonyFileLog.Enabled = true;
#else
            HarmonyFileLog.Enabled = false;
#endif

			var harmony = new Harmony(PLUGIN_GUID);
			harmony.PatchAll();
		}

		protected void OnDestroy()
		{
			foreach (var obj in FindObjectsOfType<ScaleWithDistance>())
				Destroy(obj);
		}
	}
}
