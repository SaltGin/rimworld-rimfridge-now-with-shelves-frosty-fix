using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimFridge
{
	[StaticConstructorOnStartup]
	static internal class FridgeLighting
	{
		internal enum LightType : byte
		{
			Normal = 0,
			Dark = 1,
			Off = 2,
			Last = Off
		}

		internal static Texture2D[] iconsByLightType;
		internal static ColorInt[] coloursByLightType;

		static FridgeLighting ()
		{
			iconsByLightType = new Texture2D[3];
			iconsByLightType[(uint) LightType.Normal] = ContentFinder<Texture2D>.Get("UI/Icons/normal", true);
			iconsByLightType[(uint) LightType.Dark] = ContentFinder<Texture2D>.Get("UI/Icons/dark", true);
			iconsByLightType[(uint) LightType.Off] = ContentFinder<Texture2D>.Get("UI/Icons/off", true);

			coloursByLightType = new ColorInt[3];
			coloursByLightType[(uint) LightType.Normal] = new ColorInt(89, 188, 255, 0);
			coloursByLightType[(uint) LightType.Dark] = new ColorInt(78, 226, 229, 0);
			coloursByLightType[(uint) LightType.Off] = new ColorInt(0, 0, 0, 0);
		}

		internal static LightType CycleLightType (LightType type)
		{
			uint nextType = (uint) type + 1;
			nextType += ((nextType == (uint) LightType.Dark) & !ModsConfig.IdeologyActive) ? (uint) 1 : (uint) 0;
			nextType = nextType <= (uint) LightType.Last ? nextType : (uint) LightType.Normal;

			return (LightType) nextType;
		}
	}

	public sealed class CompProperties_ToggleGlower : CompProperties_Glower
	{
		public CompProperties_ToggleGlower ()
		{
			base.compClass = typeof(CompToggleGlower);
		}
	}

	sealed class CompToggleGlower : CompGlower
	{
		FridgeLighting.LightType lightType;

		public override IEnumerable<Gizmo> CompGetGizmosExtra ()
		{
			foreach (var g in base.CompGetGizmosExtra())
				yield return g;

			Texture2D icon = FridgeLighting.iconsByLightType[(uint) lightType];

			yield return new Command_Action
			{
				action = delegate
				{
					Verse.Sound.SoundStarter.PlayOneShotOnCamera(RimWorld.SoundDefOf.Tick_High);
					this.CycleLightType();
				},
				defaultLabel = "RimFridge.ToggleGlowColor".Translate(),
				defaultDesc = "RimFridge.ToggleGlowColorDesc".Translate(),
				icon = icon
			};
		}

		protected override bool ShouldBeLitNow
		{
			get
			{
				if (this.lightType == FridgeLighting.LightType.Off)
				{
					return false;
				}

				return base.ShouldBeLitNow;
			}
		}

		public override void PostSpawnSetup (bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			this.SetLightTypeTo(this.lightType);
		}

		private void CycleLightType ()
		{
			this.SetLightTypeTo(FridgeLighting.CycleLightType(this.lightType));
		}

		private void SetLightTypeTo (FridgeLighting.LightType type)
		{
			this.lightType = type;
			base.Props.glowColor = FridgeLighting.coloursByLightType[(uint) type];
			base.parent.Map.glowGrid.DeRegisterGlower(this);
			base.parent.Map.glowGrid.RegisterGlower(this);
		}

		public override void PostExposeData ()
		{
			base.PostExposeData();

			if (Scribe.mode == LoadSaveMode.Saving)
			{
				Scribe_Values.Look(ref this.lightType, "lightType", FridgeLighting.LightType.Normal, true);
			}
			else if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				/* Before version 2.0.0 of RimFridge, there were only two light-types,
				   and the active type was persisted via the `isDarklight` boolean,
				   hence we use a light-type of 255 to migrate old glowers. */

				byte lightType = 255;
				Scribe_Values.Look(ref lightType, "lightType", (byte) 255);

				if (lightType == 255)
				{
					bool isDarklight = false;
					Scribe_Values.Look(ref isDarklight, "isDarklight", false);

					lightType = isDarklight ? (byte) FridgeLighting.LightType.Dark : (byte) FridgeLighting.LightType.Normal;
				}

				this.lightType = (FridgeLighting.LightType) (
					  lightType <= (byte) FridgeLighting.LightType.Last
					? lightType
					: (byte) FridgeLighting.LightType.Normal
				);
			}
		}
	}
}

