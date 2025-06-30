using Verse;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RimFridge
{
	/* FridgeCache forwards to FridgeCacheFast because there are
	   mods that use this class via reflection.

	   For any mod-authors reading this:
	   please continue using `FridgeCache` instead of `FridgeCacheFast`,
	   I won't break the public API. */
	public sealed class FridgeCache : MapComponent
	{
		private Dictionary<IntVec3, CompRefrigerator> FridgeGrid;
		private Dictionary<IntVec3, RimFridge_Building> rimFridgeCache;
		private Dictionary<IntVec3, RimFridge_WallBuilding> wallFridgeCache;
		private Dictionary<IntVec3, RimFridge_MultiSidedWallBuilding> multiSidedCache;
		private List<CompRefrigerator> compList;
		private List<RimFridge_Building> rimFridgeList;
		private List<RimFridge_WallBuilding> wallFridgeList;
		private List<RimFridge_MultiSidedWallBuilding> multiSidedList;

		public FridgeCache (Map map) : base(map)
		{
			FridgeCacheFast.compCache[map] = (this.FridgeGrid = new Dictionary<IntVec3, CompRefrigerator>());
			FridgeCacheFast.rimFridgeCache[map] = (this.rimFridgeCache = new Dictionary<IntVec3, RimFridge_Building>());
			FridgeCacheFast.wallFridgeCache[map] = (this.wallFridgeCache = new Dictionary<IntVec3, RimFridge_WallBuilding>());
			FridgeCacheFast.multiSidedCache[map] = (this.multiSidedCache = new Dictionary<IntVec3, RimFridge_MultiSidedWallBuilding>());
			FridgeCacheFast.compList[map] = (this.compList = new List<CompRefrigerator>());
			FridgeCacheFast.rimFridgeList[map] = (this.rimFridgeList = new List<RimFridge_Building>());
			FridgeCacheFast.wallFridgeList[map] = (this.wallFridgeList = new List<RimFridge_WallBuilding>());
			FridgeCacheFast.multiSidedList[map] = (this.multiSidedList = new List<RimFridge_MultiSidedWallBuilding>());

			map.events.RegionsRoomsChanged += this.ReactToChangeOfRegionsAndRooms;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasFridgeAt (IntVec3 cell)
		{
			return this.FridgeGrid.ContainsKey(cell);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<CompRefrigerator> GetFridgeComps ()
		{
			return this.compList;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FridgeCache GetFridgeCache (Map map)
		{
			return map.GetComponent<FridgeCache>();
		}

		public static void AddFridge (CompRefrigerator comp, Map map)
		{
			Thing parent = comp.parent;
			CellRect cells = GenAdj.OccupiedRect(parent);

			FridgeCacheFast.AddToCache(FridgeCacheFast.compCache[map], FridgeCacheFast.compList[map], comp, cells);

			if (parent is RimFridge_Building fridge)
			{
				FridgeCacheFast.AddToCache(FridgeCacheFast.rimFridgeCache[map], FridgeCacheFast.rimFridgeList[map], fridge, cells);

				if (parent is RimFridge_WallBuilding wallFridge)
				{
					FridgeCacheFast.AddToCache(FridgeCacheFast.wallFridgeCache[map], FridgeCacheFast.wallFridgeList[map], wallFridge, cells);

					if (parent is RimFridge_MultiSidedWallBuilding multiSided)
					{
						FridgeCacheFast.AddToCache(FridgeCacheFast.multiSidedCache[map], FridgeCacheFast.multiSidedList[map], multiSided, cells);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetFridge (IntVec3 cell, Map map, out CompRefrigerator comp)
		{
			return FridgeCacheFast.compCache[map].TryGetValue(cell, out comp);
		}

		public static void RemoveFridge (CompRefrigerator comp, Map map)
		{
			Thing parent = comp.parent;
			CellRect cells = GenAdj.OccupiedRect(parent);

			FridgeCacheFast.RemoveFromCache(FridgeCacheFast.compCache[map], FridgeCacheFast.compList[map], comp, cells);

			if (parent is RimFridge_Building fridge)
			{
				FridgeCacheFast.RemoveFromCache(FridgeCacheFast.rimFridgeCache[map], FridgeCacheFast.rimFridgeList[map], fridge, cells);

				if (parent is RimFridge_WallBuilding wallFridge)
				{
					FridgeCacheFast.RemoveFromCache(FridgeCacheFast.wallFridgeCache[map], FridgeCacheFast.wallFridgeList[map], wallFridge, cells);

					if (parent is RimFridge_MultiSidedWallBuilding multiSided)
					{
						FridgeCacheFast.RemoveFromCache(FridgeCacheFast.multiSidedCache[map], FridgeCacheFast.multiSidedList[map], multiSided, cells);
					}
				}
			}
		}

		public override void ExposeData ()
		{
			base.ExposeData();
		}

		public override void MapRemoved ()
		{
			FridgeCacheFast.compCache.Remove(this.map);
			FridgeCacheFast.rimFridgeCache.Remove(this.map);
			FridgeCacheFast.wallFridgeCache.Remove(this.map);
			FridgeCacheFast.multiSidedCache.Remove(this.map);
			FridgeCacheFast.compList.Remove(this.map);
			FridgeCacheFast.rimFridgeList.Remove(this.map);
			FridgeCacheFast.wallFridgeList.Remove(this.map);
			FridgeCacheFast.multiSidedList.Remove(this.map);

			this.FridgeGrid.Clear();
			this.rimFridgeCache.Clear();
			this.wallFridgeCache.Clear();
			this.multiSidedCache.Clear();
			this.compList.Clear();
			this.rimFridgeList.Clear();
			this.wallFridgeList.Clear();
			this.multiSidedList.Clear();
		}

		public void ReactToChangeOfRegionsAndRooms ()
		{
			foreach (RimFridge_Building rimFridge in this.rimFridgeList)
			{
				rimFridge.ReactToChangeOfRegionsAndRooms();
			}
		}
	}

	internal static class FridgeCacheFast
	{
		internal static Dictionary<Map, Dictionary<IntVec3, CompRefrigerator>> compCache;
		internal static Dictionary<Map, Dictionary<IntVec3, RimFridge_Building>> rimFridgeCache;
		internal static Dictionary<Map, Dictionary<IntVec3, RimFridge_WallBuilding>> wallFridgeCache;
		internal static Dictionary<Map, Dictionary<IntVec3, RimFridge_MultiSidedWallBuilding>> multiSidedCache;
		internal static Dictionary<Map, List<CompRefrigerator>> compList;
		internal static Dictionary<Map, List<RimFridge_Building>> rimFridgeList;
		internal static Dictionary<Map, List<RimFridge_WallBuilding>> wallFridgeList;
		internal static Dictionary<Map, List<RimFridge_MultiSidedWallBuilding>> multiSidedList;

		internal static void RemoveFromCache <T> (Dictionary<IntVec3, T> cache, List<T> list, T value, CellRect cells)
		{
			foreach (IntVec3 cell in cells)
			{
				cache.Remove(cell);
			}

			list.Remove(value);
		}

		internal static void AddToCache <T> (Dictionary<IntVec3, T> cache, List<T> list, T value, CellRect cells)
		{
			foreach (IntVec3 cell in cells)
			{
				cache[cell] = value;
			}

			if (!list.Contains(value))
			{
				list.Add(value);
			}
		}
	}

	internal static class PrisonCellStateTracking
	{
		internal static void ReactToChangeOfPrisonCellStatusForRoom (Room room, bool isPrisonCell)
		{
			foreach (RimFridge_MultiSidedWallBuilding multiSided in FridgeCacheFast.multiSidedList[room.Map])
			{
				multiSided.ReactToChangeOfPrisonCellStatusForRoom();
			}
		}
	}
}

