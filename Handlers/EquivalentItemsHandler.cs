using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace UtilitySlots.Handlers
{
    public class EquivalentItemsHandler : UtilityItemHandler
	{
		public readonly IReadOnlyList<int> ItemIds;

		public IEnumerable<UtilityItemHandler> Handlers => ItemIds.Select(id => UtilityAccessories.GetHandler(ContentSamples.ItemsByType[id]));

		public EquivalentItemsHandler(params int[] types)
		{
			ItemIds = types;
		}

		public override void ApplyEffect(Player p, bool hideVisual)
		{
			foreach (var handler in Handlers)
				handler.ApplyEffect(p, hideVisual);
		}
	}
}
