using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace UtilitySlots
{
	public class GlobalItemHook : GlobalItem
	{
		public override bool CanEquipAccessory(Item item, Player player, int slot) =>
			!player.UtilityInv().AccCheck(item, slot);

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
			Main.player[Main.myPlayer].UtilityInv().ModifyTooltips(item, tooltips);
		}
	}
}
