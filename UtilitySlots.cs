using System.IO;
using Terraria.ModLoader;

namespace UtilitySlots
{
	public class UtilitySlots : Mod
	{
		public static UtilitySlots Instance { get; private set; }
		
		public UtilitySlots() {
			Properties = new ModProperties {
				Autoload = true
			};
		}

		public override void Load() {
			Instance = this;

			UtilityAccessories.Load();
			UtilityInventory.Hook();
		}

		public override void Unload() {
			UtilityAccessories.Unload();
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) => NetHandler.HandlePacket(reader, whoAmI);
	}
}
