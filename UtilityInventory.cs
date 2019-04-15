using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using static UtilitySlots.UtilityAccessories;
using static Mono.Cecil.Cil.OpCodes;
using MonoMod.Cil;

namespace UtilitySlots
{
	public static class UtilityPlayer
	{
		public static UtilityInventory UtilityInv(this Player player) =>
			player.GetModPlayer<UtilityInventory>();
	}

	public class UtilityInventory : ModPlayer
	{
		public override bool CloneNewInstances => false;

		public static bool Incompatible(Item item1, Item item2, bool vanity) {
			return item1.type > 0 && item2.type > 0 &&
				   (item1.IsTheSameAs(item2) || !vanity && item1.wingSlot > 0 && item2.wingSlot > 0);
		}

		private const int EquipPage = 4;

		public Item[] items = new Item[Player.SupportedSlotsAccs];
		public Item[] dyes = new Item[Player.SupportedSlotsAccs];
		public BitsByte hideVisual = new BitsByte();
		private bool handlingEquip;
		private bool utilitySlotHover;
		private PlayerDrawInfo shaderCache;

		public int SlotCount => 5 + player.extraAccessorySlots;

		public override void Initialize() {
			for (int i = 0; i < items.Length; i++) {
				items[i] = new Item();
				dyes[i] = new Item();
			}
		}

		public override void clientClone(ModPlayer clientClone) {
			var utilityInv = (UtilityInventory) clientClone;
			for (int i = 0; i < items.Length; i++)
				utilityInv.items[i] = items[i].Clone();
			for (int i = 0; i < dyes.Length; i++)
				utilityInv.dyes[i] = dyes[i].Clone();

			utilityInv.hideVisual = hideVisual;
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
			for (int i = 0; i < items.Length; i++)
				NetHandler.SendSlot(toWho, player.whoAmI, i, items[i]);
			for (int i = 0; i < dyes.Length; i++)
				NetHandler.SendSlot(toWho, player.whoAmI, i + items.Length, dyes[i]);

			NetHandler.SendVisualState(toWho, player.whoAmI, hideVisual);
		}

		public override void SendClientChanges(ModPlayer clientPlayer) {
			var clientInv = (UtilityInventory) clientPlayer;
			for (int i = 0; i < items.Length; i++)
				if (items[i].IsNotTheSameAs(clientInv.items[i]))
					NetHandler.SendSlot(-1, player.whoAmI, i, items[i]);

			for (int i = 0; i < dyes.Length; i++)
				if (dyes[i].IsNotTheSameAs(clientInv.dyes[i]))
					NetHandler.SendSlot(-1, player.whoAmI, i + items.Length, dyes[i]);

			if (hideVisual != clientInv.hideVisual)
				NetHandler.SendVisualState(-1, player.whoAmI, hideVisual);
		}

		public void SetSlot(byte slot, Item item) {
			if (slot < items.Length)
				items[slot] = item;
			else
				dyes[slot - items.Length] = item;
		}

		public override TagCompound Save() {
			return new TagCompound {
				["items"] = items.Select(ItemIO.Save).ToList(),
				["dyes"] = dyes.Select(ItemIO.Save).ToList(),
				["hideVisual"] = (byte) hideVisual
			};
		}

		public override void Load(TagCompound tag) {
			tag.GetList<TagCompound>("items").Select(ItemIO.Load).ToList().CopyTo(items);
			tag.GetList<TagCompound>("dyes").Select(ItemIO.Load).ToList().CopyTo(dyes);
			hideVisual = tag.GetByte("hideVisual");
		}

		public override void LoadLegacy(BinaryReader r) {
			r.ReadByte();//version

			foreach (var item in items)
				ItemIO.LoadLegacy(item, r);

			foreach (var item in dyes)
				ItemIO.LoadLegacy(item, r);

			hideVisual = r.ReadByte();
		}

		/// <summary>
		/// Should we prevent the item being put into the slot?
		/// </summary>
		internal bool AccCheck(Item item, int slot) {
			if (handlingEquip) {
				if (item.type > 0 && GetHandler(item) == null)
					return true;

				if (Incompatible(items[slot], item, false))//if the item is incompatible with this slot, it's fine to swap it
					return false;
			}

			return items.Any(i => Incompatible(i, item, slot >= 10));
		}

		/// <summary>
		/// Implements fast equip
		/// </summary>
		private bool ArmorSwap(ref Item item) {
			var handler = GetHandler(item);
			if (handler == null)
				return false;

			//incompatible utility swap
			for (int i = 0; i < SlotCount; i++) {
				if (Incompatible(items[i], item, false)) {
					InvUtils.Swap(ref items[i], ref item);
					return true;
				}
			}

			//incompatible with main equipment item
			for (int i = 0; i < SlotCount; i++)
				if (Incompatible(item, player.armor[3 + i], false) ||
					Incompatible(item, player.armor[13 + i], true))
					return false;

			//free slot for a partial utility accessory in the main equipment slots
			if (!handler.FullyFunctional && Enumerable.Range(3, SlotCount).Any(i => player.armor[i].type <= 0))
				return false;

			//insert into first empty slot
			for (int i = 0; i < SlotCount; i++) {
				if (items[i].type == 0) {
					InvUtils.Swap(ref items[i], ref item);
					return true;
				}
			}

			//no empty slots, swap with first
			if (handler.FullyFunctional) {
				InvUtils.Swap(ref items[0], ref item);
				return true;
			}

			return false;
		}

		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
			for (int i = 0; i < items.Length; i++) {
				var item = items[i];
				if (item.type > 0)
					GetHandler(item).ApplyEffect(player, hideVisual[i],
						ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
			}
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
			if (Main.myPlayer == player.whoAmI && player.difficulty > 0) {
				InvUtils.DropItems(player.position + player.Size/2, items);
				InvUtils.DropItems(player.position + player.Size/2, dyes);
			}
		}

		private void HandleVisualTick(int slot) {
			player.mouseInterface = true;
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				hideVisual[slot] = !hideVisual[slot];
				Main.PlaySound(12);
			}
			Main.HoverItem = new Item();
			Main.hoverItemName = Lang.inter[hideVisual[slot] ? 60 : 59].Value;
		}

		private void HandleEquip(int slot) {
			player.mouseInterface = true;

			if (slot >= SlotCount && Main.mouseItem.type > 0)
				return;

			handlingEquip = true;
			ItemSlot.OverrideHover(items, ItemSlot.Context.EquipAccessory, slot);
			if (Main.mouseLeftRelease && Main.mouseLeft)
			{
				ItemSlot.LeftClick(items, ItemSlot.Context.EquipAccessory, slot);
				Recipe.FindRecipes();
			}
			else
			{
				ItemSlot.RightClick(items, ItemSlot.Context.EquipAccessory, slot);
			}
			handlingEquip = false;

			if (items[slot].type > 0) {
				Main.HoverItem = items[slot].Clone();
				Main.hoverItemName = items[slot].Name;
				utilitySlotHover = true;
			}
			else {
				Main.hoverItemName = "Utility Accessory";
			}
		}

		private void HandleDye(int slot) {
			player.mouseInterface = true;
			if (slot >= SlotCount && Main.mouseItem.type > 0)
				return;

			ItemSlot.Handle(dyes, ItemSlot.Context.EquipDye, slot);
		}

		private void Draw(SpriteBatch sb, int mH) {
			utilitySlotHover = false;

			Main.inventoryScale = 0.85f;
			var mousePoint = new Point(Main.mouseX, Main.mouseY);

			var slotTex = UtilitySlots.Instance.GetTexture("assets/background");
			var slotBgTex = UtilitySlots.Instance.GetTexture("assets/slot_bg_utility");
			var slotSize = (int)(slotTex.Width*Main.inventoryScale);
			var x = Main.screenWidth - 92;
			var y = mH + 174;

			int numToDraw = SlotCount;
			if (numToDraw == 5 && items[5].type > 0 || dyes[5].type > 0)
				numToDraw++;

			var defaultInventoryBack = Main.inventoryBack;
			for (int i = 0; i < numToDraw; i++) {
				var r = new Rectangle(x, y + 47 * i, slotSize, slotSize);

				var visualTex = Main.inventoryTickOnTexture;
				var visualRect = new Rectangle(r.Right - 10, r.Top - 2, visualTex.Width, visualTex.Height);

				if (visualRect.Contains(mousePoint))
					HandleVisualTick(i);
				else if (r.Contains(mousePoint))
					HandleEquip(i);

				if (i >= SlotCount)
					Main.inventoryBack = new Color(80, 80, 80, 80);

				InvUtils.DrawSlot(sb, r.TopLeft(), slotTex, items[i], slotBgTex);

				if (hideVisual[i])
					visualTex = Main.inventoryTickOffTexture;
				sb.Draw(visualTex, visualRect.TopLeft(), Color.White * 0.7f);

				//dye
				r.X -= 47;
				if (r.Contains(mousePoint))
					HandleDye(i);

				ItemSlot.Draw(sb, dyes, ItemSlot.Context.EquipDye, i, r.TopLeft());
			}
			Main.inventoryBack = defaultInventoryBack;
		}

		internal void ModifyTooltips(Item item, List<TooltipLine> tooltip) {
			if (utilitySlotHover && item.type > 0)
				GetHandler(item).ModifyTooltip(tooltip);
		}


		public override void FrameEffects() {
			//act as if they're before regular equipment slots
			shaderCache = new PlayerDrawInfo();
			var freeHandon = player.handon <= 0;
			var freeHandoff = player.handoff <= 0;
			var freeBack = player.back <= 0;
			var freeFront = player.front <= 0;
			var freeShoe = player.shoe <= 0;
			var freeWaist = player.waist <= 0;
			var freeNeck = player.neck <= 0;
			var freeFace = player.face <= 0;
			var freeBallon = player.balloon <= 0;

			for (int i = 0; i < SlotCount; i++) {
				var item = items[i];

				if ((player.shield > 0 && item.frontSlot >= 1 && item.frontSlot <= 4) ||
					(player.front >= 1 && player.front <= 4 && item.shieldSlot > 0)) continue;

				if (item.wingSlot > 0 && player.wings == item.wingSlot &&
						(!hideVisual[i] || (player.velocity.Y != 0f && !player.mount.Active))) {
					player.wings = item.wingSlot;
					shaderCache.wingShader = dyes[i].dye;
				}

				if (hideVisual[i])
					continue;

				if (freeHandon && item.handOnSlot > 0) {
					player.handon = item.handOnSlot;
					shaderCache.handOnShader = dyes[i].dye;
				}
				if (freeHandoff && item.handOffSlot > 0) {
					player.handoff = item.handOffSlot;
					shaderCache.handOffShader = dyes[i].dye;
				}
				//ignore the fact that back items after front items reset front items cause it seems dumb
				if (freeBack && item.backSlot > 0) {
					player.back = item.backSlot;
					shaderCache.backShader = dyes[i].dye;
				}
				if (freeFront && item.frontSlot > 0) {
					player.front = item.frontSlot;
					shaderCache.frontShader = dyes[i].dye;
				}
				if (freeShoe && item.shoeSlot > 0) {
					player.shoe = item.shoeSlot;
					shaderCache.shoeShader = dyes[i].dye;
				}
				if (freeWaist && item.waistSlot > 0) {
					player.waist = item.waistSlot;
					shaderCache.waistShader = dyes[i].dye;
				}
				if (freeNeck && item.neckSlot > 0) {
					player.neck = item.neckSlot;
					shaderCache.neckShader = dyes[i].dye;
				}
				if (freeFace && item.faceSlot > 0) {
					player.face = item.faceSlot;
					shaderCache.faceShader = dyes[i].dye;
				}
				if (freeBallon && item.balloonSlot > 0) {
					player.balloon = item.balloonSlot;
					shaderCache.balloonShader = dyes[i].dye;
				}
			}
		}

		public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) {
			if (shaderCache.wingShader > 0) drawInfo.wingShader = shaderCache.wingShader;
			if (shaderCache.handOnShader > 0) drawInfo.handOnShader = shaderCache.handOnShader;
			if (shaderCache.handOffShader > 0) drawInfo.handOffShader = shaderCache.handOffShader;
			if (shaderCache.backShader > 0) drawInfo.backShader = shaderCache.backShader;
			if (shaderCache.frontShader > 0) drawInfo.frontShader = shaderCache.frontShader;
			if (shaderCache.shoeShader > 0) drawInfo.shoeShader = shaderCache.shoeShader;
			if (shaderCache.waistShader > 0) drawInfo.waistShader = shaderCache.waistShader;
			if (shaderCache.neckShader > 0) drawInfo.neckShader = shaderCache.neckShader;
			if (shaderCache.faceShader > 0) drawInfo.faceShader = shaderCache.faceShader;
			if (shaderCache.balloonShader > 0) drawInfo.balloonShader = shaderCache.balloonShader;
		}

		#region Hooks
		private static void DrawInventory(int mH) {
			Main.player[Main.myPlayer].UtilityInv().Draw(Main.spriteBatch, mH);
		}

		/// <summary>
		/// </summary>
		/// <param name="pos">Page icon drawing position</param>
		/// <returns>true if the page icon is under the mouse</returns>
		private static bool DrawPageIcons(Vector2 pos) {
			pos.X += 92f;
			pos.Y += 2f;
			float scale = 0.8f;

			var tex = UtilitySlots.Instance.GetTexture("assets/btn_ua_"+(Main.EquipPage == EquipPage ? 1 : 0));
			bool highlight = false;
			if (Collision.CheckAABBvAABBCollision(pos, tex.Size(), new Vector2(Main.mouseX, Main.mouseY), Vector2.One) &&
					(Main.mouseItem.stack < 1 || GetHandler(Main.mouseItem) != null || Main.mouseItem.dye > 0)) {
				highlight = true;
				Main.spriteBatch.Draw(UtilitySlots.Instance.GetTexture("assets/btn_ua_2"),
					pos, null, Main.OurFavoriteColor, 0f, new Vector2(2f), scale, SpriteEffects.None, 0f);
			}
			Main.spriteBatch.Draw(tex, pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			return highlight;
		}

		/// <summary>
		/// Resets Main.EquipPage to Main.EquipPageSelected for utility items if selected page is utility
		/// </summary>
		private static void ItemEquipPage() {
			if (GetHandler(Main.mouseItem) != null && Main.EquipPageSelected == EquipPage)
				Main.EquipPage = EquipPage;
		}

		private static bool ArmorSwapHook(Player player, Item[] inv, int slot) {
			if (!player.UtilityInv().ArmorSwap(ref inv[slot]))
				return false;

			Main.PlaySound(7);
			Recipe.FindRecipes();
			Main.EquipPageSelected = EquipPage;
			AchievementsHelper.HandleOnEquip(player, inv[slot], ItemSlot.Context.EquipAccessory);
			return true;
		}

		internal static void Hook() {
			IL.Terraria.Main.DrawInventory += HookDrawInventory;
			IL.Terraria.Main.DrawPageIcons += HookDrawPageIcons;
			IL.Terraria.UI.ItemSlot.SwapEquip_ItemArray_int_int += HookArmorSwap;
		}

		private static void HookDrawInventory(ILContext il) {
			var c = new ILCursor(il);

			//else if (Main.EquipPage == UtilityInventory.EquipPage) UtilityInventory.DrawInventory(Main.mH)
			//before else if (Main.EquipPage == 1)
			c.GotoNext(MoveType.AfterLabel,
				i => i.MatchLdsfld<Main>(nameof(Main.EquipPage)),
				i => i.MatchLdcI4(1));

			var endIfLabel = c.Prev.Operand; //br endIf
			var elseIfLabel = c.DefineLabel();
			c.Emit(Ldsfld, typeof(Main).GetField(nameof(Main.EquipPage)));
            c.Emit(Ldc_I4, EquipPage);
            c.Emit(Bne_Un, elseIfLabel);
            c.Emit(Ldsfld, typeof(Main).GetField("mH", BindingFlags.NonPublic | BindingFlags.Static));
			c.EmitDelegate<Action<int>>(DrawInventory);
            c.Emit(Br, endIfLabel);
			c.MarkLabel(elseIfLabel);
		}

		private static void HookDrawPageIcons(ILContext il) {
			var c = new ILCursor(il);

			//after Vector2 vector = new Vector2((float)(screenWidth - 162), (float)(142 + mH));
			//if(UtilityInventory.DrawPageIcons(vector)) num = UtilityInventory.EquipPage;
			var newVec2 = typeof(Vector2).GetConstructor(new[] {typeof(float), typeof(float)});
			c.GotoNext(MoveType.After, i => i.MatchCall(newVec2));

			c.Emit(Ldloc, 1);//vector
			c.EmitDelegate<Func<Vector2, bool>>(DrawPageIcons);
			var endIfLabel = c.DefineLabel();
			c.Emit(Brfalse, endIfLabel);
			c.Emit(Ldc_I4, EquipPage);
			c.Emit(Stloc, 0);//num
			c.MarkLabel(endIfLabel);

			//vector.X += 82 -> 52
			if (c.TryGotoNext(i => i.MatchLdcR4(82f)))
				c.Next.Operand = 52f;

			//vector.X -= 48 -> 40
			while (c.TryGotoNext(i => i.MatchLdcR4(48f)))
				c.Next.Operand = 40f;

			//UtilityInventory.ItemEquipPage();
			//before return num;
			c.GotoNext(i => i.MatchRet());
			c.EmitDelegate<Action>(ItemEquipPage);
		}

		private static void HookArmorSwap(ILContext il) {
			var c = new ILCursor(il);

			//else if (UtilityInventory.ArmorSwapHook(player, inv, slot)) {}
			//before else if (Main.projHook[inv[slot].shoot])
			c.GotoNext(MoveType.AfterLabel, i => i.MatchLdsfld<Main>(nameof(Main.projHook)));

			var endIfLabel = c.Prev.Operand; //br endIf
			c.Emit(Ldloc, 0);//player
			c.Emit(Ldarg, 0);//inv
			c.Emit(Ldarg, 2);//slot
			c.EmitDelegate<Func<Player, Item[], int, bool>>(ArmorSwapHook);
			c.Emit(Brtrue, endIfLabel);
		}
		#endregion
	}
}