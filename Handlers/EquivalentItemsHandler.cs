using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace UtilitySlots.Handlers
{
    public class EquivalentItemsHandler : UtilityItemHandler
	{
		public IList<FullyFunctionalHandler> subHandlers;

		public EquivalentItemsHandler(params int[] types)
		{
			subHandlers = types.Select(type => new FullyFunctionalHandler(type)).ToList();
		}

		public override void ApplyEffect(Player p, bool hideVisual)
		{
			foreach (var handler in subHandlers)
				handler.ApplyEffect(p, hideVisual);
		}
	}
}
