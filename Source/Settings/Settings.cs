using RimWorld;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimFridge
{
	public class SettingsController : Mod
	{
		public static Harmony harmony;
		public SettingsController (ModContentPack content) : base(content)
		{
			Settings.PowerFactor = new FloatInput("RimFridge.BasePowerFactor");
			Settings.forcedApplicationOfPatches = new();

			harmony = new Harmony("com.rimfridge.rimworld.mod");

			/* We're initialising these static fields here,
			   instead of simply initialising them in their declaration,
			   to avoid any runtime-level checks for ensuring that the .cctor was called. */
			FridgeCacheFast.compCache = new Dictionary<Map, Dictionary<IntVec3, CompRefrigerator>>();
			FridgeCacheFast.rimFridgeCache = new Dictionary<Map, Dictionary<IntVec3, RimFridge_Building>>();
			FridgeCacheFast.wallFridgeCache = new Dictionary<Map, Dictionary<IntVec3, RimFridge_WallBuilding>>();
			FridgeCacheFast.doubleSidedCache = new Dictionary<Map, Dictionary<IntVec3, RimFridge_DoubleSidedWallBuilding>>();

			Patch(typeof(EnsureThatItemsInAFridgeCanBeReachedByPawns.SetPathEndModeForReachabilityCanReachSuchThatItemsInFridgeMayBeReached));
			Patch(typeof(EnsureThatItemsInAFridgeCanBeReachedByPawns.SetPathEndModeForThingFromRegionListerReachableSuchThatItemsInWallFridgeMayBeReached));
			Patch(typeof(Patch_Thing_AmbientTemperature));
			Patch(typeof(Patch_PassingShip_TryOpenComms));
			Patch(typeof(EnsureThatPrisonersGetFoodFromFridgesInPrisons));
			Patch(typeof(DisplayStackedItemsNicelyInFridges.MungeTrueCenterOfItemsInFridges));
			Patch(typeof(DisplayStackedItemsNicelyInFridges.MakeTheStackCountLabelsReadable));
			Patch(typeof(HacksForCompatibility.ForceTheApplicationOfSomePatches));

			base.GetSettings<Settings>();
		}

		internal static void Patch (Type type)
		{
			try
			{
				harmony.CreateClassProcessor(type).Patch();
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString());
			}
		}

		internal static void Unpatch (Type type)
		{
			try
			{
				harmony.CreateClassProcessor(type).Unpatch();
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString());
			}
		}

		internal static Type PatchWithFallback (Type ideal, Type fallback)
		{
			try
			{
				try
				{
					harmony.CreateClassProcessor(ideal).Patch();
					return ideal;
				}
				catch (Exception inner)
				{
					if (inner is TranspilerFallbackException)
					{
						Logger.Warning(inner.ToString());
					}
					else
					{
						Logger.Error(inner.ToString());
					}

					Logger.Warning($"Failed to apply `{ideal.FullName}`, falling back to `{fallback.FullName}`.");

					harmony.CreateClassProcessor(fallback).Patch();
					return fallback;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString());
			}

			return null;
		}

		internal static Type PatchWithFallbacks (Type ideal, Type fallback0, Type fallback1)
		{
			try
			{
				try
				{
					harmony.CreateClassProcessor(ideal).Patch();
					return ideal;
				}
				catch (Exception inner0)
				{
					if (inner0 is TranspilerFallbackException)
					{
						Logger.Warning(inner0.ToString());
					}
					else
					{
						Logger.Error(inner0.ToString());
					}

					Logger.Warning($"Failed to apply `{ideal.FullName}`, falling back to `{fallback0.FullName}`.");

					try
					{
						harmony.CreateClassProcessor(fallback0).Patch();
						return fallback0;
					}
					catch (Exception inner1)
					{
						if (inner1 is TranspilerFallbackException)
						{
							Logger.Warning(inner1.ToString());
						}
						else
						{
							Logger.Error(inner1.ToString());
						}

						Logger.Warning($"Failed to apply `{fallback0.FullName}`, falling back to `{fallback1.FullName}`.");

						harmony.CreateClassProcessor(fallback1).Patch();
						return fallback1;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString());
			}

			return null;
		}

		public override string SettingsCategory ()
		{
			return "RimFridge";
		}

		public static int maximumDefaultMaximumItemsPerCell = 3;

		internal class GUIState
		{
			internal Vector2 forcedApplicationOfPatchesScrollPosition;
			internal List<bool> initialStateOfShouldForceApplicationOfPatches;
		}

		internal GUIState guiState = null;

		public override void DoSettingsWindowContents (Rect rect)
		{
			if (guiState == null)
			{
				guiState = new();
				guiState.initialStateOfShouldForceApplicationOfPatches = (
					Settings.forcedApplicationOfPatches.Select(a => a.shouldForceApplication).ToList()
				);
			}

			GUI.BeginGroup(new Rect(0, 60, 800, 600));
			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(0, 40, 300, 20), "RimFridge.ModifyBasePowerRequirement".Translate() + ":");
			Settings.PowerFactor.AsString = Widgets.TextField(new Rect(320, 40, 100, 20), Settings.PowerFactor.AsString);

			if (Widgets.ButtonText(new Rect(320, 65, 100, 20), "RimFridge.Apply".Translate()))
			{
				if (Settings.PowerFactor.ValidateInput())
				{
					GetSettings<Settings>().Write();
					Messages.Message("RimFridge.NewPowerFactorApplied".Translate(), MessageTypeDefOf.PositiveEvent);

					if (Current.Game != null)
					{
						RimFridgeSettingsUtil.ApplyFactor(Settings.PowerFactor.AsFloat);
					}
				}
			}

			Widgets.Label(new Rect(20, 100, 400, 30), "RimFridge.PowerFactorExplanation".Translate());
			Widgets.CheckboxLabeled(new Rect(0, 140, 200, 30), "RimFridge.ActAsTradeBeacon".Translate(), ref Settings.ActAsBeacon);
			Widgets.CheckboxLabeled(new Rect(0, 180, 200, 30), "RimFridge.EnableFrostyBeverages".Translate(), ref Settings.enableFrostyBeverages);

			if (ShouldShowCompatibilitySettings)
			{
				Widgets.DrawMenuSection(new Rect(0, 210, 800, 370));

				Widgets.Label(new Rect(10, 210, 790, 30), "RimFridge.Compatibility".Translate());

				Widgets.Label(new Rect(10, 240, 790, 30), "RimFridge.ForceApplicationOfThesePatches".Translate());
				Widgets.BeginScrollView(new Rect(0, 280, 800, 300), ref guiState.forcedApplicationOfPatchesScrollPosition, new Rect(0, 250, 800 - GenUI.ScrollBarWidth, 30 * Settings.forcedApplicationOfPatches.Count));

				for (int index = 0; index < Settings.forcedApplicationOfPatches.Count; ++index)
				{
					var patch = Settings.forcedApplicationOfPatches[index];

					Widgets.CheckboxLabeled(
						new Rect(20, 280 + index * 30, 780 - GenUI.ScrollBarWidth, 28),
						patch.patch,
						ref patch.shouldForceApplication,
						placeCheckboxNearText: true
					);
				}

				Widgets.EndScrollView();
			}

			GUI.EndGroup();
		}

		public override void WriteSettings ()
		{
			base.WriteSettings();

			if (ShouldShowCompatibilitySettings)
			{
				if (
					!guiState.initialStateOfShouldForceApplicationOfPatches.SequenceEqual(
						Settings.forcedApplicationOfPatches.Select(a => a.shouldForceApplication)
					)
				)
				{
					ModsConfig.RestartFromChangedMods();
				}
			}

			guiState = null;
		}

		protected static bool ShouldShowCompatibilitySettings
		{
			get => Current.Game == null;
		}
	}

	internal class Settings : ModSettings
	{
		public static readonly FloatInput PowerFactor = new FloatInput("RimFridge.BasePowerFactor");
		public static bool ActAsBeacon = false;
		public static bool enableFrostyBeverages = true;
		public static int defaultMaximumItemsPerCell;
		/* Making this a List causes access to be O(n), but we want to maintain
			the order the patches were loaded in. */
		public static List<ApplicationOfPatch> forcedApplicationOfPatches = new();

		internal class ApplicationOfPatch : IExposable
		{
			public string patch;
			public bool shouldForceApplication;

			public void ExposeData ()
			{
				Scribe_Values.Look(ref this.patch, "patch");
				Scribe_Values.Look(ref this.shouldForceApplication, "shouldForceApplication", true);
			}
		}

		public override void ExposeData ()
		{
			base.ExposeData();
			Scribe_Values.Look(ref(PowerFactor.AsString), "RimFridge.PowerFactor", "1.00", false);
			Scribe_Values.Look(ref ActAsBeacon, "RimFridge.ActAsBeacon", false, false);
			Scribe_Values.Look(ref defaultMaximumItemsPerCell, "RimFridge.DefaultMaximumItemsPerCell", 3, false);
			Scribe_Values.Look(ref enableFrostyBeverages, "RimFridge.EnableFrostyBeverages", true, false);
			Scribe_Collections.Look(ref forcedApplicationOfPatches, "RimFridge.ForcedApplicationOfPatches", LookMode.Deep);

			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				forcedApplicationOfPatches ??= new();
			}
		}
	}
}

