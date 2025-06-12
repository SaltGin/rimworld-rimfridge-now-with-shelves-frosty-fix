using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace RimFridge
{
	internal static class CompManipulation
	{
		public static readonly AccessTools.FieldRef<ThingWithComps, List<ThingComp>> compsOfThingWithComps = (
			AccessTools.FieldRefAccess<ThingWithComps, List<ThingComp>>("comps")
		);
		public static readonly AccessTools.FieldRef<ThingWithComps, Dictionary<Type, ThingComp[]>> compsByTypeOfThingWithComps = (
			AccessTools.FieldRefAccess<ThingWithComps, Dictionary<Type, ThingComp[]>>("compsByType")
		);

		internal static void AddCompTo <Comp> (ThingWithComps thingWithComps, Comp comp, CompProperties props)
		where Comp : ThingComp
		{
			List<ThingComp> comps = (compsOfThingWithComps(thingWithComps) ??= new(1));

			/* This is how `ThingWithComps#InitializeComps` does it. */
			try
			{
				comps.Add(comp);
				comp.Initialize(props);
			}
			catch (Exception error)
			{
				Logger.Error($"Failed to initialise a ThingComp: {error}");
				comps.Remove(comp);
				return;
			}

			Dictionary<Type, ThingComp[]> compsByType = (compsByTypeOfThingWithComps(thingWithComps) ??= new(1));

			ThingComp[] compsOfType;
			int offset;

			if (compsByType.TryGetValue(typeof(Comp), out compsOfType))
			{
				offset = compsOfType.Length;
				Array.Resize(ref compsOfType, offset + 1);
			}
			else
			{
				compsOfType = new ThingComp[1];
				offset = 0;
			}

			compsOfType[offset] = comp;
			compsByType[typeof(Comp)] = compsOfType;
		}
	}
}

