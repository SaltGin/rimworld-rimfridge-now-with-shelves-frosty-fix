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
		private Dictionary<IntVec3, RimFridge_DoubleSidedWallBuilding> doubleSidedCache;

		public FridgeCache (Map map) : base(map)
		{
			FridgeCacheFast.compCache[map] = (this.FridgeGrid = new Dictionary<IntVec3, CompRefrigerator>());
			FridgeCacheFast.rimFridgeCache[map] = (this.rimFridgeCache = new Dictionary<IntVec3, RimFridge_Building>());
			FridgeCacheFast.wallFridgeCache[map] = (this.wallFridgeCache = new Dictionary<IntVec3, RimFridge_WallBuilding>());
			FridgeCacheFast.doubleSidedCache[map] = (this.doubleSidedCache = new Dictionary<IntVec3, RimFridge_DoubleSidedWallBuilding>());

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
			return this.FridgeGrid.Values;
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

			FridgeCacheFast.AddToCache(FridgeCacheFast.compCache[map], comp, cells);

			if (parent is RimFridge_Building fridge)
			{
				FridgeCacheFast.AddToCache(FridgeCacheFast.rimFridgeCache[map], fridge, cells);

				if (parent is RimFridge_WallBuilding wallFridge)
				{
					FridgeCacheFast.AddToCache(FridgeCacheFast.wallFridgeCache[map], wallFridge, cells);

					if (parent is RimFridge_DoubleSidedWallBuilding doubleSided)
					{
						FridgeCacheFast.AddToCache(FridgeCacheFast.doubleSidedCache[map], doubleSided, cells);
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

			FridgeCacheFast.RemoveFromCache(FridgeCacheFast.compCache[map], cells);

			if (parent is RimFridge_Building fridge)
			{
				FridgeCacheFast.RemoveFromCache(FridgeCacheFast.rimFridgeCache[map], cells);

				if (parent is RimFridge_WallBuilding)
				{
					FridgeCacheFast.RemoveFromCache(FridgeCacheFast.wallFridgeCache[map], cells);

					if (parent is RimFridge_DoubleSidedWallBuilding)
					{
						FridgeCacheFast.RemoveFromCache(FridgeCacheFast.doubleSidedCache[map], cells);
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
			FridgeCacheFast.doubleSidedCache.Remove(this.map);

			this.FridgeGrid.Clear();
			this.rimFridgeCache.Clear();
			this.wallFridgeCache.Clear();
			this.doubleSidedCache.Clear();
		}

		public void ReactToChangeOfRegionsAndRooms ()
		{
			foreach (RimFridge_Building rimFridge in this.rimFridgeCache.Values)
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
		internal static Dictionary<Map, Dictionary<IntVec3, RimFridge_DoubleSidedWallBuilding>> doubleSidedCache;

		internal static void RemoveFromCache <T> (Dictionary<IntVec3, T> cache, CellRect cells)
		{
			foreach (IntVec3 cell in cells)
			{
				cache.Remove(cell);
			}
		}

		internal static void AddToCache <T> (Dictionary<IntVec3, T> cache, T value, CellRect cells)
		{
			foreach (IntVec3 cell in cells)
			{
				cache[cell] = value;
			}
		}
	}
}

