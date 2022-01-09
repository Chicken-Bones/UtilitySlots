using Terraria.ModLoader;
using static UtilitySlots.UtilityAccessories;
using static Terraria.ID.ItemID;
using UtilitySlots.Handlers;
using Terraria;

namespace UtilitySlots
{
    internal class VanillaUtilityAccessories : ILoadable
    {
        public void Load(Mod mod) {
			AddUtilityAccessory(
				CloudinaBottle, SandstorminaBottle, BlizzardinaBottle, FartinaJar, TsunamiInABottle,

				ShinyRedBalloon, BalloonPufferfish,
				CloudinaBalloon, SandstorminaBalloon, BlizzardinaBalloon,
				FartInABalloon, HoneyBalloon, SharkronBalloon,
				BundleofBalloons,

				LuckyHorseshoe, ObsidianHorseshoe,
				BlueHorseshoeBalloon, WhiteHorseshoeBalloon, YellowHorseshoeBalloon,
				BalloonHorseshoeFart, BalloonHorseshoeHoney, BalloonHorseshoeSharkron,

				Aglet, AnkletoftheWind, Magiluminescence,
				FrogLeg, FlyingCarpet,				

				IceSkates, HermesBoots, FlurryBoots, SailfishBoots, SandBoots, RocketBoots,
				SpectreBoots, LightningBoots, FrostsparkBoots, AmphibianBoots, 
				WaterWalkingBoots, ObsidianWaterWalkingBoots, LavaWaders,
				FlowerBoots, FairyBoots,

				EmpressFlightBooster, GravityGlobe, 

				ClimbingClaws, ShoeSpikes, TigerClimbingGear,
				Tabi, BlackBelt, MasterNinjaGear,
				Flipper, FrogFlipper, FrogWebbing, FrogGear,
				FloatingTube, JellyfishNecklace, DivingGear, DivingHelmet, JellyfishDivingGear, ArcticDivingGear, NeptunesShell,

				HighTestFishingLine, AnglerEarring, TackleBox, AnglerTackleBag, LavaFishingHook, LavaproofTackleBag,

				GoldRing, DiscountCard, CoinRing, GreedyRing, TreasureMagnet,
				PortableStool, GolfBall, CordageGuide, DontStarveShaderItem, 
				LaserRuler, SpectreGoggles, MechanicalLens,
				Toolbox, BrickLayer, ExtendoGrip, PaintSprayer, PortableCementMixer, Toolbelt, ArchitectGizmoPack, AncientChisel, ActuationAccessory,

				CopperWatch, TinWatch, SilverWatch, TungstenWatch, GoldWatch, PlatinumWatch,
				Compass, DepthMeter, GPS, FishermansGuide, WeatherRadio, Sextant, FishFinder, MetalDetector, Stopwatch, DPSMeter, GoblinTech, TallyCounter, LifeformAnalyzer, Radar, REK, PDA
				);


			// TODO: I think there's a way to detect music boxes
			AddUtilityAccessory(MusicBox, MusicBoxTitle,
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

			// TODO, TerraSpark Boots, Moon Shell, Hellfire Treads

			AddHandler(MasterNinjaGear, new EquivalentItemsHandler(TigerClimbingGear, Tabi).NegateTip("Tooltip1"));
			AddHandler(ObsidianWaterWalkingBoots, new EquivalentItemsHandler(WaterWalkingBoots).NegateTip("Tooltip1"));
			AddHandler(ObsidianHorseshoe, new EquivalentItemsHandler(LuckyHorseshoe).NegateTip("Tooltip1"));
			AddHandler(HoneyBalloon, new EquivalentItemsHandler(ShinyRedBalloon).NegateTip("Tooltip1"));
			AddHandler(BalloonHorseshoeHoney, new EquivalentItemsHandler(ShinyRedBalloon, LuckyHorseshoe).NegateTip("Tooltip0"));
			AddHandler(CoinRing, new EquivalentItemsHandler(GoldRing).NegateTip("Tooltip1"));
			AddHandler(GreedyRing, new EquivalentItemsHandler(DiscountCard, GoldRing).NegateTip("Tooltip1"));

			AddHandler(LavaWaders, new LavaWaderHandler().NegateTip("Tooltip1"));
		}

		public void Unload() { }

		private class LavaWaderHandler : UtilityItemHandler
		{
			public override void ApplyEffect(Player p, bool hideVisual) => p.waterWalk = true;
		}
	}
}
