using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
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

        public override bool CanAcceptItem(Item checkItem) => 
            UtilityAccessories.GetHandler(checkItem) != null;

        public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) =>
            IsEmpty && (UtilityAccessories.GetHandler(item)?.FullyFunctional ?? false);

        public override void ApplyEquipEffects() {
            UtilityAccessories.GetHandler(FunctionalItem)?.ApplyEffect(Player, HideVisuals);
            Player.ApplyEquipVanity(VanityItem);
        }

        public override void OnMouseHover(int context) {
            if (context == -10) {
                if (FunctionalItem.IsAir)
                    Main.hoverItemName = "Utility Accessory";
                else
                    GlobalItemHook.UtilityHoverItem = Main.HoverItem;
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
