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
			float num = 15f;

			if (parent.MapHeld != null)
			{
				num = GridsUtility.GetTemperature(parent.PositionHeld, parent.MapHeld);
			}

			CompEquippable comp = parent.GetComp<CompEquippable>();

			if (comp != null)
			{
				Pawn casterPawn = comp.PrimaryVerb.CasterPawn;

				if (casterPawn != null)
				{
					num = GridsUtility.GetTemperature(casterPawn.PositionHeld, casterPawn.MapHeld);
				}
			}

			if (parent.Spawned)
			{
				List<Thing> thingList = GridsUtility.GetThingList(parent.PositionHeld, parent.MapHeld);

				for (int i = 0; i < thingList.Count; i++)
				{
					CompRefrigerator fridge = ThingCompUtility.TryGetComp<CompRefrigerator>(thingList[i]);

					if (fridge != null)
					{
						num = fridge.currentTemp;
						break;
					}
				}
			}

			temperature += (num - temperature) * 0.05f;
		}

		public override string CompInspectStringExtra ()
		{
			return (temperature <= IDEAL_TEMPERATURE) ? "RimFridge.FrostyBeverage".Translate() : "";
		}
	}
}

