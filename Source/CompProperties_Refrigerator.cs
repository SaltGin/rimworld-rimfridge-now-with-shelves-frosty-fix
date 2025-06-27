using System.Collections.Generic;
using Verse;

namespace RimFridge
{
	public sealed class CompProperties_Refrigerator : CompProperties
	{

		public CompProperties_Refrigerator ()
		{
			compClass = typeof(CompRefrigerator);
		}

		public List<string> drinksBestCold;
		public float defaultDesiredTemperature = -5f;
		public bool findAllRottableForFilters;

		public HashSet<ThingDef> drinksBestColdDefs;

		public override void ResolveReferences (ThingDef parentDef)
		{
			this.drinksBestColdDefs = new HashSet<ThingDef>();

			foreach (string defName in this.drinksBestCold)
			{
				ThingDef def = DefDatabase<ThingDef>.GetNamed(defName, false);

				if (def != null)
				{
					this.drinksBestColdDefs.Add(def);
				}
			}
		}
	}
}

