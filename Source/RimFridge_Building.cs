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

			FridgeCacheFast.AddToCache(
				FridgeCacheFast.rimFridgeCache[map],
				FridgeCacheFast.rimFridgeList[map],
				this,
				GenAdj.OccupiedRect(this)
			);

			this.ReactToChangeOfRegionsAndRooms();
		}

		public override void DeSpawn (DestroyMode mode)
		{
			FridgeCacheFast.RemoveFromCache(
				FridgeCacheFast.rimFridgeCache[this.Map],
				FridgeCacheFast.rimFridgeList[this.Map],
				this,
				GenAdj.OccupiedRect(this)
			);

			this.rooms = null;

			base.DeSpawn(mode);
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

			this.rooms = GatherRooms(this.Position, this.Map);
		}

		public static Room[] GatherRooms (IntVec3 cell, Map map)
		{
			Room[] rooms = new Room[1];
			rooms[0] = cell.GetRoom(map);
			return rooms;
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

			this.rooms = GatherRooms(this.GatherAdjacentRegions());
		}

		public static Room[] GatherRooms (Region[] possibleRegions)
		{
			int possibleCount = possibleRegions.Length;
			int uniqueCount = 0;

			Region[] regionsScratch = new Region[6];

			for (int regionIndex = 0; regionIndex < possibleCount; ++regionIndex)
			{
				Region region = possibleRegions[regionIndex];

				if (region == null)
				{
					goto handledRegion;
				}

				for (int uniqueIndex = 0; uniqueIndex < uniqueCount; ++uniqueIndex)
				{
					if (regionsScratch[uniqueIndex] == region)
					{
						goto handledRegion;
					}
				}

				regionsScratch[uniqueCount++] = region;
			handledRegion: {}
			}

			Region[] regions = new Region[uniqueCount];

			for (int index = 0; index < uniqueCount; ++index)
			{
				regions[index] = regionsScratch[index];
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

			return rooms;
		}

		public abstract Region[] GatherAdjacentRegions ();

		public override void SpawnSetup (Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);

			FridgeCacheFast.AddToCache(
				FridgeCacheFast.wallFridgeCache[map],
				FridgeCacheFast.wallFridgeList[map],
				this,
				GenAdj.OccupiedRect(this)
			);
		}

		public override void DeSpawn (DestroyMode mode)
		{
			FridgeCacheFast.RemoveFromCache(
				FridgeCacheFast.wallFridgeCache[this.Map],
				FridgeCacheFast.wallFridgeList[this.Map],
				this,
				GenAdj.OccupiedRect(this)
			);

			base.DeSpawn(mode);
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

	public class RimFridge_MultiSidedWallBuilding : RimFridge_WallBuilding
	{
		public static ushort prisonCellSideAvoidancePathFindCost;

		internal int[] pathFindCostCells;

		/* If this multi-sided wall-fridge is between
		   prison-cells and non-prison-cells
		   this will be an array of the prison-cells otherwise it will be null.
		   If prisonCellSideAvoidancePathFindCost is zero this will be null
		   as it does not matter. */
		internal Room[] prisonCellSidesToAvoid;

		public RimFridge_MultiSidedWallBuilding () : base()
		{}

		public override void ReactToChangeOfRegionsAndRooms ()
		{
			if (!this.Spawned)
			{
				this.rooms = new Room[0];
				return;
			}

			Region[] regions = this.GatherAdjacentRegions();
			this.rooms = GatherRooms(regions);
			this.RectifyPrisonCellSidesToAvoidStatus(regions);
		}

		public void ReactToChangeOfPrisonCellStatusForRoom ()
		{
			if (!this.Spawned)
			{
				this.prisonCellSidesToAvoid = null;
				this.pathFindCostCells = null;
				return;
			}

			Region[] regions = prisonCellSideAvoidancePathFindCost != 0 ? this.GatherAdjacentRegions() : null;
			this.RectifyPrisonCellSidesToAvoidStatus(regions);
		}

		public override void SpawnSetup (Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);

			FridgeCacheFast.AddToCache(
				FridgeCacheFast.multiSidedCache[map],
				FridgeCacheFast.multiSidedList[map],
				this,
				GenAdj.OccupiedRect(this)
			);
		}

		public override void DeSpawn (DestroyMode mode)
		{
			FridgeCacheFast.RemoveFromCache(
				FridgeCacheFast.multiSidedCache[this.Map],
				FridgeCacheFast.multiSidedList[this.Map],
				this,
				GenAdj.OccupiedRect(this)
			);

			base.DeSpawn(mode);
		}

		public override Region[] GatherAdjacentRegions ()
		{
			return GatherAdjacentRegions(this.Position, this.Map, this.Rotation, this.def);
		}

		public static Region[] GatherAdjacentRegions (IntVec3 cell, Map map, Rot4 rotatedBy, ThingDef def)
		{
			Region[] regions;
			Region[] regionGrid = map.regionGrid.DirectGrid;

			/* Here we very deliberately query the region-grid in row-major order
			   for a better memory-access pattern. */

			/* Be aware that we depend on the positions of the cells within the
			   returned array in `RectifyPrisonCellSidesToAvoidStatus`. */

			int sizeX = map.Size.x;
			int cellIndex;

			if (def.size.x == 1)
			{
				regions = new Region[4];
				/*
					 D
					B C
					 A
				*/
				/* A */ regions[0] = regionGrid[cellIndex = (cell.z - 1) * sizeX + cell.x];
				/* B */ regions[1] = regionGrid[cellIndex += sizeX - 1];
				/* C */ regions[2] = regionGrid[cellIndex += 2];
				/* D */ regions[3] = regionGrid[cellIndex += sizeX - 1];
			}
			else
			{
				regions = new Region[6];
				int rotation = rotatedBy.AsInt;

				if ((rotation & 1) == 0)
				{
				/* North or south. */
					/*
						 EF
						C  D
						 AB
					*/
					/* A */ regions[0] = regionGrid[cellIndex = (cell.z - 1) * sizeX + cell.x - (rotation >> 1)];
					/* B */ regions[1] = regionGrid[++cellIndex];
					/* C */ regions[2] = regionGrid[cellIndex += sizeX - 2];
					/* D */ regions[3] = regionGrid[cellIndex += 3];
					/* E */ regions[4] = regionGrid[cellIndex += sizeX - 2];
					/* F */ regions[5] = regionGrid[++cellIndex];
				}
				else
				{
				/* East or west. */
					/*
						 F
						E D
						B C
						 A
					*/
					/* A */ regions[0] = regionGrid[cellIndex = (cell.z - (rotation == 1 ? 2 : 1)) * sizeX + cell.x];
					/* B */ regions[1] = regionGrid[cellIndex += sizeX - 1];
					/* C */ regions[2] = regionGrid[cellIndex += 2];
					/* D */ regions[3] = regionGrid[cellIndex += sizeX];
					/* E */ regions[4] = regionGrid[cellIndex -= 2];
					/* F */ regions[5] = regionGrid[cellIndex += sizeX + 1];
				}
			}

			return regions;
		}

		public void RectifyPrisonCellSidesToAvoidStatus (Region[] adjacentRegions)
		{
			if (prisonCellSideAvoidancePathFindCost == 0)
			{
				this.prisonCellSidesToAvoid = null;
				this.pathFindCostCells = null;
				return;
			}

			byte roomMask = 0;
			byte prisonCellMask = 0;

			int regionCount = adjacentRegions.Length;
			Region region;

			for (int index = 0; index < regionCount; ++index)
			{
				region = adjacentRegions[index];

				if (region != null && region.valid)
				{
					prisonCellMask |= (byte) ((region.Room.IsPrisonCell ? 1 : 0) << index);
					roomMask |= (byte) (1 << index);
				}
			}

			if ((prisonCellMask == 0) | (prisonCellMask == roomMask))
			{
				this.prisonCellSidesToAvoid = null;
				this.pathFindCostCells = null;
				return;
			}

			Room[] prisonCells = new Room[6];
			int uniquePrisonCellCount = 0;

			for (int regionIndex = 0; regionIndex < regionCount; ++regionIndex)
			{
				if ((prisonCellMask & (1 << regionIndex)) != 0)
				{
					Room room = adjacentRegions[regionIndex].Room;

					for (int roomIndex = 0; roomIndex < uniquePrisonCellCount; ++roomIndex)
					{
						if (prisonCells[roomIndex] == room)
						{
							goto prisonCellIsInArray;
						}
					}

					prisonCells[uniquePrisonCellCount++] = room;
				}
			prisonCellIsInArray: {}
			}

			Room[] uniquePrisonCells = new Room[uniquePrisonCellCount];

			for (int index = 0; index < uniquePrisonCellCount; ++index)
			{
				uniquePrisonCells[index] = prisonCells[index];
			}

			this.prisonCellSidesToAvoid = uniquePrisonCells;

			int[] cells = new int[10];
			int cellCount = 0;

			IntVec3 cell = this.Position;
			int sizeX = this.Map.Size.x;
			int cellIndex;

			if (this.def.size.x == 1)
			{
				/*
					 D
					B C
					 A
				*/
				cellIndex = (cell.z - 1) * sizeX + cell.x - 1;
				if ((prisonCellMask & 0b0011) != 0) {cells[cellCount++] = cellIndex;}
				if ((prisonCellMask & 0b0001) != 0) {cells[cellCount++] = cellIndex + 1;}
				if ((prisonCellMask & 0b0101) != 0) {cells[cellCount++] = cellIndex + 2;}
				cellIndex += sizeX;
				if ((prisonCellMask & 0b0010) != 0) {cells[cellCount++] = cellIndex;}
				if ((prisonCellMask & 0b0100) != 0) {cells[cellCount++] = cellIndex + 2;}
				cellIndex += sizeX;
				if ((prisonCellMask & 0b1010) != 0) {cells[cellCount++] = cellIndex;}
				if ((prisonCellMask & 0b1000) != 0) {cells[cellCount++] = cellIndex + 1;}
				if ((prisonCellMask & 0b1100) != 0) {cells[cellCount++] = cellIndex + 2;}
			}
			else
			{
				int rotation = this.Rotation.AsInt;

				if ((rotation & 1) == 0)
				{
				/* North or south. */
					/*
						 EF
						C  D
						 AB
					*/
					cellIndex = (cell.z - 1) * sizeX + cell.x - (rotation == 0 ? 1 : 2);
					if ((prisonCellMask & 0b000111) != 0) {cells[cellCount++] = cellIndex;}
					if ((prisonCellMask & 0b000011) != 0) {cells[cellCount++] = cellIndex + 1; cells[cellCount++] = cellIndex + 2;}
					if ((prisonCellMask & 0b001011) != 0) {cells[cellCount++] = cellIndex + 3;}
					cellIndex += sizeX;
					if ((prisonCellMask & 0b000100) != 0) {cells[cellCount++] = cellIndex;}
					if ((prisonCellMask & 0b001000) != 0) {cells[cellCount++] = cellIndex + 3;}
					cellIndex += sizeX;
					if ((prisonCellMask & 0b110100) != 0) {cells[cellCount++] = cellIndex;}
					if ((prisonCellMask & 0b110000) != 0) {cells[cellCount++] = cellIndex + 1; cells[cellCount++] = cellIndex + 2;}
					if ((prisonCellMask & 0b111000) != 0) {cells[cellCount++] = cellIndex + 3;}
				}
				else
				{
				/* East or west. */
					/*
						 F
						E D
						B C
						 A
					*/
					cellIndex = (cell.z - (rotation == 1 ? 2 : 1)) * sizeX + cell.x - 1;
					if ((prisonCellMask & 0b010011) != 0) {cells[cellCount++] = cellIndex;}
					if ((prisonCellMask & 0b000001) != 0) {cells[cellCount++] = cellIndex + 1;}
					if ((prisonCellMask & 0b001101) != 0) {cells[cellCount++] = cellIndex + 2;}
					cellIndex += sizeX;
					if ((prisonCellMask & 0b010010) != 0) {cells[cellCount++] = cellIndex;}
					if ((prisonCellMask & 0b001100) != 0) {cells[cellCount++] = cellIndex + 2;}
					cellIndex += sizeX;
					if ((prisonCellMask & 0b010010) != 0) {cells[cellCount++] = cellIndex;}
					if ((prisonCellMask & 0b001100) != 0) {cells[cellCount++] = cellIndex + 2;}
					cellIndex += sizeX;
					if ((prisonCellMask & 0b110010) != 0) {cells[cellCount++] = cellIndex;}
					if ((prisonCellMask & 0b100000) != 0) {cells[cellCount++] = cellIndex + 1;}
					if ((prisonCellMask & 0b101100) != 0) {cells[cellCount++] = cellIndex + 2;}
				}
			}

			int[] pathFindCostCells = new int[cellCount];

			for (int index = 0; index < cellCount; ++index)
			{
				pathFindCostCells[index] = cells[index];
			}

			this.pathFindCostCells = pathFindCostCells;
		}
	}


	public class RimFridgeNonWallBuildingPlaceWorker : PlaceWorker
	{
		public override void DrawGhost (ThingDef def, IntVec3 centre, Rot4 rotation, Color ghostColour, Thing thing)
		{
			List<IntVec3> cells = new List<IntVec3>();

			Room[] rooms = RimFridge_Building.GatherRooms(centre, Find.CurrentMap);
			int roomCount = rooms.Length;

			for (int index = 0; index < roomCount; ++index)
			{
				Room room = rooms[index];

				if (room != null && !room.UsesOutdoorTemperature)
				{
					cells.AddRange(room.Cells);
					GenDraw.DrawFieldEdges(cells, GenTemperature.ColorRoomHot);
					cells.Clear();
				}
			}
		}
	}


	public class RimFridgeMultiSidedWallBuildingPlaceWorker : PlaceWorker
	{
		public override void DrawGhost (ThingDef def, IntVec3 centre, Rot4 rotation, Color ghostColour, Thing thing)
		{
			Map map = Find.CurrentMap;
			List<IntVec3> cells = new List<IntVec3>();

			var addCell = (IntVec3 offset) =>
			{
				IntVec3 cell = centre + offset.RotatedBy(rotation);

				if (cell.GetRoom(map) != null)
				{
					cells.Add(cell);
				}
			};

			addCell(new IntVec3(0, 0, +1));
			addCell(new IntVec3(0, 0, -1));

			if (def.size.x == 1)
			{
				addCell(new IntVec3(+1, 0, 0));
				addCell(new IntVec3(-1, 0, 0));
			}
			else
			{
				addCell(new IntVec3(+1, 0, +1));
				addCell(new IntVec3(+1, 0, -1));
				addCell(new IntVec3(+2, 0, 0));
				addCell(new IntVec3(-1, 0, 0));
			}

			GenDraw.DrawFieldEdges(cells, new Color(0.35f, 1f, 0f));

			Room[] rooms = RimFridge_WallBuilding.GatherRooms(
				RimFridge_MultiSidedWallBuilding.GatherAdjacentRegions(centre, map, rotation, def)
			);

			int roomCount = rooms.Length;

			for (int index = 0; index < roomCount; ++index)
			{
				Room room = rooms[index];

				if (!room.UsesOutdoorTemperature)
				{
					cells.Clear();
					cells.AddRange(room.Cells);
					GenDraw.DrawFieldEdges(cells, GenTemperature.ColorRoomHot);
				}
			}
		}
	}
}

