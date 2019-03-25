using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ID.ItemID;

namespace UtilitySlots
{
	public static class UtilityAccessories
	{
		public abstract class Handler
		{
			public IList<string> negatedTipLines = new List<string>();

			public virtual bool FullyFunctional => false;

			public abstract void ApplyEffect(Player p, bool hideVisual,
				ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff);

			public virtual void ModifyTooltip(List<TooltipLine> tooltip) {
				foreach (var line in tooltip) {
					if (line.isModifier) {
						line.overrideColor = Color.Gray;
						line.text = "Modifiers have no effect";
					}
					if (negatedTipLines.Contains(line.Name))
						line.overrideColor = Color.Gray;
				}
			}

			public Handler NegateTip(params string[] lines) {
				negatedTipLines = lines;
				return this;
			}
		}

		public class SingleTypeHandler : Handler
		{
			protected readonly Item item;

			public SingleTypeHandler(int itemId) {
				item = InvUtils.Default(itemId, true);
			}

			public override bool FullyFunctional => true;

			public override void ApplyEffect(Player p, bool hideVisual,
					ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
				p.VanillaUpdateEquip(item);
				p.VanillaUpdateAccessory(p.whoAmI, item, hideVisual, ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
			}
		}

		public class EquivalentHandler : Handler
		{
			public IList<SingleTypeHandler> subHandlers;

			public EquivalentHandler(params int[] types) {
				subHandlers = types.Select(type => new SingleTypeHandler(type)).ToList();
			}

			public override void ApplyEffect(Player p, bool hideVisual, ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
				foreach (var handler in subHandlers)
					handler.ApplyEffect(p, hideVisual, ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
			}
		}

		private static List<Func<Item, Handler>> providers = new List<Func<Item, Handler>>();
		private static Dictionary<int, Handler> itemIdHandlers = new Dictionary<int, Handler>();

		public static Handler GetHandler(Item item) =>
			providers.Select(provider => provider(item)).FirstOrDefault(handler => handler != null);

		public static void AddProvider(Func<Item, Handler> provider) => providers.Add(provider);

		public static void AddHandler(int itemId, Handler handler) => itemIdHandlers[itemId] = handler;

		public static void AddDefaultHandler(params int[] itemIds) {
			foreach (var itemId in itemIds)
				AddHandler(itemId, new SingleTypeHandler(itemId));
		}

		internal static void Load() {
			// default handler provider
			AddProvider(item => {
				itemIdHandlers.TryGetValue(item.type, out var h);
				return h;
			});

			// wing provider
			AddProvider(item => item.wingSlot > 0 ? new SingleTypeHandler(item.type) : null);

			AddDefaultHandler(
				CloudinaBottle, SandstorminaBottle, BlizzardinaBottle, FartinaJar, TsunamiInABottle,

				ShinyRedBalloon, BalloonPufferfish,
				CloudinaBalloon, SandstorminaBalloon, BlizzardinaBalloon,
				FartInABalloon, HoneyBalloon, SharkronBalloon,
				BundleofBalloons,

				LuckyHorseshoe, ObsidianHorseshoe,
				BlueHorseshoeBalloon, WhiteHorseshoeBalloon, YellowHorseshoeBalloon,
				BalloonHorseshoeFart, BalloonHorseshoeHoney, BalloonHorseshoeSharkron,

				Aglet, AnkletoftheWind,
				FrogLeg, FlyingCarpet,

				IceSkates, HermesBoots, FlurryBoots, SailfishBoots, RocketBoots,
				SpectreBoots, LightningBoots, FrostsparkBoots,
				WaterWalkingBoots, ObsidianWaterWalkingBoots, LavaWaders,
				FlowerBoots,

				ClimbingClaws, ShoeSpikes, TigerClimbingGear,
				Tabi, BlackBelt, MasterNinjaGear,
				Flipper, JellyfishNecklace, DivingGear, DivingHelmet, JellyfishDivingGear, ArcticDivingGear,

				HighTestFishingLine, AnglerEarring, TackleBox, AnglerTackleBag,

				GoldRing, DiscountCard, CoinRing, GreedyRing,
				LaserRuler, CordageGuide,
				Toolbox, BrickLayer, ExtendoGrip, PaintSprayer, PortableCementMixer, Toolbelt, ArchitectGizmoPack);

			AddDefaultHandler(MusicBox, MusicBoxTitle,
				MusicBoxOverworldDay, MusicBoxAltOverworldDay, MusicBoxNight,
				MusicBoxRain, MusicBoxSnow, MusicBoxIce,
				MusicBoxDesert, MusicBoxOcean, MusicBoxSpace,
				MusicBoxUnderground, MusicBoxAltUnderground, MusicBoxMushrooms, MusicBoxJungle,
				MusicBoxCorruption, MusicBoxUndergroundCorruption,
				MusicBoxCrimson, MusicBoxUndergroundCrimson,
				MusicBoxTheHallow, MusicBoxUndergroundHallow,
				MusicBoxHell, MusicBoxDungeon, MusicBoxTemple,
				MusicBoxBoss1, MusicBoxBoss2, MusicBoxBoss3, MusicBoxBoss4, MusicBoxBoss5,
				MusicBoxPlantera, MusicBoxEerie, MusicBoxEclipse,
				MusicBoxGoblins, MusicBoxPirates, MusicBoxMartians,
				MusicBoxPumpkinMoon, MusicBoxFrostMoon,
				MusicBoxTowers, MusicBoxLunarBoss);

			AddHandler(MasterNinjaGear, new EquivalentHandler(TigerClimbingGear, Tabi).NegateTip("Tooltip1"));
			AddHandler(ObsidianWaterWalkingBoots, new EquivalentHandler(WaterWalkingBoots).NegateTip("Tooltip1"));
			AddHandler(ObsidianHorseshoe, new EquivalentHandler(LuckyHorseshoe).NegateTip("Tooltip1"));
			AddHandler(HoneyBalloon, new EquivalentHandler(ShinyRedBalloon).NegateTip("Tooltip1"));
			AddHandler(BalloonHorseshoeHoney, new EquivalentHandler(ShinyRedBalloon, LuckyHorseshoe).NegateTip("Tooltip0"));
			AddHandler(CoinRing, new EquivalentHandler(GoldRing).NegateTip("Tooltip1"));
			AddHandler(GreedyRing, new EquivalentHandler(DiscountCard, GoldRing).NegateTip("Tooltip1"));

			AddHandler(LavaWaders, new LavaWaderHandler().NegateTip("Tooltip1"));
		}

		internal static void Unload() {
			providers.Clear();
			itemIdHandlers.Clear();
		}
	}

	internal class LavaWaderHandler : UtilityAccessories.Handler
	{
		public override void ApplyEffect(Player p, bool hideVisual,
				ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
			p.waterWalk = true;
		}
	}
}
