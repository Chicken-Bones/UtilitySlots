using System;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using static Mono.Cecil.Cil.OpCodes;
using Terraria.ModLoader;
using System.Linq;
using Terraria.GameContent;
using Terraria.Localization;

namespace UtilitySlots
{
    internal class UtilitySlotsEquipPage : ILoadable
    {
        private Asset<Texture2D>[] buttonTextures;

        private const int EquipPage = 4;

        public static bool IsSelected => Main.EquipPage == EquipPage;

        public void Load(Mod mod) {
            buttonTextures = Enumerable.Range(0, 3).Select(i => mod.Assets.Request<Texture2D>("assets/btn_ua_" + i)).ToArray();
            IL.Terraria.Main.DrawPageIcons += HookDrawPageIcons;
            On.Terraria.UI.ItemSlot.AccessorySwap += TrackUtilitySlotQuickSwap;
            On.Terraria.UI.ItemSlot.SwapEquip_ItemArray_int_int += ApplyUtilitySlotQuickSwap;
        }

        private static bool SelectEquipPageFromQuickSwap;

        private bool TrackUtilitySlotQuickSwap(On.Terraria.UI.ItemSlot.orig_AccessorySwap orig, Player player, Item item, ref Item result) {
            var preSwapItems = ContentInstance<UtilitySlot>.Instances.Select(slot => slot.FunctionalItem).ToArray();
            bool swapped = orig(player, item, ref result);
            if (!swapped)
                return false;

            var postSwapItems = ContentInstance<UtilitySlot>.Instances.Select(slot => slot.FunctionalItem).ToArray();
            if (!Enumerable.SequenceEqual(preSwapItems, postSwapItems))
                SelectEquipPageFromQuickSwap = true; // quick swapped into our slot, show our equip page!

            return true;
        }

        private void ApplyUtilitySlotQuickSwap(On.Terraria.UI.ItemSlot.orig_SwapEquip_ItemArray_int_int orig, Item[] inv, int context, int slot) {
            orig(inv, context, slot);
            if (SelectEquipPageFromQuickSwap) {
                SelectEquipPageFromQuickSwap = false;
                Main.EquipPageSelected = EquipPage;
            }
        }

        public void Unload() {}

		internal static void DrawPartiallyFunctionalAccDetails(Vector2 position) {
			float itemScale = Main.inventoryScale * 0.7f;
			float itemSpacing = 40 * itemScale;

			position += new Vector2(18, 68) * Main.inventoryScale;

			foreach (var item in ContentInstance<UtilitySlot>.Instances.Select(slot => slot.FunctionalItem)) {
				var h = UtilityAccessories.GetHandler(item);
				if (h == null) continue;

				var hint = h.PartiallyFunctionalHintKey;
				if (hint == null) continue;

				if (DrawItem(item, position, itemScale, Color.White)) {
					Main.HoverItem = new Item();
					Main.hoverItemName = Language.GetTextValue(hint);
				}
				position.X -= itemSpacing;
			}
		}

		private static bool DrawItem(Item item, Vector2 position, float scale, Color color) {
			Main.instance.LoadItem(item.type);
			Texture2D tex = TextureAssets.Item[item.type].Value;
			Rectangle region = (Main.itemAnimations[item.type] == null) ? tex.Frame() : Main.itemAnimations[item.type].GetFrame(tex);

			var nominalSize = 32f;
			if (region.Width > nominalSize || region.Height > nominalSize)
				scale *= MathF.Min(nominalSize / region.Height, nominalSize / region.Width);

			position += new Vector2(nominalSize, nominalSize) * scale / 2f;
			var origin = region.Size() / 2f;

			Main.spriteBatch.Draw(tex, position, region, item.GetAlpha(color), 0f, origin, scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(tex, position, region, item.GetColor(color), 0f, origin, scale, SpriteEffects.None, 0f);

			return Collision.CheckAABBvAABBCollision(position - region.Size() * scale / 2f, region.Size() * scale, Main.MouseScreen, Vector2.One);
		}

		/// <summary>
		/// </summary>
		/// <param name="pos">Page icon drawing position</param>
		/// <returns>true if the page icon is under the mouse</returns>
		private bool DrawPageIcons(Vector2 pos) {
            pos.X += 92f;
            pos.Y += 2f;
            float scale = 0.8f;

            var tex = buttonTextures[Main.EquipPage == EquipPage ? 1 : 0].Value;
            bool highlight = false;
            if (Collision.CheckAABBvAABBCollision(pos, tex.Size(), Main.MouseScreen, Vector2.One) &&
                    (Main.mouseItem.stack < 1 || UtilityAccessories.GetHandler(Main.mouseItem) != null || Main.mouseItem.dye > 0)) {
                highlight = true;
                Main.spriteBatch.Draw(buttonTextures[2].Value, pos, null, Main.OurFavoriteColor, 0f, new Vector2(2f), scale, SpriteEffects.None, 0f);
				Main.HoverItem = new Item();
				Main.hoverItemName = Language.GetTextValue("Mods.UtilitySlots.PageName");
			}
            Main.spriteBatch.Draw(tex, pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            return highlight;
        }

        /// <summary>
        /// Resets Main.EquipPage to Main.EquipPageSelected for utility items if selected page is utility
        /// </summary>
        private void ItemEquipPage() {
            if (UtilityAccessories.GetHandler(Main.mouseItem) != null && Main.EquipPageSelected == EquipPage)
                Main.EquipPage = EquipPage;
        }

        private void HookDrawPageIcons(ILContext il) {
            var c = new ILCursor(il);

            //after Vector2 vector = new Vector2(screenWidth - 162, yPos);
            //if(UtilityInventory.DrawPageIcons(vector)) num = UtilityInventory.EquipPage;
            var newVec2 = typeof(Vector2).GetConstructor(new[] { typeof(float), typeof(float) });
            c.GotoNext(MoveType.After, i => i.MatchCall(newVec2));

            c.Emit(Ldloc, 1);//vector
            c.EmitDelegate(DrawPageIcons);
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
            c.EmitDelegate(ItemEquipPage);
        }
    }
}
