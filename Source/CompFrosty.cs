using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace RimFridge
{
	internal sealed class CompFrosty : ThingComp
	{
		public static readonly AccessTools.FieldRef<TickManager, TickList> tickListRareOfTickManager = (
			AccessTools.FieldRefAccess<TickManager, TickList>("tickListRare")
		);

		// Most beer's ideal temperature is around 8 degC
		private const float IDEAL_TEMPERATURE = 8f;

		// Starting temperature
		public float temperature = 21f;

		public CompProperties_Frosty Props => (CompProperties_Frosty) props;

		public override void PostIngested (Pawn ingester)
		{
			base.PostIngested(ingester);

			if (temperature <= IDEAL_TEMPERATURE)
			{
				ingester.needs.mood.thoughts.memories.TryGainMemory(Props.thought, null);
			}
		}

		public override void PostSplitOff (Thing piece)
		{
			ThingWithComps thingWithComps = piece as ThingWithComps;

			if (thingWithComps.GetComp<CompFrosty>() == null)
			{
				CompFrosty compFrosty = new CompFrosty();
				compFrosty.parent = thingWithComps;
				compFrosty.temperature = temperature;
				CompManipulation.AddCompTo(thingWithComps, compFrosty, CompProperties_Frosty.Beer);

				/* If this thing's ticker-type is rare,
				   it will have already been registered in the rare-tick-list
				   by `Thing#SpawnSetup`; if so we won't register it again. */
				if (thingWithComps.def.tickerType != TickerType.Rare)
				{
					tickListRareOfTickManager(Find.TickManager).RegisterThing(thingWithComps);
				}
			}
		}

		public override void CompTickRare ()
		{
			base.CompTickRare();

			Map map = this.parent.MapHeld;
			IntVec3 cell = this.parent.PositionHeld;

			if (map == null)
			{
				/* We'll just assume that the beverage is frozen in time. */
				return;
			}

			float ambientTemperature;

			if (FridgeCache.TryGetFridge(cell, map, out CompRefrigerator fridge))
			{
				ambientTemperature = fridge.currentTemp;
			}
			else
			{
				GenTemperature.TryGetTemperatureForCell(cell, map, out ambientTemperature);
			}

			this.temperature += (ambientTemperature - this.temperature) * 0.05f;
		}

		public override string CompInspectStringExtra ()
		{
			return (temperature <= IDEAL_TEMPERATURE) ? "RimFridge.FrostyBeverage".Translate() : "";
		}
	}
}

