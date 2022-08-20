using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace UtilitySlots.Handlers
{
    public abstract class UtilityItemHandler
	{
		public virtual bool FullyFunctional => false;

		public virtual string PartiallyFunctionalHintKey { get; protected set; }

		public virtual IList<string> NegatedTipLines { get; protected set; } = new List<string>();

		public abstract void ApplyEffect(Player p, bool hideVisual);

		public virtual void ModifyTooltip(List<TooltipLine> tooltip)
		{
			foreach (var line in tooltip)
			{
				if (line.IsModifier)
				{
					line.OverrideColor = Color.Gray;
					line.Text = "Modifiers have no effect";
				}
				if (NegatedTipLines.Contains(line.Name))
					line.OverrideColor = Color.Gray;
			}
		}

		public UtilityItemHandler NegateTip(params string[] lines)
		{
			NegatedTipLines = lines;
			return this;
		}

		public UtilityItemHandler WithHint(string key) {
			PartiallyFunctionalHintKey = key;
			return this;
		}
	}
}
