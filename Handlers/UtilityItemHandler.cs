using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace UtilitySlots.Handlers
{
    public abstract class UtilityItemHandler
	{
		public IList<string> negatedTipLines = new List<string>();

		public virtual bool FullyFunctional => false;

		public abstract void ApplyEffect(Player p, bool hideVisual);

		public virtual void ModifyTooltip(List<TooltipLine> tooltip)
		{
			foreach (var line in tooltip)
			{
				if (line.isModifier)
				{
					line.overrideColor = Color.Gray;
					line.text = "Modifiers have no effect";
				}
				if (negatedTipLines.Contains(line.Name))
					line.overrideColor = Color.Gray;
			}
		}

		public UtilityItemHandler NegateTip(params string[] lines)
		{
			negatedTipLines = lines;
			return this;
		}
	}
}
