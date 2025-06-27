using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimFridge
{
	public class RimFridge_Building : Building_Storage, ICompRefrigeratorParent, IRenameable
	{
		public Room[] rooms;

		public RimFridge_Building () : base()
		{}

		internal uint packedState;
		internal int maximumItemsPerCell;
		public string fridgeLabel;

		internal static class Flags
		{
			internal const uint itemsRequirePathEndModeOfTouch = 1 << 0;
			internal const uint forbidsUsageByAnimals = 1 << 1;

			internal const uint persistedFlagsMask = forbidsUsageByAnimals;
		}

		public bool ForbidsUsageByAnimals
		{
			get => (this.packedState & Flags.forbidsUsageByAnimals) != 0;
			set
			{
				this.packedState &= ~Flags.forbidsUsageByAnimals;
				this.packedState |= value ? Flags.forbidsUsageByAnimals : 0;
			}
		}

		public override int MaxItemsInCell
		{
			get => this.maximumItemsPerCell;
		}

		public string RenamableLabel
		{
			get => this.fridgeLabel ?? this.BaseLabel;
			set => this.fridgeLabel = value;
		}

		public string BaseLabel
		{
			get => this.Label;
		}

		public string InspectLabel
		{
			get => this.RenamableLabel;
		}

		public override IEnumerable<Gizmo> GetGizmos ()
		{
			foreach (var gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}

			yield return new Command_Action
			{
				action = this.ShowRenameFridgeDialog,
				defaultLabel = "Rename".Translate(),
				defaultDesc = "RimFridge.RenameTheRefrigerator".Translate(),
				hotKey = KeyBindingDefOf.Misc1,
				icon = TexButton.Rename
			};

			if (this.maximumItemsPerCell > 0)
			{
				yield return new Command_Action
				{
					action = () =>
					{
						SoundStarter.PlayOneShotOnCamera(SoundDefOf.DragSlider);
						this.LowerMaxItemsInCell();
					},
					defaultLabel = "RimFridge.LowerMaxItemsPerCell".Translate(),
					defaultDesc = "RimFridge.LowerMaxItemsPerCellDescription".Translate(),
					icon = TexButton.Minus
				};
			}

			if (this.maximumItemsPerCell < this.def.building.maxItemsInCell)
			{
				yield return new Command_Action
				{
					action = () =>
					{
						SoundStarter.PlayOneShotOnCamera(SoundDefOf.DragSlider);
						this.RaiseMaxItemsInCell();
					},
					defaultLabel = "RimFridge.RaiseMaxItemsPerCell".Translate(),
					defaultDesc = "RimFridge.RaiseMaxItemsPerCellDescription".Translate(),
					icon = TexButton.Plus
				};
			}

			if (this.ForbidsUsageByAnimals)
			{
				yield return new Command_Action
				{
					action = () =>
					{
						SoundStarter.PlayOneShotOnCamera(SoundDefOf.TinyBell);
						this.PermitUsageByAnimals();
					},
					defaultLabel = "RimFridge.PermitUsageByAnimals".Translate(),
					defaultDesc = "RimFridge.PermitUsageByAnimalsDescription".Translate(),
					icon = TexButton.Ingest
				};
			}
			else
			{
				yield return new Command_Action
				{
					action = () =>
					{
						SoundStarter.PlayOneShotOnCamera(SoundDefOf.Crunch);
						this.ForbidUsageByAnimals();
					},
					defaultLabel = "RimFridge.ForbidUsageByAnimals".Translate(),
					defaultDesc = "RimFridge.ForbidUsageByAnimalsDescription".Translate(),
					icon = TexButton.LockNorthUp
				};
			}
		}

		public void ShowRenameFridgeDialog ()
		{
			Find.WindowStack.Add(new Dialog_RenameFridge(this));
		}

		public void TogglePermissionForUsageByAnimals ()
		{
			this.ForbidsUsageByAnimals = !this.ForbidsUsageByAnimals;
		}

		public void ForbidUsageByAnimals ()
		{
			this.ForbidsUsageByAnimals = true;
		}

		public void PermitUsageByAnimals ()
		{
			this.ForbidsUsageByAnimals = false;
		}

		public void LowerMaxItemsInCell ()
		{
			this.LowerMaxItemsInCellBy(1);
		}

		public int LowerMaxItemsInCellBy (byte delta)
		{
			int oldItemsPerCell = this.maximumItemsPerCell;
			int itemsPerCell = delta < oldItemsPerCell ? oldItemsPerCell - delta : 0;

			int greatestItemCountInCell = 0;
			Map map = this.Map;

			foreach (IntVec3 cell in this.OccupiedRect())
			{
				int itemCount = cell.GetItemCount(map);
				greatestItemCountInCell = itemCount > greatestItemCountInCell ? itemCount : greatestItemCountInCell;
			}

			if (greatestItemCountInCell > itemsPerCell)
			{
				itemsPerCell = greatestItemCountInCell;

				Verse.Messages.Message(
					"RimFridge.MaxItemsInCellCannotBeLowerThanGreatestItemCount".Translate(greatestItemCountInCell),
					this,
					RimWorld.MessageTypeDefOf.RejectInput,
					historical: false
				);
			}

			this.maximumItemsPerCell = itemsPerCell;

			return oldItemsPerCell - itemsPerCell;
		}

		public void RaiseMaxItemsInCell ()
		{
			this.RaiseMaxItemsInCellBy(1);
		}

		public int RaiseMaxItemsInCellBy (byte delta)
		{
			int oldItemsPerCell = this.maximumItemsPerCell;
			int itemsPerCell = oldItemsPerCell + delta;

			if (itemsPerCell > this.def.building.maxItemsInCell)
			{
				itemsPerCell = this.def.building.maxItemsInCell;

				Verse.Messages.Message(
					"RimFridge.MaxItemsInCellCannotBeGreaterThanBuildingLimit".Translate(this.def.building.maxItemsInCell),
					this,
					RimWorld.MessageTypeDefOf.RejectInput,
					historical: false
				);
			}

			this.maximumItemsPerCell = itemsPerCell;

			return itemsPerCell - oldItemsPerCell;
		}

		public override void PostMake ()
		{
			this.maximumItemsPerCell = this.DefaultMaximumItemsPerCellForNewFridge;
			base.PostMake();
		}

		public int DefaultMaximumItemsPerCellForNewFridge
		{
			get
			{
				int maxItemsPerCell = Settings.defaultMaximumItemsPerCell;
				return (
					  maxItemsPerCell <= this.def.building.maxItemsInCell
					? maxItemsPerCell
					: this.def.building.maxItemsInCell
				);
			}
		}

		public int DefaultMaximumItemsPerCellForExistingFridge
		{
			get
			{
				int maximum = this.DefaultMaximumItemsPerCellForNewFridge;
				Map map = this.Map;

				foreach (IntVec3 cell in this.OccupiedRect())
				{
					int itemCount = cell.GetItemCount(map);
					maximum = itemCount > maximum ? itemCount : maximum;
				}

				return maximum;
			}
		}

		public override string GetInspectString ()
		{
			return new StringBuilder(base.GetInspectString()).AppendLine().Append(
				"RimFridge.MaxItemsPerCell".Translate(this.MaxItemsInCell)
			).ToString();
		}

		public override void ExposeData ()
		{
			base.ExposeData();

			if (Scribe.mode == LoadSaveMode.Saving)
			{
				uint packedState = this.packedState & Flags.persistedFlagsMask;
				Scribe_Values.Look(ref packedState, "packedState");
			}
			else if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				uint packedState = 0;
				Scribe_Values.Look(ref packedState, "packedState");
				this.packedState |= packedState & Flags.persistedFlagsMask;
			}

			Scribe_Values.Look(ref this.maximumItemsPerCell, "maxItemsPerCell", 0x7FFFFFFF);

			/* Versions older than 1.2.0 of RimFridge handled the fridge label
			   via the CompRefrigerator comp, rather than via the this building.
			   So, if that comp was previously renamed that name will be assigned
			   to its parent RimFridge during the LoadingVars stage. Thus, during the
			   LoadingVars we load the fridgeLabel only if the fridgeLabel is null,
			   so as not clobber the name migrated from the comp. */
			if (Scribe.mode != LoadSaveMode.LoadingVars || this.fridgeLabel == null)
			{
				Scribe_Values.Look(ref this.fridgeLabel, "fridgeLabel");
			}
		}

		public override void PostMapInit ()
		{
			base.PostMapInit();

			/* I'm not happy about doing this here, but we can't do it
			   in `ExposeData` as the map's `ThingList`s
			   have yet to be initialised when those methods are called. */
			if (this.maximumItemsPerCell == 0x7FFFFFFF)
			{
				this.maximumItemsPerCell = this.DefaultMaximumItemsPerCellForExistingFridge;
			}
		}

		public override void SpawnSetup (Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);

			FridgeCacheFast.AddToCache(FridgeCacheFast.rimFridgeCache[map], this, GenAdj.OccupiedRect(this));

			this.ReactToChangeOfRegionsAndRooms();
		}

		public override void DeSpawn (DestroyMode mode)
		{
			base.DeSpawn(mode);

			this.rooms = null;

			FridgeCacheFast.RemoveFromCache(FridgeCacheFast.rimFridgeCache[this.Map], GenAdj.OccupiedRect(this));
		}

		public virtual float GetTemperatureOfSurroundings (CompRefrigerator comp)
		{
			return this.rooms[0].Temperature;
		}

		public virtual void PushTransferredAndGeneratedHeat (float energy, CompRefrigerator comp)
		{
			this.rooms[0].PushHeat(energy);
		}

		public virtual void ReactToChangeOfRegionsAndRooms ()
		{
			if (!this.Spawned)
			{
				this.rooms = new Room[0];
				return;
			}

			Room[] rooms = new Room[1];
			rooms[0] = this.Position.GetRoom(this.Map);
			this.rooms = rooms;
		}
	}

	public abstract class RimFridge_WallBuilding : RimFridge_Building
	{
		public RimFridge_WallBuilding () : base()
		{
			this.packedState |= Flags.itemsRequirePathEndModeOfTouch;
		}

		public override void ReactToChangeOfRegionsAndRooms ()
		{
			if (!this.Spawned)
			{
				this.rooms = new Room[0];
				return;
			}

			Region[] possibleRegions = this.GatherAdjacentRegions();

			int possibleCount = possibleRegions.Length;
			int uniqueCount = 0;

			for (int regionIndex = 0; regionIndex < possibleCount; ++regionIndex)
			{
				Region region = possibleRegions[regionIndex];

				if (region == null)
				{
					goto handledRegion;
				}

				for (int uniqueIndex = 0; uniqueIndex < uniqueCount; ++uniqueIndex)
				{
					if (possibleRegions[uniqueIndex] == region)
					{
						goto handledRegion;
					}
				}

				possibleRegions[uniqueCount++] = region;
			handledRegion: {}
			}

			Region[] regions = new Region[uniqueCount];

			for (int index = 0; index < uniqueCount; ++index)
			{
				regions[index] = possibleRegions[index];
			}

			Room[] rooms = new Room[uniqueCount];
			int roomCount = 0;

			for (int regionIndex = 0; regionIndex < uniqueCount; ++regionIndex)
			{
				Room room = regions[regionIndex].Room;

				if (room == null)
				{
					goto handledRoom;
				}

				for (int roomIndex = 0; roomIndex < roomCount; ++roomIndex)
				{
					if (rooms[roomIndex] == room)
					{
						goto handledRoom;
					}
				}

				rooms[roomCount++] = room;
			handledRoom: {}
			}

			Array.Resize(ref rooms, roomCount);

			this.rooms = rooms;
		}

		public abstract Region[] GatherAdjacentRegions ();

		public override void SpawnSetup (Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);

			FridgeCacheFast.AddToCache(FridgeCacheFast.wallFridgeCache[map], this, GenAdj.OccupiedRect(this));
		}

		public override void DeSpawn (DestroyMode mode)
		{
			base.DeSpawn(mode);

			FridgeCacheFast.RemoveFromCache(FridgeCacheFast.wallFridgeCache[this.Map], GenAdj.OccupiedRect(this));
		}

		public override float GetTemperatureOfSurroundings (CompRefrigerator comp)
		{
			float totalTemperature = 0f;

			Room[] rooms = this.rooms;
			int roomCount = rooms.Length;

			if (roomCount == 0)
			{
				goto slowAndSillyPath;
			}

			for (int index = 0; index < roomCount; ++index)
			{
				totalTemperature += rooms[index].Temperature;
			}

			return totalTemperature / (float) roomCount;
		slowAndSillyPath:
			RoofDef roof = this.Position.GetRoof(this.Map);

			if (roof == null)
			{
				return this.Map.mapTemperature.OutdoorTemp;
			}

			return comp.currentTemp;
		}

		public override void PushTransferredAndGeneratedHeat (float energy, CompRefrigerator comp)
		{
			Room[] rooms = this.rooms;
			int roomCount = rooms.Length;

			if (roomCount == 0)
			{
				goto slowAndSillyPath;
			}

			float energyPerRoom = energy / (float) roomCount;

			for (int index = 0; index < roomCount; ++index)
			{
				rooms[index].PushHeat(energyPerRoom);
			}

			return;
		slowAndSillyPath:
			RoofDef roof = this.Position.GetRoof(this.Map);

			if (roof == null)
			{
				return;
			}

			comp.currentTemp += energy + 1f;
		}
	}

	public class RimFridge_DoubleSidedWallBuilding : RimFridge_WallBuilding, IPathFindCostProvider
	{
		public static ushort prisonCellSideAvoidancePathFindCost;

		internal CellRect pathFindCostCellRect;

		/* If this double-sided wall-fridge is between
		   a room that is a prison-cell and a room that is not a prison-cell
		   this will be a reference to the prison-cell, otherwise it will be null.
		   If prisonCellSideAvoidancePathFindCost is zero this will be null
		   as it does not matter. */
		internal Room prisonCellSideToAvoid;

		public RimFridge_DoubleSidedWallBuilding () : base()
		{}

		public ushort PathFindCostFor (Pawn pawn)
		{
			/* This is the fast-path. */
			if (this.prisonCellSideToAvoid == null)
			{
				return 0;
			}

			/* This is the very-slightly-less-fast-path. */
			if (pawn.jobs?.curDriver == null)
			{
				return 0;
			}

			if (
				/* If they have no faction they don't care. */
				   pawn.Faction is not {} faction
				/* If they don't belong to the player's faction they don't care. */
				|| !faction.IsPlayer
				/* If they're not free they don't care. */
				|| pawn.HostFaction != null
				/* If they're a slave they don't care. */
				|| pawn.IsSlave
				/* If they're already in the prison-cell there's no call for discouraging them from entering it. */
				|| this.Map.regionGrid.GetValidRegionAt(pawn.Position).Room == this.prisonCellSideToAvoid
			)
			{
	  			return 0;
			}

			return prisonCellSideAvoidancePathFindCost;
		}

		/* This is for IPathFindCostProvider, which we use to discourage pawns
		   from using the socially-improper side if desired,
		   hence why the occupied-rect extends past the fridge. */
		public CellRect GetOccupiedRect ()
		{
			return this.pathFindCostCellRect;
		}

		public override void ReactToChangeOfRegionsAndRooms ()
		{
			base.ReactToChangeOfRegionsAndRooms();
			this.RectifyPrisonCellSideToAvoidStatus();
		}

		public void ReactToChangeOfPrisonCellStatusForRoom ()
		{
			this.RectifyPrisonCellSideToAvoidStatus();
		}

		public override void SpawnSetup (Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);

			FridgeCacheFast.AddToCache(FridgeCacheFast.doubleSidedCache[map], this, GenAdj.OccupiedRect(this));
		}

		public override void DeSpawn (DestroyMode mode)
		{
			base.DeSpawn(mode);

			FridgeCacheFast.RemoveFromCache(FridgeCacheFast.doubleSidedCache[this.Map], GenAdj.OccupiedRect(this));
		}

		public override Region[] GatherAdjacentRegions ()
		{
			Region[] regions;

			int sizeX = this.def.size.x - 1;

			IntVec3 cell = this.Position;
			Map map = this.Map;
			int rotation = this.Rotation.AsInt;

			if (sizeX == 0)
			{
				IntVec3 oppositeCell = cell;

				if ((rotation & 1) == 0)
				{
				/* North or south. */
					cell.z += rotation == 0 ? +1 : -1;
					oppositeCell.z += rotation == 0 ? -1 : +1;
				}
				else
				{
				/* East or west. */
					cell.x += rotation == 1 ? +1 : -1;
					oppositeCell.x += rotation == 1 ? -1 : +1;
				}

				regions = new Region[2];
				regions[0] = map.regionGrid.GetValidRegionAt(cell);
				regions[1] = map.regionGrid.GetValidRegionAt(oppositeCell);
			}
			else
			{
				IntVec3 oppositeCell = cell;
				IntVec3 adjacentCell;
				IntVec3 diagonalCell;

				if ((rotation & 1) == 0)
				{
				/* North or south. */
					cell.z += rotation == 0 ? +1 : -1;
					oppositeCell.z += rotation == 0 ? -1 : +1;
					adjacentCell = cell;
					adjacentCell.x += rotation == 0 ? +1 : -1;
					diagonalCell = oppositeCell;
					diagonalCell.x += rotation == 0 ? +1 : -1;
				}
				else
				{
				/* East or west. */
					cell.x += rotation == 1 ? +1 : -1;
					oppositeCell.x += rotation == 1 ? -1 : +1;
					adjacentCell = cell;
					adjacentCell.z += rotation == 1 ? -1 : +1;
					diagonalCell = oppositeCell;
					diagonalCell.z += rotation == 1 ? -1 : +1;
				}

				regions = new Region[4];
				regions[0] = map.regionGrid.GetValidRegionAt(cell);
				regions[1] = map.regionGrid.GetValidRegionAt(adjacentCell);
				regions[2] = map.regionGrid.GetValidRegionAt(oppositeCell);
				regions[3] = map.regionGrid.GetValidRegionAt(diagonalCell);
			}

			return regions;
		}

		public void RectifyPrisonCellSideToAvoidStatus ()
		{
			if (prisonCellSideAvoidancePathFindCost == 0)
			{
				goto noPrisonCellSideToAvoid;
			}

			Room[] adjacentRooms = this.rooms;
			int roomCount = adjacentRooms.Length;
			Room prisonCell = null;
			int state = 0;

			for (int index = 0; index < roomCount; ++index)
			{
				Room room = adjacentRooms[index];
				int isPrisonCell = room.IsPrisonCell ? 1 : 0;
				prisonCell = isPrisonCell != 0 ? room : prisonCell;
				state |= 1 << isPrisonCell;
			}

			if (state == 0b11)
			{
				this.prisonCellSideToAvoid = prisonCell;

				int sizeX = this.def.size.x - 1;

				IntVec3 cell = this.Position;
				Map map = this.Map;
				IntVec3 adjacentCell;
				int rotation = this.Rotation.AsInt;

				if ((rotation & 1) == 0)
				{
				/* South or north. */
					--cell.z;
					/* If this is a south-facing 2x1 fridge, decrement x by 1.  */
					cell.x -= (rotation == 2 ? 1 : 0) & sizeX;

					if (cell.GetRoom(map) == prisonCell)
					{
						goto prisonCellIsSouth;
					}
					else if (sizeX != 0)
					{
						adjacentCell = cell;
						++adjacentCell.x;

						if (adjacentCell.GetRoom(map) == prisonCell)
						{
							goto prisonCellIsSouth;
						}
					}

					cell.z += 2;
				prisonCellIsSouth:
					this.pathFindCostCellRect.minX = cell.x - 1;
					this.pathFindCostCellRect.maxX = cell.x + sizeX + 1;
					this.pathFindCostCellRect.minZ = cell.z;
					this.pathFindCostCellRect.maxZ = cell.z;
				}
				else
				{
				/* East or west. */
					--cell.x;
					/* If this is a east-facing 2x1 fridge, decrement z by 1.  */
					cell.z -= (rotation == 1 ? 1 : 0) & sizeX;

					if (cell.GetRoom(map) == prisonCell)
					{
						goto prisonCellIsWest;
					}
					else if (sizeX != 0)
					{
						adjacentCell = cell;
						++adjacentCell.z;

						if (adjacentCell.GetRoom(map) == prisonCell)
						{
							goto prisonCellIsWest;
						}
					}

					cell.x += 2;
				prisonCellIsWest:
					this.pathFindCostCellRect.minX = cell.x;
					this.pathFindCostCellRect.maxX = cell.x;
					this.pathFindCostCellRect.minZ = cell.z - 1;
					this.pathFindCostCellRect.maxZ = cell.z + sizeX + 1;
				}

				return;
			}
		noPrisonCellSideToAvoid:
			this.prisonCellSideToAvoid = null;
			this.pathFindCostCellRect.minX = 0;
			this.pathFindCostCellRect.minZ = 0;
			this.pathFindCostCellRect.maxX = 0;
			this.pathFindCostCellRect.maxZ = 0;
		}
	}
}

