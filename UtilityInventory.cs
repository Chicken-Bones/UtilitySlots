using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace UtilitySlots
{
    public class UtilityInventory : ModPlayer
	{
		public override void SaveData(TagCompound tag) {}

		public override void LoadData(TagCompound tag) {
			var items = tag.GetList<TagCompound>("items").Select(ItemIO.Load).ToArray();
			var dyes = tag.GetList<TagCompound>("dyes").Select(ItemIO.Load).ToArray();
			byte hideVisual = tag.GetByte("hideVisual");

			var slots = ContentInstance<UtilitySlot>.Instances;
			for (int i = 0; i < Player.SupportedSlotsAccs; i++) {
				var slot = slots[i];
				if (i < items.Length) slot.FunctionalItem = items[i];
				if (i < dyes.Length) slot.DyeItem = dyes[i];
				slot.HideVisuals |= (hideVisual & (1 << i)) != 0;
			}
		}
	}
}