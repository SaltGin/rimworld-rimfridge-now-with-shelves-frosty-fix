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
		public static Type appliedHandleTheProprietyOfWallFridgesPatch;
		public static Type appliedMungeTrueCenterOfItemsInFridgesPatch;
		public static Type appliedMakeTheStackCountLabelsReadablePatch;
		public static Type appliedDissuadeColonistsFromPathingToWallFridgesViaPrisonCellsPatch;

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
			FridgeCacheFast.multiSidedCache = new Dictionary<Map, Dictionary<IntVec3, RimFridge_MultiSidedWallBuilding>>();
			FridgeCacheFast.compList = new Dictionary<Map, List<CompRefrigerator>>();
			FridgeCacheFast.rimFridgeList = new Dictionary<Map, List<RimFridge_Building>>();
			FridgeCacheFast.wallFridgeList = new Dictionary<Map, List<RimFridge_WallBuilding>>();
			FridgeCacheFast.multiSidedList = new Dictionary<Map, List<RimFridge_MultiSidedWallBuilding>>();

			Patch(typeof(EnsureThatItemsInAFridgeCanBeReachedByPawns.SetPathEndModeForReachabilityCanReachSuchThatItemsInFridgeMayBeReached));
			Patch(typeof(EnsureThatItemsInAFridgeCanBeReachedByPawns.SetPathEndModeForThingFromRegionListerReachableSuchThatItemsInWallFridgeMayBeReached));

			appliedHandleTheProprietyOfWallFridgesPatch = PatchWithFallback(
				typeof(EnsureThatItemsInAFridgeCanBeReachedByPawns.HandleTheProprietyOfWallFridges.IsSociallyProperTranspiler),
				typeof(EnsureThatItemsInAFridgeCanBeReachedByPawns.HandleTheProprietyOfWallFridges.IsSociallyProperPostfix)
			);

			Patch(typeof(Patch_Thing_AmbientTemperature));

			Patch(typeof(AllowFridgesToActAsOrbitalTradeBeacons.LaunchItemsFromFridges));
			Patch(typeof(AllowFridgesToActAsOrbitalTradeBeacons.WorkaroundCommsConsoleStupidity));

			Patch(typeof(HandleDeathPallsProperlyForCorpsesInWallFridges.TreatWallFridgesAsIndoorsIfAppropriate));

			Patch(typeof(HacksForCompatibility.ForceTheApplicationOfSomePatches));

			Patch(typeof(PrisonCellChangeTracking.TrackChangeOfPrisonCellStatusForRoom));

			base.GetSettings<Settings>();

			Settings.SetDefaultSettings(this);
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
			internal string prisonCellSideAvoidanceStrengthBuffer;
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

			Widgets.CheckboxLabeled(new Rect(0, 140, 300, 30), "RimFridge.ActAsTradeBeacon".Translate(), ref Settings.ActAsBeacon);
			TooltipHandler.TipRegion(new Rect(0, 140, 300, 30), "RimFridge.ActAsTradeBeaconDescription".Translate());
			Widgets.CheckboxLabeled(new Rect(0, 180, 300, 30), "RimFridge.EnableFrostyBeverages".Translate(), ref Settings.enableFrostyBeverages);
			TooltipHandler.TipRegion(new Rect(0, 180, 300, 30), "RimFridge.EnableFrostyBeveragesDescription".Translate());
			Widgets.CheckboxLabeled(new Rect(0, 210, 300, 30), "RimFridge.UglyStackAppearance".Translate(), ref Settings.uglyStackAppearance);
			TooltipHandler.TipRegion(new Rect(0, 210, 300, 30), "RimFridge.UglyStackAppearanceDescription".Translate());

			Widgets.CheckboxLabeled(new Rect(330, 140, 300, 30), "RimFridge.WallFridgesBlockLight".Translate(), ref Settings.wallFridgesBlockLight);
			TooltipHandler.TipRegion(new Rect(330, 140, 300, 30), "RimFridge.WallFridgesBlockLightDescription".Translate());

			Widgets.Label(new Rect(0, 240, 300, 30), "RimFridge.DefaultMaximumItemsPerCellSetting".Translate(Settings.defaultMaximumItemsPerCell));
			TooltipHandler.TipRegion(new Rect(0, 240, 300, 30), "RimFridge.DefaultMaximumItemsPerCellSettingDescription".Translate());

			if (
				   Settings.defaultMaximumItemsPerCell > 0
				&& Widgets.ButtonText(new Rect(320, 240, 30, 20), "-")
			)
			{
				--Settings.defaultMaximumItemsPerCell;
			}

			if (
				   Settings.defaultMaximumItemsPerCell < maximumDefaultMaximumItemsPerCell
				&& Widgets.ButtonText(new Rect(360, 240, 30, 20), "+")
			)
			{
				++Settings.defaultMaximumItemsPerCell;
			}

			Widgets.Label(new Rect(0, 270, 300, 30), "RimFridge.PrisonCellSideAvoidanceStrength".Translate());
			TooltipHandler.TipRegion(new Rect(0, 270, 300, 30), "RimFridge.PrisonCellSideAvoidanceStrengthDescription".Translate(1600));
			Widgets.IntEntry(new Rect(320, 270, 320, 30), ref Settings.prisonCellSideAvoidanceStrength, ref guiState.prisonCellSideAvoidanceStrengthBuffer, 100);

			if (ShouldShowCompatibilitySettings)
			{
				Widgets.DrawMenuSection(new Rect(0, 310, 800, 240));

				Widgets.Label(new Rect(10, 310, 790, 30), "RimFridge.Compatibility".Translate());

				Widgets.Label(new Rect(10, 330, 790, 30), "RimFridge.ForceApplicationOfThesePatches".Translate());
				Widgets.BeginScrollView(new Rect(0, 370, 800, 180), ref guiState.forcedApplicationOfPatchesScrollPosition, new Rect(0, 370, 800 - GenUI.ScrollBarWidth, 30 * Settings.forcedApplicationOfPatches.Count));

				for (int index = 0; index < Settings.forcedApplicationOfPatches.Count; ++index)
				{
					var patch = Settings.forcedApplicationOfPatches[index];

					Widgets.CheckboxLabeled(
						new Rect(20, 370 + index * 30, 780 - GenUI.ScrollBarWidth, 28),
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

			if (guiState != null)
			{
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

			Settings.Reify();
		}

		protected static bool ShouldShowCompatibilitySettings
		{
			get => Current.Game == null;
		}
	}

	internal class Settings : ModSettings
	{
		public static FloatInput PowerFactor;
		public static bool ActAsBeacon;
		public static int defaultMaximumItemsPerCell;
		public static bool enableFrostyBeverages;
		public static bool uglyStackAppearance;
		public static bool wallFridgesBlockLight;
		public static int prisonCellSideAvoidanceStrength;
		/* Making this a List causes access to be O(n), but we want to maintain
			the order the patches were loaded in. */
		public static List<ApplicationOfPatch> forcedApplicationOfPatches;

		internal static bool loadedSettings;
		internal static int schemaVersion;

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

			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				loadedSettings = true;
			}

			if (Scribe.mode == LoadSaveMode.Saving)
			{
				int latestSchemaVersion = 1;
				Scribe_Values.Look(ref latestSchemaVersion, "RimFridge.SchemaVersion", 0, true);
			}
			else
			{
				Scribe_Values.Look(ref schemaVersion, "RimFridge.SchemaVersion", 0, true);
			}

			Scribe_Values.Look(ref(PowerFactor.AsString), "RimFridge.PowerFactor", "1.00", true);
			Scribe_Values.Look(ref ActAsBeacon, "RimFridge.ActAsBeacon", false, true);
			Scribe_Values.Look(ref defaultMaximumItemsPerCell, "RimFridge.DefaultMaximumItemsPerCell", 3, true);
			Scribe_Values.Look(ref enableFrostyBeverages, "RimFridge.EnableFrostyBeverages", true, true);
			Scribe_Values.Look(ref uglyStackAppearance, "RimFridge.UglyStackAppearance", false, true);
			Scribe_Values.Look(ref wallFridgesBlockLight, "RimFridge.WallFridgesBlockLight", false, true);
			Scribe_Values.Look(ref prisonCellSideAvoidanceStrength, "RimFridge.PrisonCellSideAvoidanceStrength", -1, true);
			Scribe_Collections.Look(ref forcedApplicationOfPatches, "RimFridge.ForcedApplicationOfPatches", LookMode.Deep);

			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				forcedApplicationOfPatches ??= new();
				Reify();
			}
		}

		public static void SetDefaultSettings (SettingsController mod)
		{
			if (Settings.loadedSettings)
			{
				/* v2.0.0 introduced an issue that caused the default settings
				   to not get set when the player had no existing configuration for the mod.
				   (ExposeData isn't called when a mod's settings are first created by the game).
				   This meant that `defaultMaximumItemsPerCell` and `prisonCellSideAvoidanceStrength`
				   would get saved as 0, so if either of them are zero we'll set the correct defaults,
				   unless `schemaVersion` isn't zero, in which case we'll respect the user's configuration. */
				if (schemaVersion != 0 || defaultMaximumItemsPerCell != 0 && prisonCellSideAvoidanceStrength != 0)
				{
					return;
				}

				Logger.Warning($"Broken mod options from v2.0.0-to-v2.0.2 detected. Overriding these settings: 'Enable Frosty Beverages'; 'Default maximum items per cell'; 'Prison-cell side avoidance strength'.");
			}

			Settings.loadedSettings = true;

			defaultMaximumItemsPerCell = 3;
			enableFrostyBeverages = true;
			prisonCellSideAvoidanceStrength = -1;

			mod.WriteSettings();
		}

		public static void Reify ()
		{
			if (uglyStackAppearance)
			{
				if (SettingsController.appliedMungeTrueCenterOfItemsInFridgesPatch != null)
				{
					SettingsController.Unpatch(SettingsController.appliedMungeTrueCenterOfItemsInFridgesPatch);
					SettingsController.appliedMungeTrueCenterOfItemsInFridgesPatch = null;
				}

				if (SettingsController.appliedMakeTheStackCountLabelsReadablePatch != null)
				{
					SettingsController.Unpatch(SettingsController.appliedMakeTheStackCountLabelsReadablePatch);
					SettingsController.appliedMakeTheStackCountLabelsReadablePatch = null;
				}
			}
			else
			{
				if (SettingsController.appliedMungeTrueCenterOfItemsInFridgesPatch == null)
				{
					SettingsController.appliedMungeTrueCenterOfItemsInFridgesPatch = SettingsController.PatchWithFallback(
						typeof(DisplayStackedItemsNicelyInFridges.MungeTrueCenterOfItemsInFridges.MungeItemCenterTranspiler),
						typeof(DisplayStackedItemsNicelyInFridges.MungeTrueCenterOfItemsInFridges.MungeTrueCenterPostfix)
					);
				}

				if (SettingsController.appliedMakeTheStackCountLabelsReadablePatch == null)
				{
					SettingsController.appliedMakeTheStackCountLabelsReadablePatch = SettingsController.PatchWithFallbacks(
						typeof(DisplayStackedItemsNicelyInFridges.MakeTheStackCountLabelsReadable.OffsetTheLabelsTranspiler),
						typeof(DisplayStackedItemsNicelyInFridges.MakeTheStackCountLabelsReadable.OffsetTheLabelsFallback),
						typeof(DisplayStackedItemsNicelyInFridges.MakeTheStackCountLabelsReadable.OffsetTheLabelsSlowPostfix)
					);
				}
			}

			ushort oldPrisonCellSideAvoidancePathFindCost = RimFridge_MultiSidedWallBuilding.prisonCellSideAvoidancePathFindCost;

			RimFridge_MultiSidedWallBuilding.prisonCellSideAvoidancePathFindCost = (ushort) (
				  prisonCellSideAvoidanceStrength < 0
				? 1600
				: (
					  prisonCellSideAvoidanceStrength > 0xFFFF
					? 0xFFFF
					: prisonCellSideAvoidanceStrength
				)
			);

			if (RimFridge_MultiSidedWallBuilding.prisonCellSideAvoidancePathFindCost == 0)
			{
				if (SettingsController.appliedDissuadeColonistsFromPathingToWallFridgesViaPrisonCellsPatch != null)
				{
					SettingsController.Unpatch(SettingsController.appliedMungeTrueCenterOfItemsInFridgesPatch);
					SettingsController.appliedDissuadeColonistsFromPathingToWallFridgesViaPrisonCellsPatch = null;
				}
			}
			else
			{
				if (SettingsController.appliedDissuadeColonistsFromPathingToWallFridgesViaPrisonCellsPatch == null)
				{
					SettingsController.Patch(
						typeof(EnsureThatItemsInAFridgeCanBeReachedByPawns.HandleTheProprietyOfWallFridges.DissuadeColonistsFromPathingToWallFridgesViaPrisonCells)
					);
					SettingsController.appliedDissuadeColonistsFromPathingToWallFridgesViaPrisonCellsPatch = (
						typeof(EnsureThatItemsInAFridgeCanBeReachedByPawns.HandleTheProprietyOfWallFridges.DissuadeColonistsFromPathingToWallFridgesViaPrisonCells)
					);
				}
			}

			if (Current.Game != null)
			{
				ReifyLoadedGameDependentSettings();

				if (oldPrisonCellSideAvoidancePathFindCost != RimFridge_MultiSidedWallBuilding.prisonCellSideAvoidancePathFindCost)
				{
					foreach (Map map in Find.Maps)
					{
						if (FridgeCacheFast.multiSidedList.TryGetValue(map, out List<RimFridge_MultiSidedWallBuilding> list))
						{
							foreach (RimFridge_MultiSidedWallBuilding multiSided in list)
							{
								multiSided.ReactToChangeOfPrisonCellStatusForRoom();
							}
						}
					}
				}
			}
		}

		public static void ReifyLoadedGameDependentSettings ()
		{
			bool blockLight = Settings.wallFridgesBlockLight;

			DefDatabase<ThingDef>.GetNamed("RimFridge_SingleWallRefrigerator").blockLight = blockLight;
			DefDatabase<ThingDef>.GetNamed("RimFridge_WallRefrigerator").blockLight = blockLight;

			foreach (Map map in Find.Maps)
			{
				if (FridgeCacheFast.multiSidedList.TryGetValue(map, out List<RimFridge_MultiSidedWallBuilding> list))
				{
					GlowGrid glowGrid = map.glowGrid;

					foreach (RimFridge_MultiSidedWallBuilding multiSided in list)
					{
						foreach (IntVec3 cell in multiSided.OccupiedRect().Cells)
						{
							if (blockLight)
							{
								glowGrid.LightBlockerAdded(cell);
							}
							else
							{
								glowGrid.LightBlockerRemoved(cell);
							}
						}
					}
				}
			}
		}
	}
}

