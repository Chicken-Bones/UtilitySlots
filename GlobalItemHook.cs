using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace UtilitySlots
{
	public class GlobalItemHook : GlobalItem
	{
        public override void Load() {
            On.Terraria.UI.ItemSlot.MouseHover_ItemArray_int_int += ItemSlot_MouseHover_ItemArray_int_int;
        }

        private static Item _hoverItemTextOverride;

        private void ItemSlot_MouseHover_ItemArray_int_int(On.Terraria.UI.ItemSlot.orig_MouseHover_ItemArray_int_int orig, Item[] inv, int context, int slot) {
            orig(inv, context, slot);
            var item = inv[slot];
            if (ContentInstance<UtilitySlot>.Instances.Any(slot => item == slot.FunctionalItem))
                _hoverItemTextOverride = Main.HoverItem;
            else
                _hoverItemTextOverride = null;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
			if (item == _hoverItemTextOverride)
				UtilityAccessories.GetHandler(item).ModifyTooltip(tooltips);
		}
	}
}
