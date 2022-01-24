using Terraria.ModLoader;
using static UtilitySlots.UtilityAccessories;
using static Terraria.ID.ItemID;
using UtilitySlots.Handlers;
using Terraria;
using System.Collections.Generic;

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
			AddUtilityAccessory(
				MusicBoxOverworldDay, MusicBoxEerie, MusicBoxNight, MusicBoxTitle, MusicBoxUnderground, MusicBoxBoss1, MusicBoxJungle, MusicBoxCorruption,
				MusicBoxUndergroundCorruption, MusicBoxTheHallow, MusicBoxBoss2, MusicBoxUndergroundHallow, MusicBoxBoss3, MusicBox, MusicBoxSnow, MusicBoxSpace,
				MusicBoxCrimson, MusicBoxBoss4, MusicBoxAltOverworldDay, MusicBoxRain, MusicBoxIce, MusicBoxDesert, MusicBoxOcean, MusicBoxDungeon, MusicBoxPlantera,
				MusicBoxBoss5, MusicBoxTemple, MusicBoxEclipse, MusicBoxMushrooms, MusicBoxPumpkinMoon, MusicBoxAltUnderground, MusicBoxFrostMoon, MusicWallpaper,
				MusicBoxUndergroundCrimson, MusicBoxLunarBoss, MusicBoxMartians, MusicBoxPirates, MusicBoxHell, MusicBoxTowers, MusicBoxGoblins, MusicBoxSandstorm,
				MusicBoxDD2, MusicBoxOceanAlt, MusicBoxSlimeRain, MusicBoxSpaceAlt, MusicBoxTownDay, MusicBoxTownNight, MusicBoxWindyDay, MusicBoxDayRemix, MusicBoxTitleAlt,
				MusicBoxStorm, MusicBoxGraveyard, MusicBoxUndergroundJungle, MusicBoxJungleNight, MusicBoxQueenSlime, MusicBoxEmpressOfLight, MusicBoxDukeFishron, MusicBoxMorningRain,
				MusicBoxConsoleTitle, MusicBoxUndergroundDesert, MusicBoxOWRain, MusicBoxOWDay, MusicBoxOWNight, MusicBoxOWUnderground, MusicBoxOWDesert, MusicBoxOWOcean, MusicBoxOWMushroom,
				MusicBoxOWDungeon, MusicBoxOWSpace, MusicBoxOWUnderworld, MusicBoxOWSnow, MusicBoxOWCorruption, MusicBoxOWUndergroundCorruption, MusicBoxOWCrimson, MusicBoxOWUndergroundCrimson,
				MusicBoxOWUndergroundSnow, MusicBoxOWUndergroundHallow, MusicBoxOWBloodMoon, MusicBoxOWBoss2, MusicBoxOWBoss1, MusicBoxOWInvasion, MusicBoxOWTowers, MusicBoxOWMoonLord, MusicBoxOWPlantera,
				MusicBoxOWJungle, MusicBoxOWWallOfFlesh, MusicBoxOWHallow, MusicBoxCredits, MusicBoxDeerclops); 

			AddHandler(MasterNinjaGear, new EquivalentItemsHandler(TigerClimbingGear, Tabi).NegateTip("Tooltip1"));
			AddHandler(HoneyBalloon, new EquivalentItemsHandler(ShinyRedBalloon).NegateTip("Tooltip1"));
			AddHandler(BalloonHorseshoeHoney, new EquivalentItemsHandler(ShinyRedBalloon, LuckyHorseshoe).NegateTip("Tooltip0"));
			AddHandler(CoinRing, new EquivalentItemsHandler(GoldRing).NegateTip("Tooltip1"));
			AddHandler(GreedyRing, new EquivalentItemsHandler(DiscountCard, GoldRing).NegateTip("Tooltip1"));

			// Because of TerraSpark Boots, we've decided to allow lava and fire immunity post hardmode.
			// Lava waders combine the effects of Obsidian Skull, Lava Charm, Water Walking Boots, and Obsidian Rose
			// Lava Charm, Obsidian Skull and Obsidian Horseshoe will also activate post-hardmode.
			// Other obsidian and magma related accessories will not be permitted in utility slots, because their primary uses are combat related
			AddHardmodeLavaAccHandler(ObsidianHorseshoe, new EquivalentItemsHandler(LuckyHorseshoe).NegateTip("Tooltip1"));
			AddHardmodeLavaAccHandler(ObsidianWaterWalkingBoots, new EquivalentItemsHandler(WaterWalkingBoots).NegateTip("Tooltip1"));
			AddHardmodeLavaAccHandler(LavaWaders, new PreHardmodeLavaWaderHandler().NegateTip("Tooltip1", "Tooltip2"));
			AddHardmodeLavaAccHandler(TerrasparkBoots, new EquivalentItemsHandler(FrostsparkBoots, LavaWaders).NegateTip("Tooltip3", "Tooltip4"));

			void AddHardmodeLavaAccHandler(int itemId, UtilityItemHandler preHardmodeHandler) =>
				AddHandler(itemId, new HardmodeDependentHandler(preHardmodeHandler, new FullyFunctionalHandler(itemId)));
		}

		public void Unload() { }

		private class HardmodeDependentHandler : UtilityItemHandler
		{
			private readonly UtilityItemHandler preHardmode;
			private readonly UtilityItemHandler postHardmode;

			public UtilityItemHandler ActiveHandler => Main.hardMode ? postHardmode : preHardmode;

			public HardmodeDependentHandler(UtilityItemHandler preHardmode, UtilityItemHandler postHardmode) {
				this.preHardmode = preHardmode;
				this.postHardmode = postHardmode;
			}

			public override bool FullyFunctional => ActiveHandler.FullyFunctional;

			public override void ApplyEffect(Player p, bool hideVisual) => ActiveHandler.ApplyEffect(p, hideVisual);

			public override void ModifyTooltip(List<TooltipLine> tooltip) => ActiveHandler.ModifyTooltip(tooltip);
		}

		// grants the ability to walk on lava as well as water, but not any other immunity
		private class PreHardmodeLavaWaderHandler : UtilityItemHandler
		{
			public override void ApplyEffect(Player p, bool hideVisual) => p.waterWalk = true;
		}
	}
}
