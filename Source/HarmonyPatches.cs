using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
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

		public static class HandleTheProprietyOfWallFridges
		{
			public static bool IsThingSociallyProperForPrisonerInRoom (Thing thing, Room roomOfPawn)
			{
				if (!FridgeCacheFast.wallFridgeCache[thing.Map].TryGetValue(thing.Position, out RimFridge_WallBuilding wallFridge))
				{
					return false;
				}

				Room[] adjacentRooms = wallFridge.rooms;
				int roomCount = adjacentRooms.Length;

				for (int index = 0; index < roomCount; ++index)
				{
					if (adjacentRooms[index] == roomOfPawn)
					{
						return true;
					}
				}

				return false;
			}

			public static bool IsSociallyProperThingInWallFridge (Thing thing, Pawn pawn, ref bool propriety)
			{
				if (!FridgeCacheFast.wallFridgeCache[thing.Map].TryGetValue(thing.Position, out RimFridge_WallBuilding wallFridge))
				{
					return false;
				}

				Room[] adjacentRooms = wallFridge.rooms;
				int roomCount = adjacentRooms.Length;

				if (thing.def == RimWorld.ThingDefOf.HemogenPack)
				{
					for (int index = 0; index < roomCount; ++index)
					{
						if (!SocialProperness.BloodfeedingPrisonerInRoom(adjacentRooms[index]))
						{
							propriety = true;
							return true;
						}
					}

					propriety = false;
					return true;
				}

				for (int index = 0; index < roomCount; ++index)
				{
					if (!adjacentRooms[index].IsPrisonCell)
					{
						propriety = true;
						return true;
					}
				}

				propriety = false;
				return true;
			}

			[HarmonyPatch(
				typeof(SocialProperness),
				nameof(SocialProperness.IsSociallyProper),
				new[] {typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool)}
			)]
			public static class IsSociallyProperTranspiler
			{
				[HarmonyTranspiler]
				static public IEnumerable<CodeInstruction> AllowABreachOfEtiquette (
					IEnumerable<CodeInstruction> theInstructions,
					ILGenerator il
				)
				{
					/* Here we're looking for a piece of code that looks like:
							...
							if (forPrisoner)
							{
								...
								Verse.GridsUtility.GetRoom(intVec, t.Map) == Verse.RegionAndRoomQuery.GetRoom(p)
								...
							}
							...
					   and replacing it with some code that looks like this:
					   		...
							if (forPrisoner)
							{
								...
								Room room;
								  Verse.GridsUtility.GetRoom(intVec, t.Map) == (room = Verse.RegionAndRoomQuery.GetRoom(p))
								? true
								: IsThingSociallyProperForPrisonerInRoom(t, room)
								...
							}
							bool propriety;
							if (IsSociallyProperThingInWallFridge(t, p, ref propriety))
							{
								return propriety;
							}
							...
					*/

					const int forPrisonerArgument = 2;

					using IEnumerator<CodeInstruction> instructions = theInstructions.GetEnumerator();

					MethodInfo getRoomOfThing = typeof(RegionAndRoomQuery).GetMethod(
						nameof(RegionAndRoomQuery.GetRoom),
						new[] {typeof(Thing), typeof(RegionType)}
					);

					uint patchStage = 0;

					CodeInstruction instruction;
				findRelevantInstruction:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.IsLdarg(forPrisonerArgument))
					{
						goto findRelevantInstruction;
					}

					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					if ((instruction.opcode != OpCodes.Brfalse_S) & (instruction.opcode != OpCodes.Brfalse))
					{
						yield return instruction;
						goto findRelevantInstruction;
					}

					Label oldNotForPrisonerTargetLabel = (Label) instruction.operand;
					Label newNotForPrisonerTargetLabel = il.DefineLabel();

					instruction.operand = newNotForPrisonerTargetLabel;

					yield return instruction;
				findGetRoomOfThingCall:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.Calls(getRoomOfThing))
					{
						goto findGetRoomOfThingCall;
					}

					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;
				findCeqAfterGetRoomOfThingCall:
					if (instruction.opcode != OpCodes.Ceq)
					{
						yield return instruction;

						if (instruction.StoresLocal())
						{
							instructions.MoveNext();
							instruction = instructions.Current;

							yield return instruction;

							if (instruction.LoadsLocal())
							{
								instructions.MoveNext();
								instruction = instructions.Current;
								goto findCeqAfterGetRoomOfThingCall;
							}
						}

						goto findRelevantInstruction;
					}

					++patchStage;

					LocalBuilder roomLocal = il.DeclareLocal(typeof(Room));

					yield return CodeInstruction.StoreLocal(roomLocal.LocalIndex);
					yield return CodeInstruction.LoadLocal(roomLocal.LocalIndex);

					yield return instruction;

					Label callIsThingSociallyProperForPrisonerLabel = il.DefineLabel();
					Label useRoomCheckResultLabel = il.DefineLabel();

					yield return new CodeInstruction(OpCodes.Brfalse_S, callIsThingSociallyProperForPrisonerLabel);
					yield return new CodeInstruction(OpCodes.Ldc_I4_1);
					yield return new CodeInstruction(OpCodes.Br_S, useRoomCheckResultLabel);
					yield return new CodeInstruction(OpCodes.Ldarg_0).LabelWith(callIsThingSociallyProperForPrisonerLabel);
					yield return CodeInstruction.LoadLocal(roomLocal.LocalIndex);
					yield return new CodeInstruction(
						OpCodes.Call,
						typeof(HandleTheProprietyOfWallFridges).GetMethod(
							nameof(HandleTheProprietyOfWallFridges.IsThingSociallyProperForPrisonerInRoom)
						)
					);

					instructions.MoveNext();
					instruction = instructions.Current;

					instruction.labels.Add(useRoomCheckResultLabel);

					yield return instruction;
				findNotForPrisonerTarget:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					if (!instruction.labels.Contains(oldNotForPrisonerTargetLabel))
					{
						yield return instruction;

						goto findNotForPrisonerTarget;
					}

					LocalBuilder proprietyLocal = il.DeclareLocal(typeof(bool));

					yield return new CodeInstruction(OpCodes.Ldarg_0).LabelWith(newNotForPrisonerTargetLabel);
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return CodeInstruction.LoadLocal(proprietyLocal.LocalIndex, true);
					yield return new CodeInstruction(
						OpCodes.Call,
						typeof(HandleTheProprietyOfWallFridges).GetMethod(
							nameof(HandleTheProprietyOfWallFridges.IsSociallyProperThingInWallFridge)
						)
					);
					yield return new CodeInstruction(OpCodes.Brfalse_S, oldNotForPrisonerTargetLabel);
					yield return CodeInstruction.LoadLocal(proprietyLocal.LocalIndex);
					yield return new CodeInstruction(OpCodes.Ret);

					yield return instruction;

					goto findRelevantInstruction;
				noMoreInstructions:
					if ((patchStage != 0) & ((patchStage & 2) == 0))
					{
						yield break;
					}

					throw new TranspilerFallbackException("The transpiler patch for `SocialProperness.IsSociallyProper` failed to apply, so we're falling back to a slower postfix patch.");
				}
			}

			[HarmonyPatch(
				typeof(SocialProperness),
				nameof(SocialProperness.IsSociallyProper),
				new[] {typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool)}
			)]
			public static class IsSociallyProperPostfix
			{
				[HarmonyPostfix]
				public static bool AllowABreachOfEtiquette (
					bool isSociallyProper,
					Thing t,
					Pawn p,
					bool forPrisoner
				)
				{
					if (forPrisoner)
					{
						return isSociallyProper || IsThingSociallyProperForPrisonerInRoom(
							t,
							RegionAndRoomQuery.GetRoom(p)
						);
					}

					bool propriety = false;

					if (IsSociallyProperThingInWallFridge(t, p, ref propriety))
					{
						return propriety;
					}

					return isSociallyProper;
				}
			}
		}
	}


	static class PrisonCellChangeTracking
	{
		[HarmonyPatch(
			typeof(Room),
			nameof(Room.Notify_RoomShapeChanged),
			new Type[0]
		)]
		public static class TrackChangeOfPrisonCellStatusForRoom
		{
			[HarmonyPrefix]
			public static void TrackChangeOfPrisonCellStatusPre (Room __instance, ref bool __state)
			{
				__state = __instance.IsPrisonCell;
			}

			[HarmonyPostfix]
			public static void TrackChangeOfPrisonCellStatusPost (Room __instance, bool __state)
			{
				bool isPrisonCell = __instance.IsPrisonCell;

				if (isPrisonCell != __state)
				{
					PrisonCellStateTracking.ReactToChangeOfPrisonCellStatusForRoom(__instance, isPrisonCell);
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

		[HarmonyPatch]
		public static class WorkaroundCommsConsoleStupidity
		{
			[HarmonyTargetMethods]
			static public IEnumerable<MethodBase> FindDelegates ()
			{
				foreach (Type nestedType in typeof(PassingShip).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Static))
				{
					foreach (MethodInfo method in nestedType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
					{
						if (method.ReturnType == typeof(void) && method.GetParameters().Length == 0)
						{
							yield return method;
						}
					}
				}
			}

			[HarmonyTranspiler]
			static public IEnumerable<CodeInstruction> AllowCommsToBeInitiatedWithOnlyFridges (
				IEnumerable<CodeInstruction> instructions,
				MethodBase method
			)
			{
				/* Here we're looking for a piece of code that looks like:
						if (!System.Linq.Enumerable.Any(RimWorld.Building_OrbitalTradeBeacon.AllPowered(this.Map)))
						{
							Verse.Messages.Message(Verse.Translator.Translate("MessageNeedBeaconToTradeWithShip"), console, RimWorld.MessageTypeDefOf.RejectInput, historical: false);
						}
						else
						{
							console.GiveUseCommsJob(negotiator, this);
						}
				   and adding to the if-condition:
						&& !FridgesAllowCommsToBeInitiatedWithPassingShip(this)

				   This code is defined in an anonymous delegate, hence `FindDelegates`
				   and the following code searching for a singular `PassingShip` field.
				*/

				FieldInfo passingShipCapture = null;

				foreach (FieldInfo field in method.DeclaringType.GetFields(BindingFlags.Public | BindingFlags.Instance))
				{
					if (field.FieldType == typeof(PassingShip))
					{
						if (passingShipCapture != null)
						{
							goto irrelevantMethod;
						}

						passingShipCapture = field;
					}
				}

				if (passingShipCapture == null)
				{
					goto irrelevantMethod;
				}

				CodeInstruction previous = null;

				foreach (CodeInstruction instruction in instructions)
				{
					/* This is a very unimportant issue, so I'm not doing anything more complex than this. */
					if (
						   instruction.LoadsConstant("MessageNeedBeaconToTradeWithShip")
						&& previous != null
						&& previous.Branches(out Label? foundBeaconsLabel)
						&& foundBeaconsLabel.HasValue
					)
					{
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Ldfld, passingShipCapture);
						yield return new CodeInstruction(
							OpCodes.Call,
							typeof(WorkaroundCommsConsoleStupidity).GetMethod(
								nameof(WorkaroundCommsConsoleStupidity.FridgesAllowCommsToBeInitiatedWithPassingShip)
							)
						);
						yield return new CodeInstruction(OpCodes.Brtrue, foundBeaconsLabel.Value);
					}

					yield return instruction;

					previous = instruction;
				}

				yield break;
			irrelevantMethod:
				foreach (CodeInstruction instruction in instructions)
				{
					yield return instruction;
				}
			}

			static public bool FridgesAllowCommsToBeInitiatedWithPassingShip (PassingShip passingShip)
			{
				if (!Settings.ActAsBeacon)
				{
					return false;
				}

				Map map = passingShip.Map;

				foreach (RimFridge_Building fridge in FridgeCacheFast.rimFridgeCache[map].Values)
				{
					RimWorld.CompPowerTrader powerComp = fridge.GetComp<RimWorld.CompPowerTrader>();

					if (powerComp == null || powerComp.PowerOn)
					{
						return true;
					}
				}

				return false;
			}
		}
	}


	public static class DisplayStackedItemsNicelyInFridges
	{
		/* This is Verse.Altitudes.LayerSpacing, which is private for whatever reason. */
		public const float altitudeLayerSpacing = 0.36585367f;
		public const float altitudeOfItemInFridge = altitudeLayerSpacing * (float) AltitudeLayer.Item;
		public const float itemInFridgeSpacing = 0.004054054f;
		public const float itemInFridgeSpacingInverse = 1f / itemInFridgeSpacing;
		public const float itemInFridgeLabelSpacing = 17f;

		public static class MungeTrueCenterOfItemsInFridges
		{
			[HarmonyPatch(typeof(GenThing), "ItemCenterAt", new[] {typeof(Thing)})]
			public static class MungeItemCenterTranspiler
			{
				[HarmonyTranspiler]
				static public IEnumerable<CodeInstruction> MungeItemCenter (
					IEnumerable<CodeInstruction> theInstructions,
					ILGenerator il
				)
				{
					/* Here we're looking for a piece of code that looks like:
							...
							IntVec3 position = thing.Position;
							...
							int itemCount = 0;
							...
							int itemsWithLowerID = 0;
							...
							List<Thing> thingList = GridsUtility.GetThingList(position, thing.Map);
							...
							if (itemCount <= 1)
							{
								...
							}
							...
					   and replacing it with some code that looks like this:
							...
							IntVec3 position = thing.Position;
							...
							int itemCount = 0;
							...
							int itemsWithLowerID = 0;
							...
							List<Thing> thingList = GridsUtility.GetThingList(position, thing.Map);
							...
							if (thingList.Count > 1 && FridgeCacheFast.rimFridgeCache[thing.Map].ContainsKey(position))
							{
								float d = (float) itemsWithLowerID;

								return new Vector3(
									(float) position.x + 0.5f,
									altitudeOfItemInFridge + d * itemInFridgeSpacing,
									(float) position.z + 0.45f + d * 0.0625f
								);
							}
							if (itemCount <= 1)
							{
								...
							}
							...
					*/

					using IEnumerator<CodeInstruction> instructions = theInstructions.GetEnumerator();

					MethodInfo getPositionOfThing = typeof(Thing).GetProperty(nameof(Thing.Position)).GetMethod;
					MethodInfo getMapOfThing = typeof(Thing).GetProperty(nameof(Thing.Map)).GetMethod;
					MethodInfo getThingList = typeof(GridsUtility).GetMethod(nameof(GridsUtility.GetThingList), new[] {typeof(IntVec3), typeof(Map)});
					MethodInfo thingListCount = typeof(List<Thing>).GetProperty("Count").GetMethod;
					FieldInfo rimFridgeCacheField = typeof(FridgeCacheFast).GetField(nameof(FridgeCacheFast.rimFridgeCache), BindingFlags.NonPublic | BindingFlags.Static);
					MethodInfo rimFridgeCacheContainsKey = typeof(Dictionary<IntVec3, RimFridge_Building>).GetMethod("ContainsKey");
					MethodInfo rimFridgeCacheByMapGetItem = typeof(Dictionary<Map, Dictionary<IntVec3, RimFridge_Building>>).GetProperty("Item").GetMethod;
					FieldInfo intVec3x = typeof(IntVec3).GetField(nameof(IntVec3.x));
					FieldInfo intVec3z = typeof(IntVec3).GetField(nameof(IntVec3.z));
					ConstructorInfo vector3Ctor = typeof(Vector3).GetConstructor(new[] {typeof(float), typeof(float), typeof(float)});

					uint patchStage = 0;

					CodeInstruction instruction;
					int positionLocalIndex;
					int depthLocalIndex;
					int itemsWithLowerIDCaptureStructLocalIndex;
					FieldInfo itemsWithLowerIDField;
					int thingListLocal;
				findInitialisationOfPosition:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.Calls(getPositionOfThing))
					{
						goto findInitialisationOfPosition;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.StoresLocal(out positionLocalIndex))
					{
						goto findInitialisationOfPosition;
					}

					++patchStage;
				findInitialisationOfDepth:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.LoadsConstant(0))
					{
						goto findInitialisationOfDepth;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.StoresLocal(out depthLocalIndex))
					{
						goto findInitialisationOfDepth;
					}

					++patchStage;
				findInitialisationOfItemsWithLowerID:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.LoadsLocalAddress(out itemsWithLowerIDCaptureStructLocalIndex))
					{
						goto findInitialisationOfItemsWithLowerID;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.LoadsConstant(0))
					{
						goto findInitialisationOfItemsWithLowerID;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					yield return instruction;

					if (instruction.opcode != OpCodes.Stfld)
					{
						goto findInitialisationOfItemsWithLowerID;
					}

					itemsWithLowerIDField = (FieldInfo) instruction.operand;

					++patchStage;
				findInitialisationOfThingList:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.Calls(getThingList))
					{
						goto findInitialisationOfThingList;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.StoresLocal(out thingListLocal))
					{
						goto findInitialisationOfThingList;
					}

					++patchStage;
				findComparisonWithDepth:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					if (!instruction.LoadsLocal(depthLocalIndex))
					{
						yield return instruction;
						goto findComparisonWithDepth;
					}

					CodeInstruction loadDepthLocal = instruction;

					instructions.MoveNext();
					instruction = instructions.Current;

					if (!instruction.LoadsConstant(1))
					{
						yield return loadDepthLocal;
						yield return instruction;
						goto findComparisonWithDepth;
					}

					CodeInstruction load1 = instruction;

					instructions.MoveNext();
					instruction = instructions.Current;

					if ((instruction.opcode != OpCodes.Bgt_S) & (instruction.opcode != OpCodes.Bgt))
					{
						yield return loadDepthLocal;
						yield return load1;
						yield return instruction;
						goto findComparisonWithDepth;
					}

					++patchStage;

					Label notInARimFridgeTarget = il.DefineLabel();
					LocalBuilder floatLocal = il.DeclareLocal(typeof(float));

					yield return CodeInstruction.LoadLocal(thingListLocal).TakeLabelsFrom(loadDepthLocal);
					yield return new CodeInstruction(OpCodes.Call, thingListCount);
					yield return new CodeInstruction(OpCodes.Ldc_I4_1);
					yield return new CodeInstruction(OpCodes.Ble_S, notInARimFridgeTarget);

					yield return new CodeInstruction(OpCodes.Ldsfld, rimFridgeCacheField);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, getMapOfThing);
					yield return new CodeInstruction(OpCodes.Call, rimFridgeCacheByMapGetItem);
					yield return CodeInstruction.LoadLocal(positionLocalIndex);
					yield return new CodeInstruction(OpCodes.Call, rimFridgeCacheContainsKey);

					yield return new CodeInstruction(OpCodes.Brfalse_S, notInARimFridgeTarget);

					yield return CodeInstruction.LoadLocal(itemsWithLowerIDCaptureStructLocalIndex, true);
					yield return new CodeInstruction(OpCodes.Ldfld, itemsWithLowerIDField);
					yield return new CodeInstruction(OpCodes.Conv_R4);
					yield return CodeInstruction.StoreLocal(floatLocal.LocalIndex);

					yield return CodeInstruction.LoadLocal(positionLocalIndex);
					yield return new CodeInstruction(OpCodes.Ldfld, intVec3x);
					yield return new CodeInstruction(OpCodes.Conv_R4);
					yield return new CodeInstruction(OpCodes.Ldc_R4, 0.5f);
					yield return new CodeInstruction(OpCodes.Add);

					yield return new CodeInstruction(OpCodes.Ldc_R4, altitudeOfItemInFridge);
					yield return CodeInstruction.LoadLocal(floatLocal.LocalIndex);
					yield return new CodeInstruction(OpCodes.Ldc_R4, itemInFridgeSpacing);
					yield return new CodeInstruction(OpCodes.Mul);
					yield return new CodeInstruction(OpCodes.Add);

					yield return CodeInstruction.LoadLocal(positionLocalIndex);
					yield return new CodeInstruction(OpCodes.Ldfld, intVec3z);
					yield return new CodeInstruction(OpCodes.Conv_R4);
					yield return new CodeInstruction(OpCodes.Ldc_R4, 0.45f);
					yield return new CodeInstruction(OpCodes.Add);
					yield return CodeInstruction.LoadLocal(floatLocal.LocalIndex);
					yield return new CodeInstruction(OpCodes.Ldc_R4, 0.0625f);
					yield return new CodeInstruction(OpCodes.Mul);
					yield return new CodeInstruction(OpCodes.Add);

					yield return new CodeInstruction(OpCodes.Newobj, vector3Ctor);
					yield return new CodeInstruction(OpCodes.Ret);

					yield return loadDepthLocal.LabelWith(notInARimFridgeTarget);
					yield return load1;
					yield return instruction;
				yieldRestOfCode:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					goto yieldRestOfCode;
				noMoreInstructions:
					if (patchStage == 5)
					{
						yield break;
					}

					throw new TranspilerFallbackException("The transpiler patch for `GenThing.ItemCenterAt` failed to apply, so we're falling back to a slower postfix patch.");
				}
			}

			[HarmonyPatch(typeof(GenThing), nameof(GenThing.TrueCenter), new[] {typeof(Thing)})]
			public static class MungeTrueCenterPostfix
			{
				[HarmonyPostfix]
				public static Vector3 MungeTrueCenter (Vector3 originalValue, Thing t)
				{
					if (t.def.category == ThingCategory.Item && t.Spawned)
					{
						IntVec3 position = t.Position;
						var things = t.Map.thingGrid.ThingsListAtFast(position);

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
								Vector3 v = position.ToVector3Shifted();
								float d = (float) depthInStack;

								return new Vector3(
									v.x,
									altitudeOfItemInFridge + d * itemInFridgeSpacing,
									v.z + d * 0.0625f - 0.05f
								);
							}
						}
					}

					return originalValue;
				}
			}
		}

		public static class MakeTheStackCountLabelsReadable
		{
			[HarmonyPatch(typeof(GenMapUI), nameof(GenMapUI.LabelDrawPosFor), new[] {typeof(Thing), typeof(float)})]
			public static class OffsetTheLabelsTranspiler
			{
				[HarmonyTranspiler]
				static public IEnumerable<CodeInstruction> OffsetTheLabels (
					IEnumerable<CodeInstruction> theInstructions,
					ILGenerator il
				)
				{
					/* Here we're looking for a piece of code that looks like:
							...
							Vector3 drawPos = thing.DrawPos;
							...
							Vector2 result = (implicit cast from Vector3 to Vector2) ...;
							...
							if (thing is Pawn)
							{
								...
							}
							...
					   and replacing it with some code that looks like this:
							...
							Vector3 drawPos = thing.DrawPos;
							...
							Vector2 result = (implicit cast from Vector3 to Vector2) ...;
							...
							if (thing is Pawn)
							{
								...
							}
							else if (FridgeCacheFast.rimFridgeCache[thing.Map].ContainsKey(thing.Position))
							{
								result.x += (
									  ((drawPos.y - altitudeOfItemInFridge) * itemInFridgeSpacingInverse + -1f)
									* itemInFridgeLabelSpacing
								);
								return result;
							}
							...
					*/

					const int thingArgument = 0;

					using IEnumerator<CodeInstruction> instructions = theInstructions.GetEnumerator();

					MethodInfo getDrawPosOfThing = typeof(Thing).GetProperty(nameof(Thing.DrawPos)).GetMethod;
					MethodInfo getPositionOfThing = typeof(Thing).GetProperty(nameof(Thing.Position)).GetMethod;
					MethodInfo getMapOfThing = typeof(Thing).GetProperty(nameof(Thing.Map)).GetMethod;
					FieldInfo rimFridgeCacheField = typeof(FridgeCacheFast).GetField(nameof(FridgeCacheFast.rimFridgeCache), BindingFlags.NonPublic | BindingFlags.Static);
					MethodInfo rimFridgeCacheContainsKey = typeof(Dictionary<IntVec3, RimFridge_Building>).GetMethod("ContainsKey");
					MethodInfo rimFridgeCacheByMapGetItem = typeof(Dictionary<Map, Dictionary<IntVec3, RimFridge_Building>>).GetProperty("Item").GetMethod;
					FieldInfo vector2x = typeof(Vector2).GetField(nameof(Vector2.x));
					FieldInfo vector3y = typeof(Vector3).GetField(nameof(Vector3.y));
					MethodInfo implicitVector3ToVector2 = typeof(Vector2).GetMethod("op_Implicit", new[] {typeof(Vector3)});

					uint patchStage = 0;

					CodeInstruction instruction;
					int drawPosLocalIndex;
					int resultLocalIndex;
				findInitialisationOfDrawPos:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.Calls(getDrawPosOfThing))
					{
						goto findInitialisationOfDrawPos;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.StoresLocal(out drawPosLocalIndex))
					{
						goto findInitialisationOfDrawPos;
					}

					++patchStage;
				findInitialisationOfResult:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.Calls(implicitVector3ToVector2))
					{
						goto findInitialisationOfResult;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.StoresLocal(out resultLocalIndex))
					{
						goto findInitialisationOfResult;
					}

					++patchStage;
				findIsPawnCondition:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.IsLdarg(thingArgument))
					{
						goto findIsPawnCondition;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					yield return instruction;

					if (instruction.opcode != OpCodes.Isinst || (Type) instruction.operand != typeof(Pawn))
					{
						goto findIsPawnCondition;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					if ((instruction.opcode != OpCodes.Brfalse_S) & (instruction.opcode != OpCodes.Brfalse))
					{
						yield return instruction;
						goto findIsPawnCondition;
					}

					Label oldIsNotPawnTargetLabel = (Label) instruction.operand;
					Label newIsNotPawnTargetLabel = il.DefineLabel();

					instruction.operand = newIsNotPawnTargetLabel;

					yield return instruction;

					++patchStage;
				findOldIsNotPawnTarget:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					if (!instruction.labels.Contains(oldIsNotPawnTargetLabel))
					{
						yield return instruction;
						goto findOldIsNotPawnTarget;
					}

					yield return new CodeInstruction(OpCodes.Ldsfld, rimFridgeCacheField).LabelWith(newIsNotPawnTargetLabel);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, getMapOfThing);
					yield return new CodeInstruction(OpCodes.Call, rimFridgeCacheByMapGetItem);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, getPositionOfThing);
					yield return new CodeInstruction(OpCodes.Call, rimFridgeCacheContainsKey);

					yield return new CodeInstruction(OpCodes.Brfalse_S, oldIsNotPawnTargetLabel);

					yield return CodeInstruction.LoadLocal(resultLocalIndex, true);
					yield return new CodeInstruction(OpCodes.Ldflda, vector2x);
					yield return new CodeInstruction(OpCodes.Dup);
					yield return new CodeInstruction(OpCodes.Ldind_R4);
					yield return CodeInstruction.LoadLocal(drawPosLocalIndex, true);
					yield return new CodeInstruction(OpCodes.Ldfld, vector3y);
					yield return new CodeInstruction(OpCodes.Ldc_R4, altitudeOfItemInFridge);
					yield return new CodeInstruction(OpCodes.Sub);
					yield return new CodeInstruction(OpCodes.Ldc_R4, itemInFridgeSpacingInverse);
					yield return new CodeInstruction(OpCodes.Mul);
					yield return new CodeInstruction(OpCodes.Ldc_R4, -1f);
					yield return new CodeInstruction(OpCodes.Add);
					yield return new CodeInstruction(OpCodes.Ldc_R4, itemInFridgeLabelSpacing);
					yield return new CodeInstruction(OpCodes.Mul);
					yield return new CodeInstruction(OpCodes.Add);
					yield return new CodeInstruction(OpCodes.Stind_R4);

					yield return CodeInstruction.LoadLocal(resultLocalIndex);
					yield return new CodeInstruction(OpCodes.Ret);

					yield return instruction;

					++patchStage;
				yieldRestOfCode:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					goto yieldRestOfCode;
				noMoreInstructions:
					if (patchStage == 4)
					{
						yield break;
					}

					throw new TranspilerFallbackException("The transpiler patch for `GenMapUI.LabelDrawPosFor` failed to apply, so we're falling back to a very-slightly-slower transpiler and postfix patch.");
				}
			}

			[HarmonyPatch(typeof(GenMapUI), nameof(GenMapUI.LabelDrawPosFor), new[] {typeof(Thing), typeof(float)})]
			public static class OffsetTheLabelsFallback
			{
				#pragma warning disable 0649
				[ThreadStatic]
				internal static float drawPosYOfThing;
				#pragma warning restore 0649

				[HarmonyTranspiler]
				static public IEnumerable<CodeInstruction> CacheDrawPosY (
					IEnumerable<CodeInstruction> theInstructions,
					ILGenerator il
				)
				{
					/* Here we're looking for a piece of code that looks  like:
							...
							Vector3 drawPos = thing.DrawPos;
							...
					   and replacing it with some code that looks like this:
							...
							Vector3 drawPos = thing.DrawPos;
							drawPosYOfThing = drawPos.y;
							...
					*/

					using IEnumerator<CodeInstruction> instructions = theInstructions.GetEnumerator();

					MethodInfo getDrawPosOfThing = typeof(Thing).GetProperty(nameof(Thing.DrawPos)).GetMethod;
					FieldInfo vector3y = typeof(Vector3).GetField(nameof(Vector3.y));
					FieldInfo drawPosYOfThingField = typeof(OffsetTheLabelsFallback).GetField("drawPosYOfThing", BindingFlags.NonPublic | BindingFlags.Static);

					uint patchStage = 0;

					CodeInstruction instruction;
					int drawPosLocalIndex;
				findInitialisationOfDrawPos:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.Calls(getDrawPosOfThing))
					{
						goto findInitialisationOfDrawPos;
					}

					instructions.MoveNext();
					instruction = instructions.Current;

					yield return instruction;

					if (!instruction.StoresLocal(out drawPosLocalIndex))
					{
						goto findInitialisationOfDrawPos;
					}

					yield return CodeInstruction.LoadLocal(drawPosLocalIndex, true);
					yield return new CodeInstruction(OpCodes.Ldfld, vector3y);
					yield return new CodeInstruction(OpCodes.Stsfld, drawPosYOfThingField);

					++patchStage;
				yieldRestOfCode:
					if (!instructions.MoveNext()) goto noMoreInstructions;
					instruction = instructions.Current;

					yield return instruction;

					goto yieldRestOfCode;
				noMoreInstructions:
					if (patchStage == 1)
					{
						yield break;
					}

					throw new TranspilerFallbackException("The fallback transpiler patch for `GenMapUI.LabelDrawPosFor` failed to apply, so we're falling back to a slower postfix patch.");
				}

				[HarmonyPostfix]
				public static Vector2 OffsetTheLabels (Vector2 originalValue, Thing thing)
				{
					if (!(thing is Pawn) && FridgeCacheFast.rimFridgeCache[thing.Map].ContainsKey(thing.Position))
					{
						originalValue.x += (
							  ((drawPosYOfThing - altitudeOfItemInFridge) * itemInFridgeSpacingInverse + -1f)
							* itemInFridgeLabelSpacing
						);
					}

					return originalValue;
				}
			}

			[HarmonyPatch(typeof(GenMapUI), nameof(GenMapUI.LabelDrawPosFor), new[] {typeof(Thing), typeof(float)})]
			public static class OffsetTheLabelsSlowPostfix
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
								originalValue.x += (float) depthInStack * itemInFridgeLabelSpacing;
							}
						}
					}

					return originalValue;
				}
			}
		}
	}


	public static class HandleDeathPallsProperlyForCorpsesInWallFridges
	{
		[HarmonyPatch(
			typeof(MutantUtility),
			nameof(MutantUtility.CanResurrectAsShambler),
			new[] {typeof(Corpse), typeof(bool)}
		)]
		public static class TreatWallFridgesAsIndoorsIfAppropriate
		{
			[HarmonyPostfix]
			public static bool TreatWallFridgesAsIndoors (
				bool canResurrectAsShambler,
				Corpse corpse,
				bool ignoreIndoors
			)
			{
				if (!canResurrectAsShambler | ignoreIndoors)
				{
					return canResurrectAsShambler;
				}

				if (!FridgeCacheFast.wallFridgeCache[corpse.MapHeld].TryGetValue(corpse.PositionHeld, out RimFridge_WallBuilding wallFridge))
				{
					return true;
				}

				Room[] adjacentRooms = wallFridge.rooms;
				int roomCount = adjacentRooms.Length;

				for (int index = 0; index < roomCount; ++index)
				{
					Room room = adjacentRooms[index];

					if (!room.ProperRoom && !room.IsDoorway)
					{
						return true;
					}
				}

				return false;
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

