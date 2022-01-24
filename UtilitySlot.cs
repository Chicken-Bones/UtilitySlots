using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace UtilitySlots
{
    [Autoload(false)]
    class UtilitySlot : ModAccessorySlot
    {
        private readonly int index;

        public UtilitySlot(int i) {
            index = i;
        }

        public override string Name => $"UtilitySlot_{index}";

        public override string FunctionalBackgroundTexture => "UtilitySlots/assets/background";

        public override string FunctionalTexture => "UtilitySlots/assets/slot_bg_utility";

        public override Vector2? CustomLocation => new Vector2(Main.screenWidth - 64 - 28, AccessorySlotLoader.DrawVerticalAlignment + (int)(index * 56 * Main.inventoryScale));

        public override bool DrawVanitySlot => false;

        public override bool IsEnabled() => Player.IsAValidEquipmentSlotForIteration(index + 3);

        public override bool IsHidden() => !UtilitySlotsEquipPage.IsSelected;

        public override bool CanAcceptItem(Item checkItem, AccessorySlotType slotType) =>
            slotType == AccessorySlotType.DyeSlot || slotType == AccessorySlotType.FunctionalSlot && UtilityAccessories.GetHandler(checkItem) != null;

        public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) =>
			FunctionalItem.IsAir && (UtilityAccessories.GetHandler(item)?.FullyFunctional ?? false);

        public override void ApplyEquipEffects() {
            UtilityAccessories.GetHandler(FunctionalItem)?.ApplyEffect(Player, HideVisuals);
            Player.ApplyEquipVanity(VanityItem);
        }

        public override void OnMouseHover(AccessorySlotType context) {
            if (context == AccessorySlotType.FunctionalSlot) {
                if (FunctionalItem.IsAir)
                    Main.hoverItemName = Language.GetTextValue("Mods.UtilitySlots.SlotText");
                else
                    GlobalItemHook.UtilityHoverItem = Main.HoverItem;
            }
        }

		public override void PostDraw(AccessorySlotType context, Item item, Vector2 position, bool isHovered) {
			// if this is the last enabled slot
			if (context == AccessorySlotType.FunctionalSlot && ContentInstance<UtilitySlot>.Instances.Last(slot => slot.IsEnabled()) == this) {
				UtilitySlotsEquipPage.DrawPartiallyFunctionalAccDetails(position);
			}
		}

		private class GlobalItemHook : GlobalItem
        {
            public static Item UtilityHoverItem;

            public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
                if (item == UtilityHoverItem)
                    UtilityAccessories.GetHandler(item).ModifyTooltip(tooltips);
            }
        }
    }
}
