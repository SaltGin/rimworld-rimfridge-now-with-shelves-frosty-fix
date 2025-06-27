using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimFridge
{
	[Serializable]
	internal class TranspilerFallbackException : Exception
	{
		public TranspilerFallbackException ()
		{}

		public TranspilerFallbackException (string message) : base(message)
		{}

		public TranspilerFallbackException (string message, Exception innerException) : base (message, innerException)
		{}
	}

	public static class EnsureThatItemsInAFridgeCanBeReachedByPawns
	{
		[HarmonyPatch(
			typeof(Reachability),
			nameof(Reachability.CanReach),
			new[] {typeof(IntVec3), typeof(LocalTargetInfo), typeof(PathEndMode), typeof(TraverseParms)}
		)]
		public static class SetPathEndModeForReachabilityCanReachSuchThatItemsInFridgeMayBeReached
		{
			public static readonly AccessTools.FieldRef<Reachability, Map> mapOfReachability = (
				AccessTools.FieldRefAccess<Reachability, Map>("map")
			);

			[HarmonyPrefix]
			public static void SetPathEndModeSuchThatItemsInFridgeMayBeReached (
				Reachability __instance,
				LocalTargetInfo dest,
				ref PathEndMode peMode,
				TraverseParms traverseParams
			)
			{
				if (
					   dest.Thing?.def.category == ThingCategory.Item
					&& FridgeCacheFast.rimFridgeCache[mapOfReachability(__instance)].TryGetValue(
						dest.Cell,
						out RimFridge_Building rimFridge
					)
				)
				{
					PathEndMode touchMode = peMode == PathEndMode.OnCell ? PathEndMode.Touch : peMode;

					peMode = (
						  (rimFridge.packedState & RimFridge_Building.Flags.itemsRequirePathEndModeOfTouch) != 0
						? touchMode
						: peMode
					);

					if (rimFridge.ForbidsUsageByAnimals)
					{
						Pawn pawn = traverseParams.pawn;
						peMode = (
							  pawn == null || pawn.RaceProps.Humanlike || pawn.IsColonyMechPlayerControlled
							? touchMode
							: PathEndMode.None
						);
					}
				}
			}
		}

		[HarmonyPatch(
			typeof(ReachabilityWithinRegion),
			nameof(ReachabilityWithinRegion.ThingFromRegionListerReachable),
			new[] {typeof(Thing), typeof(Region), typeof(PathEndMode), typeof(Pawn)}
		)]
		public static class SetPathEndModeForThingFromRegionListerReachableSuchThatItemsInWallFridgeMayBeReached
		{
			[HarmonyPrefix]
			public static void SetPathEndModeSuchThatItemsInFridgeMayBeReached (
				Thing thing,
				Region region,
				ref PathEndMode peMode,
				Pawn traveler
			)
			{
				if (
					   thing.def.category == ThingCategory.Item
					&& FridgeCacheFast.rimFridgeCache[region.Map].TryGetValue(
						thing.Position,
						out RimFridge_Building rimFridge
					)
				)
				{
					PathEndMode touchMode = peMode == PathEndMode.OnCell ? PathEndMode.Touch : peMode;

					peMode = (
						  (rimFridge.packedState & RimFridge_Building.Flags.itemsRequirePathEndModeOfTouch) != 0
						? touchMode
						: peMode
					);

					if (rimFridge.ForbidsUsageByAnimals)
					{
						peMode = (
							  traveler == null || traveler.RaceProps.Humanlike || traveler.IsColonyMechPlayerControlled
							? touchMode
							: PathEndMode.None
						);
					}
				}
			}
		}
	}

	[HarmonyBefore(new string[] {"io.github.dametri.thermodynamicscore"})]
	[HarmonyPriority(Priority.First)]
	[HarmonyPatch(typeof(Thing), "AmbientTemperature", MethodType.Getter)]
	static class Patch_Thing_AmbientTemperature
	{
		static bool Prefix (Thing __instance, ref float __result)
		{
			Pawn p = __instance as Pawn;

			if (
				   (p == null || p.Dead)
				&& __instance.Map != null
				&& FridgeCache.TryGetFridge(__instance.Position, __instance.Map, out CompRefrigerator fridge)
				&& fridge != null
			)
			{
				__result = fridge.currentTemp;
				return false;
			}

			return true;
		}
	}


	public static class AllowFridgesToActAsOrbitalTradeBeacons
	{
		[HarmonyPatch(
			typeof(TradeUtility),
			nameof(TradeUtility.AllLaunchableThingsForTrade),
			new[] {typeof(Map), typeof(ITrader)}
		)]
		public static class LaunchItemsFromFridges
		{
			[HarmonyPostfix]
			public static IEnumerable<Thing> MaybeLaunchItemsFromFridges (
				IEnumerable<Thing> originalThings,
				Map map,
				ITrader trader
			)
			{
				foreach (Thing thing in originalThings)
				{
					yield return thing;
				}

				if (!Settings.ActAsBeacon)
				{
					yield break;
				}

				ThingGrid thingGrid = map.thingGrid;

				foreach (RimFridge_Building fridge in FridgeCacheFast.rimFridgeCache[map].Values)
				{
					StorageSettings storageSettings = fridge.settings;

					foreach (IntVec3 cell in fridge.AllSlotCells())
					{
						foreach (Thing refrigeratedItem in thingGrid.ThingsListAtFast(cell))
						{
							if (
								   refrigeratedItem.def.category == Verse.ThingCategory.Item
								&& TradeUtility.PlayerSellableNow(refrigeratedItem, trader)
								&& storageSettings.AllowedToAccept(refrigeratedItem)
							)
							{
								yield return refrigeratedItem;
							}
						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(FoodUtility), nameof(FoodUtility.TryFindBestFoodSourceFor))]
	static class EnsureThatPrisonersGetFoodFromFridgesInPrisons
	{
		static void Postfix (ref bool __result, Pawn getter, Pawn eater, ref Thing foodSource, ref ThingDef foodDef)
		{
			if (
				   __result == false
				&& getter.Map != null
				&& getter == eater
				&& getter.RaceProps.ToolUser
				&& getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
			{
				Room prison = getter.Position.GetRoomOrAdjacent(getter.Map);

				if (prison != null && prison.IsPrisonCell)
				{
					foreach (Thing t in prison.ContainedAndAdjacentThings)
					{
						if (
							   t.Map != null
							&& t is Building_Storage storage
							&& !t.IsForbidden(getter)
						)
						{
							foreach (IntVec3 cell in storage.AllSlotCells())
							{
								foreach (Thing possibleFood in t.Map.thingGrid.ThingsAt(cell))
								{
									if (
										   getter.RaceProps.CanEverEat(possibleFood)
										&& !possibleFood.IsForbidden(getter)
										&& storage.Map.reservationManager.CanReserve(getter, new LocalTargetInfo(possibleFood))
									)
									{
										__result = true;
										foodSource = possibleFood;
										foodDef = possibleFood.def;
										return;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public static class DisplayStackedItemsNicelyInFridges
	{
		[HarmonyPatch(typeof(GenThing), nameof(GenThing.TrueCenter), new[] {typeof(Thing)})]
		public static class MungeTrueCenterOfItemsInFridges
		{
			[HarmonyPostfix]
			public static Vector3 MungeTrueCenter (Vector3 originalValue, Thing t)
			{
				if (t.def.category == ThingCategory.Item && t.Spawned)
				{
					var things = t.Map.thingGrid.ThingsListAtFast(t.Position);

					if (things.Count > 2)
					{
						var thingID = t.thingIDNumber;
						int depthInStack = 0;
						bool haveFridgeInCell = false;

						foreach (var eachThing in things)
						{
							haveFridgeInCell = haveFridgeInCell || eachThing is RimFridge_Building;
							depthInStack += (
								eachThing.thingIDNumber < thingID
								&& eachThing.def.category == ThingCategory.Item
							) ? 1 : 0;
						}

						if (haveFridgeInCell)
						{
							IntVec3 p = t.Position;
							Vector3 v = p.ToVector3Shifted();
							float altitude = t.def.Altitude;

							return new Vector3(
								v.x,
								altitude + (float) depthInStack * (3f / 74f) / 10f,
								v.z + (float) depthInStack / 16f - 0.05f
							);
						}
					}
				}

				return originalValue;
			}
		}

		[HarmonyPatch(typeof(GenMapUI), nameof(GenMapUI.LabelDrawPosFor), new[] {typeof(Thing), typeof(float)})]
		public static class MakeTheStackCountLabelsReadable
		{
			[HarmonyPostfix]
			public static Vector2 OffsetTheLabels (Vector2 originalValue, Thing thing)
			{
				if (thing.def.category == ThingCategory.Item && thing.Spawned)
				{
					var things = thing.Map.thingGrid.ThingsListAtFast(thing.Position);

					if (things.Count > 2)
					{
						var thingID = thing.thingIDNumber;
						int depthInStack = -1;
						bool haveFridgeInCell = false;

						foreach (var eachThing in things)
						{
							haveFridgeInCell = haveFridgeInCell || eachThing is RimFridge_Building;
							depthInStack += (
								eachThing.thingIDNumber < thingID
								&& eachThing.def.category == ThingCategory.Item
							) ? 1 : 0;
						}

						if (haveFridgeInCell)
						{
							originalValue.x += (float) depthInStack * 17.0f;
						}
					}
				}

				return originalValue;
			}
		}
	}

	public static class HacksForCompatibility
	{
		[HarmonyPatch(
			typeof(BackCompatibility),
			nameof(BackCompatibility.GetBackCompatibleType),
			new[] {typeof(Type), typeof(string), typeof(XmlNode)}
		)]
		public static class ChangeTheClassOfOldWallFridges
		{
			[HarmonyPrefix]
			public static bool GetBackCompatibleType (
				ref Type __result,
				Type baseType,
				string providedClassName,
				XmlNode node
			)
			{
				/* This is a bit of a kludge, but we do it only once for each affected save,
				   and the patch is applied and removed on demand,
				   so there's no performance impact in the usual case. */

				if (providedClassName != "RimFridge.RimFridge_Building")
				{
					return true;
				}

				if (node != null)
				{
					foreach (XmlNode childNode in node)
					{
						if (childNode.NodeType == XmlNodeType.Element)
						{
							XmlElement element = (XmlElement) childNode;

							if (element.Name == "def")
							{
								if (
									   element.InnerXml == "RimFridge_SingleWallRefrigerator"
									|| element.InnerXml == "RimFridge_WallRefrigerator"
								)
								{
									Logger.Message("Found an old-style wall-fridge; migrating it to the new-style.");
									__result = typeof(RimFridge_DoubleSidedWallBuilding);
									return false;
								}

								break;
							}
						}
					}
				}

				__result = typeof(RimFridge_Building);
				return false;
			}
		}


		[HarmonyPatch(typeof(LoadedModManager), nameof(LoadedModManager.ErrorCheckPatches))]
		public static class ForceTheApplicationOfSomePatches
		{
			[HarmonyPostfix]
			public static void RifleThroughThePatchesThatAreAboutToBeApplied ()
			{
				/* If an exception is thrown by ErrorCheckPatches, the game stops loading and all that's
					left is a black screen. To avoid being the mod that breaks a user's game, we catch
					ANY exception. */
				try
				{
					var rimFridge = LoadedModManager.GetMod<SettingsController>();
					var rimFridgeMetadata = rimFridge.Content.ModMetaData;
					var rimFridgeIndex = Compatibility.FindIndexOfModInActiveModLoadOrder(rimFridge);

					var patchesToNotForceApplicationOf = new HashSet<string>(
						Settings.forcedApplicationOfPatches.Where(a => !a.shouldForceApplication).Select(a => a.patch)
					);

					var applicationOfPatches = new List<Settings.ApplicationOfPatch>();

					var modsField = typeof(PatchOperationFindMod).GetField("mods", BindingFlags.Instance | BindingFlags.NonPublic);
					var applicableModNamesOf = (PatchOperationFindMod p) => (List<string>) modsField.GetValue(p);

					foreach (ModContentPack mod in LoadedModManager.RunningModsListForReading.Skip(rimFridgeIndex + 1))
					{
						var patches = mod.Patches.OfType<PatchOperationFindMod>();

						if (patches.Any(p => applicableModNamesOf (p).Any(name => name == rimFridgeMetadata.Name)))
						{
							/* This mod has at-least one patch that targets us, so we're not going to force the
								application of any patches, as that would likely cause problems if the mod's already
								accounted for us in a potentially-separate patch. */
							continue;
						}

						foreach (var patch in patches)
						{
							try
							{
								var applicableModNames = applicableModNamesOf(patch);

								if (applicableModNames.Any(Compatibility.IsRecognisedRimFridgeModName))
								{
									var patchID = Compatibility.IdentifierForPatch(patch, of: mod);

									var forcingApplication = !patchesToNotForceApplicationOf.Contains(patchID);

									if (forcingApplication)
									{
										Logger.Message($"Forcing the application of patch {patchID} of {mod.ModMetaData.Name}.");

										applicableModNames.Add(rimFridgeMetadata.Name);
									}

									applicationOfPatches.Add(
										new()
										{
											patch = patchID,
											shouldForceApplication = forcingApplication
										}
									);
								}
							}
							catch (Exception e)
							{
								Logger.Error(e.ToString());
							}
						}
					}

					applicationOfPatches.TrimExcess();
					Settings.forcedApplicationOfPatches = applicationOfPatches;
				}
				catch (Exception e)
				{
					Logger.Error(e.ToString());
				}
			}
		}
	}

	internal static class CodeInstructionExtensions
	{
		internal static CodeInstruction LabelWith (this CodeInstruction instruction, Label label)
		{
			instruction.labels.Add(label);
			return instruction;
		}

		internal static CodeInstruction TakeLabelsFrom (this CodeInstruction instruction, CodeInstruction labelSource)
		{
			instruction.labels.AddRange(labelSource.labels);
			labelSource.labels.Clear();
			return instruction;
		}

		internal static bool LoadsLocal (this CodeInstruction instruction)
		{
			return instruction.LoadsLocal(out int actualIndex);
		}

		internal static bool LoadsLocal (this CodeInstruction instruction, int localIndex)
		{
			return instruction.LoadsLocal(out int actualIndex) ? localIndex == actualIndex : false;
		}

		internal static bool LoadsLocal (this CodeInstruction instruction, out int localIndex)
		{
			uint opcode = (ushort) instruction.opcode.Value;

			if (opcode == 0x0011) /* Ldloc_S */
			{
				localIndex = instruction.operand is LocalBuilder l ? l.LocalIndex : (byte) instruction.operand;
				return true;
			}
			else if (opcode >= 0x0006) /* Ldloc_0 */
			{
				if (opcode <= 0x0009) /* Ldloc_3 */
				{
					localIndex = (int) (opcode - 0x0006);
					return true;
				}

				if (opcode == 0xFE0C) /* Ldloc */
				{
					localIndex = instruction.operand is LocalBuilder l ? l.LocalIndex : (ushort) instruction.operand;
					return true;
				}
			}

			localIndex = -1;
			return false;
		}

		internal static bool StoresLocal (this CodeInstruction instruction)
		{
			return instruction.StoresLocal(out int actualIndex);
		}

		internal static bool StoresLocal (this CodeInstruction instruction, int localIndex)
		{
			return instruction.StoresLocal(out int actualIndex) ? localIndex == actualIndex : false;
		}

		internal static bool StoresLocal (this CodeInstruction instruction, out int localIndex)
		{
			uint opcode = (ushort) instruction.opcode.Value;

			if (opcode == 0x0013) /* Stloc_S */
			{
				localIndex = instruction.operand is LocalBuilder l ? l.LocalIndex : (byte) instruction.operand;
				return true;
			}
			else if (opcode >= 0x000A) /* Stloc_0 */
			{
				if (opcode <= 0x000D) /* Stloc_3 */
				{
					localIndex = (int) (opcode - 0x000A);
					return true;
				}

				if (opcode == 0xFE0E) /* Stloc */
				{
					localIndex = instruction.operand is LocalBuilder l ? l.LocalIndex : (ushort) instruction.operand;
					return true;
				}
			}

			localIndex = -1;
			return false;
		}

		internal static bool LoadsLocalAddress (this CodeInstruction instruction)
		{
			return instruction.LoadsLocalAddress(out int actualIndex);
		}

		internal static bool LoadsLocalAddress (this CodeInstruction instruction, int localIndex)
		{
			return instruction.LoadsLocalAddress(out int actualIndex) ? localIndex == actualIndex : false;
		}

		internal static bool LoadsLocalAddress (this CodeInstruction instruction, out int localIndex)
		{
			uint opcode = (ushort) instruction.opcode.Value;

			if (opcode == 0x0012) /* Ldloca_S */
			{
				localIndex = instruction.operand is LocalBuilder l ? l.LocalIndex : (byte) instruction.operand;
				return true;
			}
			else if (opcode == 0xFE0D) /* Ldloca */
			{
				localIndex = instruction.operand is LocalBuilder l ? l.LocalIndex : (ushort) instruction.operand;
				return true;
			}

			localIndex = -1;
			return false;
		}
	}
}

