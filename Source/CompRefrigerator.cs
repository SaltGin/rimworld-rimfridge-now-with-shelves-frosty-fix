using RimWorld;
using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Collections.Generic;

namespace RimFridge
{
	public interface ICompRefrigeratorParent
	{
		public float GetTemperatureOfSurroundings (CompRefrigerator comp);
		public void PushTransferredAndGeneratedHeat (float energy, CompRefrigerator comp);
	}

	public sealed class CompRefrigerator : ThingComp
	{
		public float desiredTemp;
		public float currentTemp = 21f;

		public StorageSettings fixedStorageSettings;
		public CompPowerTrader powerTrader => parent.GetComp<CompPowerTrader>();
		private CompRefuelable refuelable => parent.GetComp<CompRefuelable>();

		public bool ShouldBeActive => (powerTrader != null && powerTrader.PowerOn) || (refuelable != null && refuelable.HasFuel);

		public override IEnumerable<Gizmo> CompGetGizmosExtra ()
		{
			float offsetN10 = RoundedToCurrentTempModeOffset(-10f);
			float offsetN1 = RoundedToCurrentTempModeOffset(-1f);
			float offset1 = RoundedToCurrentTempModeOffset(1f);
			float offset10 = RoundedToCurrentTempModeOffset(10f);

			foreach (Gizmo g in base.CompGetGizmosExtra())
			{
				yield return g;
			}

			yield return new Command_Action
			{
				action = delegate { InterfaceChangeTargetTemperature(offsetN10); },
				defaultLabel = offsetN10.ToStringTemperatureOffset("F0"),
				defaultDesc = "CommandLowerTempDesc".Translate(),
				//hotKey = KeyBindingDefOf.Misc5,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true)
			};
			yield return new Command_Action
			{
				action = delegate { InterfaceChangeTargetTemperature(offsetN1); },
				defaultLabel = offsetN1.ToStringTemperatureOffset("F0"),
				defaultDesc = "CommandLowerTempDesc".Translate(),
				//hotKey = KeyBindingDefOf.Misc4,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true)
			};
			yield return new Command_Action
			{
				action = () =>
				{
					SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny);
					desiredTemp = defaultDesiredTemperature;
				},
				defaultLabel = "CommandResetTemp".Translate(),
				defaultDesc = "CommandResetTempDesc".Translate(),
				//hotKey = KeyBindingDefOf.Misc1,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempReset", true)
			};
			yield return new Command_Action
			{
				action = delegate { InterfaceChangeTargetTemperature(offset1); },
				defaultLabel = "+" + offset1.ToStringTemperatureOffset("F0"),
				defaultDesc = "CommandRaiseTempDesc".Translate(),
				//hotKey = KeyBindingDefOf.Misc2,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true)
			};
			yield return new Command_Action
			{
				action = delegate { InterfaceChangeTargetTemperature(offset10); },
				defaultLabel = "+" + offset10.ToStringTemperatureOffset("F0"),
				defaultDesc = "CommandRaiseTempDesc".Translate(),
				//hotKey = KeyBindingDefOf.Misc3,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true)
			};
		}

		public List<string> drinksBestCold => ((CompProperties_Refrigerator) props).drinksBestCold;
		public float defaultDesiredTemperature => ((CompProperties_Refrigerator) props).defaultDesiredTemperature;

		private void InterfaceChangeTargetTemperature (float offset)
		{
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.DragSlider);
			desiredTemp += offset;
			desiredTemp = Mathf.Clamp(desiredTemp, -270f, 270f);
		}

		private float RoundedToCurrentTempModeOffset (float celsiusTemp)
		{
			float num = GenTemperature.CelsiusToOffset(celsiusTemp, Prefs.TemperatureMode);
			num = Mathf.RoundToInt(num);
			return GenTemperature.ConvertTemperatureOffset(num, Prefs.TemperatureMode, TemperatureDisplayMode.Celsius);
		}

		public override void CompTickRare ()
		{
			base.CompTickRare();

			Thing parent = this.parent;

			if (!parent.Spawned)
			{
				return;
			}

			if (Settings.enableFrostyBeverages)
			{
				ThingGrid thingGrid = parent.Map.thingGrid;

				/* Check for beverages which are best enjoyed cold. */
				foreach (IntVec3 cell in ((Building_Storage) parent).AllSlotCells())
				{
					List<Thing> thingList = thingGrid.ThingsListAtFast(cell);
					int thingCount = thingList.Count;

					if (thingCount > 1)
					{
						int index = 0;

						do
						{
							Thing thing = thingList[index];
							ThingDef def = thing.def;

							if (
								   def.category == ThingCategory.Item
								&& thing is ThingWithComps thingWithComps
								&& this.drinksBestCold.Contains(def.defName)
								&& thingWithComps.GetComp<CompFrosty>() == null
							)
							{
								CompFrosty compFrosty = new CompFrosty();
								compFrosty.parent = thingWithComps;
								CompManipulation.AddCompTo(thingWithComps, compFrosty, CompProperties_Frosty.Beer);

								/* If this thing's ticker-type is rare,
								   it will have already been registered in the rare-tick-list
								   by `Thing#SpawnSetup`; if so we won't register it again. */
								if (def.tickerType != TickerType.Rare)
								{
									CompFrosty.tickListRareOfTickManager(Find.TickManager).RegisterThing(thingWithComps);
								}
							}
						}
						while (++index < thingCount);
					}
				}
			}

			IntVec3 position = new IntVec3{};
			Map map = null;
			ICompRefrigeratorParent fastParent = null;
			float roomTemperature;

			if (parent is ICompRefrigeratorParent f)
			{
				fastParent = f;
				roomTemperature = fastParent.GetTemperatureOfSurroundings(this);
			}
			else
			{
				position = parent.Position;
				map = parent.Map;

				// This bit will work for normal furniture RimFridges
				if  (!GenTemperature.TryGetDirectAirTemperatureForCell(position, map, out roomTemperature))
				{
					// This is if it's a wall-mount RimFridge and not part of a "room"
					GenTemperature.TryGetAirTemperatureAroundThing(parent, out roomTemperature);
				}
			}

			float changetemperature = (roomTemperature - currentTemp) * 0.01f;
			float changeEnergy = -changetemperature;
			float powerMultiplier = 0f;

			if (currentTemp + changetemperature > desiredTemp)
			{
				// When the RimFridge's compressor is working and it's pushing the internal temperature down, it can draw a lot of power!
				// Once it gets to temp, maintaining it isn't bad.
				float change = Mathf.Max(desiredTemp - (currentTemp + changetemperature), -3f);

				if (ShouldBeActive)  //Using this just in case someone wants to make a wood-burning RimFridge
				{
					changetemperature += change;
					changeEnergy -= change * 1.25f;
				}

				powerMultiplier = -change;
			}

			// Like all refrigerators, the RimFridge is insulated.  It won't instantly drop to room-temp from loss of power and things inside
			// should be good through brief power interruptions.
			currentTemp += changetemperature;

			changeEnergy *= 1.25f;

			if (fastParent != null)
			{
				fastParent.PushTransferredAndGeneratedHeat(changeEnergy, this);
			}
			else
			{
				IntVec3 pos = position + IntVec3.North.RotatedBy(parent.Rotation);
				GenTemperature.PushHeat(pos, map, changeEnergy);
			}

			if (powerTrader != null)
			{
				powerTrader.PowerOutput = -((CompProperties_Power) powerTrader.props).PowerConsumption * ((powerMultiplier * 0.9f) + 0.1f);
			}
		}

		private void CreateFixedStorageSettings ()
		{
			fixedStorageSettings = new StorageSettings();

			if (parent is Building_Storage storage)
			{
				fixedStorageSettings.CopyFrom(storage.def.building.fixedStorageSettings);
			}

			if ((parent.GetComp<CompRefrigerator>().props as CompProperties_Refrigerator)?.findAllRottableForFilters == true)
			{
				foreach (ThingDef td in DefDatabase<ThingDef>.AllDefs)
				{
					if ((td.HasComp<CompRottable>() || td.HasComp<CompTemperatureRuinable>()) && !fixedStorageSettings.filter.Allows(td))
					{
						fixedStorageSettings.filter.SetAllow(td, true);
					}
				}
			}
		}
		public override void Initialize (CompProperties props)
		{
			base.Initialize(props);
			CreateFixedStorageSettings();

			if (parent is Building_Storage b)
			{
				b.def.building.fixedStorageSettings = fixedStorageSettings;
				b.settings = new StorageSettings(b);

				if (b.def.building.defaultStorageSettings != null)
				{
					b.settings.CopyFrom(b.def.building.defaultStorageSettings);
				}

				desiredTemp = defaultDesiredTemperature;
			}
		}

		public override void PostSpawnSetup (bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

			Thing parent = this.parent;
			FridgeCacheFast.AddToCache(FridgeCacheFast.compCache[parent.Map], this, GenAdj.OccupiedRect(parent));
		}

		public override void PostDeSpawn (Map map, DestroyMode destroyMode)
		{
			FridgeCacheFast.RemoveFromCache(FridgeCacheFast.compCache[map], GenAdj.OccupiedRect(this.parent));

			base.PostDeSpawn(map, destroyMode);
		}

		public override void PostExposeData ()
		{
			base.PostExposeData();

			Scribe_Values.Look(ref currentTemp, "currentTemp", 21f, false);
			Scribe_Values.Look(ref desiredTemp, "desiredTemp", defaultDesiredTemperature, false);

			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				string buildingLabel = null;
				Scribe_Values.Look(ref buildingLabel, "buildingLabel");

				/* Versions older than 1.2.0 of RimFridge handled the fridge label via this comp, rather
				   than via the fridge buildings. So, if this comp was previously renamed we migrate its
				   name to its parent RimFridge (if it is a RimFridge). */
				if (buildingLabel != null && this.parent is RimFridge_Building rimFridge)
				{
					rimFridge.fridgeLabel = buildingLabel;
				}

				CreateFixedStorageSettings();
			}
		}

		public override string CompInspectStringExtra ()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("RimFridge.TargetTemperature".Translate());
			sb.Append(": ");
			sb.Append(GenText.ToStringTemperature(desiredTemp, "F0"));
			sb.Append(Environment.NewLine);
			sb.Append("RimFridge.CurrentTemperature".Translate());
			sb.Append(": ");
			sb.Append(GenText.ToStringTemperature(currentTemp, "F0"));
			sb.Append(Environment.NewLine);
			sb.Append("RimFridge.Power".Translate());
			sb.Append(": ");

			if (powerTrader != null)
			{
				sb.Append((powerTrader != null && powerTrader.PowerOn) ? "On".Translate() : "Off".Translate());
			}

			return sb.ToString().TrimEndNewlines();
		}
	}
}

