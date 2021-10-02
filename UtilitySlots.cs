using Terraria;
using Terraria.ModLoader;

namespace UtilitySlots
{
    public class UtilitySlots : Mod
	{
		public static UtilitySlots Instance { get; private set; }

		public override void Load() {
			Instance = this;

			for (int i = 0; i < Player.SupportedSlotsAccs; i++)
				AddContent(new UtilitySlot(i));
		}

		public override void PostSetupContent() {
			UtilityAccessories.Load();
		}

		public override void Unload() {
			UtilityAccessories.Unload();

			Instance = null;
		}

		//public override void HandlePacket(BinaryReader reader, int whoAmI) => NetHandler.HandlePacket(reader, whoAmI);
	}
}
