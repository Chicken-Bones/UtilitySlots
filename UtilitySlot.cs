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

        public override int XLoc => Main.screenWidth - 64 - 28;

        public override int YLoc => AccessorySlotLoader.DrawVerticalAlignment + (int)(index * 56 * Main.inventoryScale);

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
    }
}
