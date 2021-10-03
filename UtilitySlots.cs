using Terraria;
using Terraria.ModLoader;

namespace UtilitySlots
{
    public class UtilitySlots : Mod
	{
		public override void Load() {
			for (int i = 0; i < Player.SupportedSlotsAccs; i++)
				AddContent(new UtilitySlot(i));
		}
	}
}
