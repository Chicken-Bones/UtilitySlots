using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace UtilitySlots
{
	public static class InvUtils
	{
		public static void DropItem(Vector2 pos, Item item) {
			int i = Item.NewItem((int)pos.X, (int)pos.Y, 0, 0, item.type);
			Main.item[i].netDefaults(item.netID);
			Main.item[i].Prefix(item.prefix);
			Main.item[i].stack = item.stack;
			Main.item[i].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
			Main.item[i].velocity.X = Main.rand.Next(-20, 21) * 0.2f;
			Main.item[i].noGrabDelay = 100;
			if (Main.netMode == 1)
				NetMessage.SendData(21, -1, -1, null, i);
		}

		public static void DropItems(Vector2 pos, Item[] items) {
			for (int i = 0; i < items.Length; i++) {
				if (items[i].stack != 0) {
					DropItem(pos, items[i]);
					items[i] = new Item();
				}
			}
		}

		public static Item Default(int id, bool noMatCheck = false) {
			var item = new Item();
			item.SetDefaults(id, noMatCheck);
			return item;
		}

		public static void DrawSlot(SpriteBatch sb, Vector2 slotPos, Texture2D tex, Item item, Texture2D emptyTex = null) {
			sb.Draw(tex, slotPos, null, Main.inventoryBack, 0f, default, Main.inventoryScale, SpriteEffects.None, 0f);

			if (item != null && !item.IsAir)
				DrawItem(sb, item, slotPos, tex.Size());
			else if (emptyTex != null)
				sb.Draw(emptyTex, slotPos + tex.Size() * Main.inventoryScale / 2,
					null, Color.White * 0.35f, 0f,
					emptyTex.Size() / 2, Main.inventoryScale, SpriteEffects.None, 0f);
		}

		public static void DrawItem(SpriteBatch sb, Item item, Vector2 slotPos, Vector2 slotSize) {
			Main.instance.LoadItem(item.type);
			var itemTexture = TextureAssets.Item[item.type].Value;
			var frame = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();
			var lightColor = Color.White;
			float itemScale = 1f;
			ItemSlot.GetItemLight(ref lightColor, ref itemScale, item);
			float texScale = 1f;
			if (frame.Width > 32 || frame.Height > 32)
				texScale = 32f/Math.Max(frame.Width, frame.Height);
			texScale *= Main.inventoryScale;
			var drawColor = item.GetAlpha(lightColor);
			var itemColor = item.GetColor(Color.White);

			var pos = slotPos + slotSize * Main.inventoryScale / 2f - frame.Size() * texScale / 2f;
			var origin = frame.Size() * (itemScale / 2f - 0.5f);
			if (ItemLoader.PreDrawInInventory(item, sb, pos, frame, drawColor, itemColor, origin, texScale * itemScale)) {
				sb.Draw(itemTexture, pos, frame, drawColor, 0f, origin, texScale * itemScale, SpriteEffects.None, 0f);
				if (item.color != Color.Transparent)
					sb.Draw(itemTexture, pos, frame, itemColor, 0f, origin, texScale * itemScale, SpriteEffects.None, 0f);
			}
			ItemLoader.PostDrawInInventory(item, sb, pos, frame, drawColor, itemColor, origin, texScale * itemScale);
		}

		public static void Swap(ref Item a, ref Item b) {
			var tmp = a.Clone();
			a = b.Clone();
			b = tmp;
		}

		/*public static void Swap(ItemSlot slot1, bool equip1, ItemSlot slot2, bool equip2) {
			if (slot1.MyItem.IsBlank() && slot2.MyItem.IsBlank())
				return;

			var temp = new Item();
			Swap(ref temp, slot2, equip2);//get dest
			Swap(ref temp, slot1, equip1);//swap src
			Swap(ref temp, slot2, equip2);//put dest

			Main.PlaySound(7, -1, -1, 1);
		}

		public static void Swap(ref Item held, ItemSlot slot, bool equip) {
			if (equip && !slot.MyItem.IsBlank())
				slot.MyItem.OnUnEquip(Main.localPlayer, slot);

			//do the swap
			var temp = held;
			held = slot.MyItem;
			slot.MyItem = temp;

			if (equip && !slot.MyItem.IsBlank())
				slot.MyItem.OnUnEquip(Main.localPlayer, slot);
		}

		public static IEnumerable<ItemSlot> OrderedInventory() {
			return ItemSlot.inventory.Skip(10).Concat(ItemSlot.inventory.Take(10));
		}

		public static bool StackItem(ref Item item, IEnumerable<ItemSlot> dest, Action<ItemSlot, int> func = null) {
			var moved = false;
			var type = item;
			foreach (var slot in dest.Where(s => s.MyItem.IsTheSameAs(type) && s.MyItem.stack < s.MyItem.maxStack).ToList()) {
				var delta = Math.Min(item.maxStack - slot.MyItem.stack, item.stack);
				slot.MyItem.stack += delta;
				item.stack -= delta;
				moved = true;

				if (func != null)
					func(slot, delta);

				if (item.IsBlank()) {
					item = new Item();
					return true;
				}
			}

			return moved;
		}

		internal static bool AddItem(ref Item item, IEnumerable<ItemSlot> dest, Action<ItemSlot, int> func = null) {
			var moved = StackItem(ref item, dest, func);

			if (item.IsBlank())
				return moved;

			var destSlot = dest.FirstOrDefault(s => s.MyItem.IsBlank());
			if (destSlot == null)
				return moved;

			Swap(ref item, destSlot, false);

			if (func != null)
				func(destSlot, destSlot.MyItem.stack);

			return moved;
		}

		public static int SpaceFor(Item item, IEnumerable<ItemSlot> slots) {
			return slots.Sum(s =>
				s.MyItem.IsBlank() ? item.maxStack : 
				s.MyItem.IsTheSameAs(item) ? item.maxStack - s.MyItem.stack : 0);
		}*/
	}
}