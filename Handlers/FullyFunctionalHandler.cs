using Terraria;

namespace UtilitySlots.Handlers
{
    public class FullyFunctionalHandler : UtilityItemHandler
	{
		protected readonly Item item;

		public FullyFunctionalHandler(int itemId)
		{
			item = new Item(itemId);
		}

		public override bool FullyFunctional => true;

		public override void ApplyEffect(Player p, bool hideVisual)
		{
			p.VanillaUpdateEquip(item);
			p.ApplyEquipFunctional(item, hideVisual);
		}
	}
}
