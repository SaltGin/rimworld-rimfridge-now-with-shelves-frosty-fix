using RimWorld;
using Verse;

namespace RimFridge
{
	internal class FloatInput
	{
		private readonly string name;
		public string AsString;

		public FloatInput (string name, float initialValue = 1f)
		{
			this.name = name;
			AsString = initialValue.ToString();
		}

		public float AsFloat
		{
			get => ValidateInput () ? float.Parse(AsString) : 1f;
			set => AsString = value.ToString();
		}

		public bool ValidateInput ()
		{
			if (float.TryParse(AsString, out float f))
			{
				if (f <= 0)
				{
					Messages.Message("RimFridge.CannotBeLessThanOrEqualToZero".Translate(name.Translate()), MessageTypeDefOf.RejectInput);
					return false;
				}
			}
			else
			{
				Messages.Message("RimFridge.UnableToParseToANumber".Translate(name.Translate()), MessageTypeDefOf.RejectInput);
				return false;
			}

			return true;
		}

		public void Copy (FloatInput fi)
		{
			AsString = fi.AsString;
		}
	}
}

