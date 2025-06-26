using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimFridge
{
	class RimFridge_Building : Building_Storage, IRenameable
	{
		public Room[] rooms;

		public RimFridge_Building () : base()
		{}

		public string fridgeLabel;

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
				action = () => Find.WindowStack.Add(new Dialog_RenameFridge(this)),
				defaultLabel = "Rename".Translate(),
				defaultDesc = "RimFridge.RenameTheRefrigerator".Translate(),
				hotKey = KeyBindingDefOf.Misc1,
				icon = ContentFinder<Texture2D>.Get("UI/Buttons/Rename", true)
			};
		}

		public override void ExposeData ()
		{
			base.ExposeData();

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
		}
	}

	public class RimFridge_DoubleSidedWallBuilding : RimFridge_WallBuilding
	{
		public RimFridge_DoubleSidedWallBuilding () : base()
		{}

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
	}
}

