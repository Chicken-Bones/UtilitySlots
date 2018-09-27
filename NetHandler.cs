using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace UtilitySlots
{
	internal class NetHandler
	{
		public const byte InventorySlot = 1;
		public const byte VisualState = 2;

		public static void HandlePacket(BinaryReader r, int fromWho) {
			switch (r.ReadByte()) {
				case InventorySlot:
					HandleSlot(r, fromWho);
					break;
				case VisualState:
					HandleVisualState(r, fromWho);
					break;
			}
		}

		public static void SendSlot(int toWho, int plr, int slot, Item item) {
			var p = UtilitySlots.Instance.GetPacket();
			p.Write(InventorySlot);
			if (Main.netMode == 2) p.Write((byte)plr);
			p.Write((byte)slot);
			ItemIO.Send(item, p, true);
			p.Send(toWho, plr);
		}

		private static void HandleSlot(BinaryReader r, int fromWho) {
			if (Main.netMode == 1) fromWho = r.ReadByte();
			var utilityInv = Main.player[fromWho].UtilityInv();
			var slot = r.ReadByte();
			var item = ItemIO.Receive(r, true);
			utilityInv.SetSlot(slot, item);

			if (Main.netMode == 2)
				SendSlot(-1, fromWho, slot, item);
		}

		public static void SendVisualState(int toWho, int plr, byte hideVisual) {
			var p = UtilitySlots.Instance.GetPacket();
			p.Write(VisualState);
			if (Main.netMode == 2) p.Write((byte)plr);
			p.Write(hideVisual);
			p.Send(toWho, plr);
		}

		private static void HandleVisualState(BinaryReader r, int fromWho) {
			if (Main.netMode == 1) fromWho = r.ReadByte();
			var utilityInv = Main.player[fromWho].UtilityInv();
			utilityInv.hideVisual = r.ReadByte();

			if (Main.netMode == 2)
				SendVisualState(-1, fromWho, utilityInv.hideVisual);
		}
	}
}
