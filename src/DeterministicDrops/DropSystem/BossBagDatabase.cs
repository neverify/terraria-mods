using System;
using System.Collections.Generic;
using Terraria.ID;

namespace DeterministicDrops.DropSystem;

internal static class BossBagDatabase
{
    public static DropContext[] GetDropContexts(int bossBagId)
    => s_bossBags.TryGetValue(bossBagId, out var contexts) ? contexts : [];

    public static DropContext[] GetExtraDrops(int itemId)
    => s_extraDrops.TryGetValue(itemId, out var contexts) ? contexts : [];

    public static CoinAmount GetCoinAmount(int bossBagId)
    {
        s_coinAmounts.TryGetValue(bossBagId, out var amount);

        int value = (int)(amount.Value * Math.Pow(1.015, 4));
        return new CoinAmount(value);
    }

    private static DropContext[] DevSets() => [
        new(s_devSets, chanceDenominator: 8, dropCondition: DropProcessor.DropCondition.CelebrationMK10),
        new(s_devSets, chanceDenominator: 16, dropCondition: DropProcessor.DropCondition.NotCelebrationMK10),
    ];

    private static readonly short[][] s_devSets = [
        [ItemID.AaronsBreastplate, ItemID.AaronsLeggings, ItemID.AaronsHelmet],
        [ItemID.ArkhalisHat, ItemID.ArkhalisShirt, ItemID.ArkhalisPants, ItemID.ArkhalisWings, ItemID.Arkhalis],
        [ItemID.CenxsTiara, ItemID.CenxsBreastplate, ItemID.CenxsLeggings, ItemID.CenxsWings],
        [ItemID.CenxsTiara, ItemID.CenxsDress, ItemID.CenxsDressPants, ItemID.CenxsWings],
        [ItemID.CrownosMask, ItemID.CrownosBreastplate, ItemID.CrownosLeggings, ItemID.CrownosWings],
        [ItemID.DTownsHelmet, ItemID.DTownsBreastplate, ItemID.DTownsLeggings, ItemID.DTownsWings],
        [ItemID.JimsHelmet, ItemID.JimsBreastplate, ItemID.JimsLeggings, ItemID.JimsWings],
        [ItemID.BejeweledValkyrieHead, ItemID.BejeweledValkyrieBody, ItemID.BejeweledValkyrieWing, ItemID.ValkyrieYoyo],
        [ItemID.LeinforsHat, ItemID.LeinforsShirt, ItemID.LeinforsPants, ItemID.LeinforsAccessory, ItemID.LeinforsWings],
        [ItemID.LokisHelm, ItemID.LokisPants, ItemID.LokisDye, ItemID.LokisWings],
        [ItemID.RedsHelmet, ItemID.RedsBreastplate, ItemID.RedsLeggings, ItemID.RedsWings, ItemID.RedsYoyo],
        [ItemID.SkiphsHelm, ItemID.SkiphsShirt, ItemID.SkiphsPants, ItemID.DevDye, ItemID.SkiphsWings],
        [ItemID.WillsHelmet, ItemID.WillsBreastplate, ItemID.WillsLeggings, ItemID.WillsWings],
        [ItemID.Yoraiz0rHead, ItemID.Yoraiz0rShirt, ItemID.Yoraiz0rPants, ItemID.Yoraiz0rDarkness, ItemID.Yoraiz0rWings],
        [ItemID.GroxTheGreatHelm, ItemID.GroxTheGreatArmor, ItemID.GroxTheGreatGreaves, ItemID.GroxTheGreatWings],
        [ItemID.FoodBarbarianHelm, ItemID.FoodBarbarianArmor, ItemID.FoodBarbarianGreaves, ItemID.FoodBarbarianWings],
        [ItemID.SafemanSunHair, ItemID.SafemanSunDress, ItemID.SafemanDressLeggings, ItemID.SafemanWings],
        [ItemID.GhostarSkullPin, ItemID.GhostarShirt, ItemID.GhostarPants, ItemID.GhostarsWings],
        [ItemID.ChickenBonesHead, ItemID.ChickenBonesBody, ItemID.ChickenBonesLegs, ItemID.ChickenBonesRobe, ItemID.ChickenBonesWings],
        [ItemID.KazzymodusHood, ItemID.KazzymodusChestpiece, ItemID.KazzymodusLeggings, ItemID.KazzymodusWings],
        [ItemID.LunasHead, ItemID.LunasBody, ItemID.LunasLegs, ItemID.LunasCloak, ItemID.LunasWings],
    ];

    private static readonly Dictionary<int, DropContext[]> s_bossBags = new()
    {
        { ItemID.KingSlimeBossBag, [
            new DropContext(ItemID.RoyalGel),
            new DropContext(ItemID.Solidifier),
            new DropContext(ItemID.SlimySaddle, chanceDenominator: 2),
            new DropContext(ItemID.SlimeGun, chanceDenominator: 2),
            new DropContext(ItemID.SlimeHook, chanceDenominator: 2),
            new DropContext(ItemID.KingSlimeMask, chanceDenominator: 7),
            new DropContext(ItemID.SlimeStaff, chanceDenominator: 30),
            new DropContext([ItemID.NinjaHood, ItemID.NinjaShirt, ItemID.NinjaPants], dropAttemptCount: 2),
        ] },

        { ItemID.EyeOfCthulhuBossBag, [
            new DropContext(ItemID.EoCShield),
            new DropContext(ItemID.EyeMask, chanceDenominator: 7),
            new DropContext(ItemID.Binoculars, chanceDenominator: 30),
            new DropContext(ItemID.UnholyArrow, minDropAmount: 20, maxDropAmount: 50),
            new DropContext(ItemID.CrimtaneOre, dropCondition: DropProcessor.DropCondition.Crimson, minDropAmount: 30, maxDropAmount: 90),
            new DropContext(ItemID.CrimsonSeeds, dropCondition: DropProcessor.DropCondition.Crimson, minDropAmount: 1, maxDropAmount: 3),
            new DropContext(ItemID.DemoniteOre, dropCondition: DropProcessor.DropCondition.Corruption, minDropAmount: 30, maxDropAmount: 90),
            new DropContext(ItemID.CorruptSeeds, dropCondition: DropProcessor.DropCondition.Corruption, minDropAmount: 1, maxDropAmount: 3),
        ] },

        { ItemID.EaterOfWorldsBossBag, [
            new DropContext(ItemID.WormScarf),
            new DropContext(ItemID.EaterMask, chanceDenominator: 7),
            new DropContext(ItemID.EatersBone, chanceDenominator: 20),
            new DropContext(ItemID.DemoniteOre, minDropAmount: 80, maxDropAmount: 110, dropCondition: DropProcessor.DropCondition.Crimson | DropProcessor.DropCondition.NotMasterMode),
            new DropContext(ItemID.DemoniteOre, minDropAmount: 110, maxDropAmount: 135, dropCondition: DropProcessor.DropCondition.Crimson | DropProcessor.DropCondition.MasterMode),
            new DropContext(ItemID.ShadowScale, minDropAmount: 20, maxDropAmount: 40, dropCondition: DropProcessor.DropCondition.Corruption | DropProcessor.DropCondition.NotMasterMode),
            new DropContext(ItemID.ShadowScale, minDropAmount: 30, maxDropAmount: 50, dropCondition: DropProcessor.DropCondition.Corruption | DropProcessor.DropCondition.MasterMode),
        ] },

        { ItemID.BrainOfCthulhuBossBag, [
            new DropContext(ItemID.BrainOfConfusion),
            new DropContext(ItemID.BrainMask, chanceDenominator: 7),
            new DropContext(ItemID.BoneRattle, chanceDenominator: 20),
            new DropContext(ItemID.CrimtaneOre, minDropAmount: 80, maxDropAmount: 110, dropCondition: DropProcessor.DropCondition.Crimson | DropProcessor.DropCondition.NotMasterMode),
            new DropContext(ItemID.CrimtaneOre, minDropAmount: 110, maxDropAmount: 135, dropCondition: DropProcessor.DropCondition.Crimson | DropProcessor.DropCondition.MasterMode),
            new DropContext(ItemID.TissueSample, minDropAmount: 20, maxDropAmount: 40, dropCondition: DropProcessor.DropCondition.Corruption | DropProcessor.DropCondition.NotMasterMode),
            new DropContext(ItemID.TissueSample, minDropAmount: 30, maxDropAmount: 50, dropCondition: DropProcessor.DropCondition.Corruption | DropProcessor.DropCondition.MasterMode),
        ] },

        { ItemID.QueenBeeBossBag, [
            new DropContext(ItemID.HiveBackpack),
            new DropContext(ItemID.HiveWand),
            new DropContext(ItemID.HoneyComb, chanceDenominator: 3),
            new DropContext(ItemID.BeeMask, chanceDenominator: 7),
            new DropContext(ItemID.Nectar, chanceDenominator: 9),
            new DropContext(ItemID.QueenOfBees, chanceDenominator: 9),
            new DropContext(ItemID.HoneyedGoggles, chanceDenominator: 9),
            new DropContext(ItemID.Beenade, minDropAmount: 10, maxDropAmount: 29),
            new DropContext(ItemID.BeeWax, minDropAmount: 17, maxDropAmount: 30),
            new DropContext([ItemID.BeeGun, ItemID.BeeKeeper, ItemID.BeesKnees]),
            new DropContext([ItemID.BeeHat, ItemID.BeeShirt, ItemID.BeePants]),
        ] },

        { ItemID.DeerclopsBossBag, [
            new DropContext(ItemID.BoneHelm),
            new DropContext(ItemID.ChesterPetItem, chanceDenominator: 3),
            new DropContext(ItemID.Eyebrella, chanceDenominator: 3),
            new DropContext(ItemID.DontStarveShaderItem, chanceDenominator: 3),
            new DropContext(ItemID.DizzyHat, chanceDenominator: 14),
            new DropContext([ItemID.PewMaticHorn, ItemID.WeatherPain, ItemID.HoundiusShootius, ItemID.LucyTheAxe]),
        ] },

        { ItemID.SkeletronBossBag, [
            new DropContext(ItemID.BoneGlove),
            new DropContext([ItemID.SkeletronMask, ItemID.SkeletronHand, ItemID.BookofSkulls]),
        ] },

        { ItemID.WallOfFleshBossBag, [
            new DropContext(ItemID.DemonHeart, dropCondition: DropProcessor.DropCondition.NotDemonHeart),
            new DropContext(ItemID.Pwnhammer),
            new DropContext(ItemID.FleshMask, chanceDenominator: 7),
            new DropContext([ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem]),
            new DropContext([ItemID.BreakerBlade, ItemID.ClockworkAssaultRifle, ItemID.LaserRifle, ItemID.FireWhip]),
        ] },

        { ItemID.QueenSlimeBossBag, [
            new DropContext(ItemID.VolatileGelatin),
            new DropContext(ItemID.QueenSlimeMountSaddle, chanceDenominator: 2),
            new DropContext(ItemID.QueenSlimeHook, chanceDenominator: 2),
            new DropContext(ItemID.Smolstar, chanceDenominator: 3),
            new DropContext(ItemID.QueenSlimeMask, chanceDenominator: 7),
            new DropContext(ItemID.GelBalloon, minDropAmount: 25, maxDropAmount: 74),
            new DropContext([ItemID.CrystalNinjaHelmet, ItemID.CrystalNinjaChestplate, ItemID.CrystalNinjaLeggings], dropAttemptCount: 2),
        ] },

        { ItemID.TwinsBossBag, [
            new DropContext(ItemID.MechanicalWheelPiece),
            new DropContext(ItemID.TwinMask, chanceDenominator: 7),
            new DropContext(ItemID.SoulofSight, minDropAmount: 25, maxDropAmount: 40),
            new DropContext(ItemID.HallowedBar, minDropAmount: 20, maxDropAmount: 35),
            .. DevSets(),
        ] },

        { ItemID.DestroyerBossBag, [
            new DropContext(ItemID.MechanicalWagonPiece),
            new DropContext(ItemID.DestroyerMask, chanceDenominator: 7),
            new DropContext(ItemID.SoulofMight, minDropAmount: 25, maxDropAmount: 40),
            new DropContext(ItemID.HallowedBar, minDropAmount: 20, maxDropAmount: 35),
            .. DevSets(),
        ] },

        { ItemID.SkeletronPrimeBossBag, [
            new DropContext(ItemID.MechanicalBatteryPiece),
            new DropContext(ItemID.SkeletronPrimeMask, chanceDenominator: 7),
            new DropContext(ItemID.SoulofFright, minDropAmount: 25, maxDropAmount: 40),
            new DropContext(ItemID.HallowedBar, minDropAmount: 20, maxDropAmount: 35),
            .. DevSets(),
        ] },

        { ItemID.PlanteraBossBag, [
            new DropContext(ItemID.SporeSac),
            new DropContext(ItemID.TempleKey),
            new DropContext(ItemID.PygmyStaff, chanceDenominator: 2),
            new DropContext(ItemID.ThornHook, chanceDenominator: 10),
            new DropContext(ItemID.PlanteraMask, chanceDenominator: 7),
            new DropContext(ItemID.Seedling, chanceDenominator: 15),
            new DropContext(ItemID.TheAxe, chanceDenominator: 20),
            new DropContext([ItemID.GrenadeLauncher, ItemID.VenusMagnum, ItemID.NettleBurst, ItemID.LeafBlower, ItemID.FlowerPow, ItemID.WaspGun, ItemID.Seedler, ItemID.FlowerWhip]),
            .. DevSets(),
        ] },

        { ItemID.GolemBossBag, [
            new DropContext(ItemID.ShinyStone),
            new DropContext(ItemID.Picksaw, chanceDenominator: 3),
            new DropContext(ItemID.GolemMask, chanceDenominator: 7),
            new DropContext(ItemID.BeetleHusk, minDropAmount: 18, maxDropAmount: 23),
            new DropContext([ItemID.Stynger, ItemID.PossessedHatchet, ItemID.SunStone, ItemID.EyeoftheGolem, ItemID.HeatRay, ItemID.StaffofEarth, ItemID.GolemFist]),
            .. DevSets(),
        ] },

        { ItemID.FishronBossBag, [
            new DropContext(ItemID.ShrimpyTruffle),
            new DropContext(ItemID.DukeFishronMask, chanceDenominator: 7),
            new DropContext(ItemID.FishronWings, chanceDenominator: 10),
            new DropContext([ItemID.BubbleGun, ItemID.Flairon, ItemID.RazorbladeTyphoon, ItemID.TempestStaff, ItemID.Tsunami, ItemID.EelWhip]),
            .. DevSets(),
        ] },

        { ItemID.FairyQueenBossBag, [
            new DropContext(ItemID.EmpressFlightBooster),
            new DropContext(ItemID.HallowBossDye, chanceDenominator: 4, minDropAmount: 3, maxDropAmount: 3),
            new DropContext(ItemID.FairyQueenMask, chanceDenominator: 7),
            new DropContext(ItemID.FairyWings, chanceDenominator: 10),
            new DropContext(ItemID.SparkleGuitar, chanceDenominator: 20),
            new DropContext(ItemID.RainbowCursor, chanceDenominator: 20),
            new DropContext([ItemID.FairyQueenMagicItem, ItemID.PiercingStarlight, ItemID.RainbowWhip, ItemID.FairyQueenRangedItem]),
            .. DevSets(),
        ] },

        { ItemID.BossBagBetsy, [
            new DropContext(ItemID.BetsyWings, chanceDenominator: 4),
            new DropContext(ItemID.BossMaskBetsy, chanceDenominator: 7),
            new DropContext(ItemID.DefenderMedal, minDropAmount: 30, maxDropAmount: 49),
            new DropContext([ItemID.DD2BetsyBow, ItemID.MonkStaffT3, ItemID.ApprenticeStaffT3, ItemID.DD2SquireBetsySword]),
            .. DevSets(),
        ] },

        { ItemID.MoonLordBossBag, [
            new DropContext(ItemID.GravityGlobe),
            new DropContext(ItemID.SuspiciousLookingTentacle),
            new DropContext(ItemID.LongRainbowTrailWings),
            new DropContext(ItemID.PortalGun, dropCondition: DropProcessor.DropCondition.NoPortalGun),
            new DropContext(ItemID.BossMaskMoonlord, chanceDenominator: 7),
            new DropContext(ItemID.MeowmereMinecart, chanceDenominator: 10),
            new DropContext(ItemID.LunarOre, minDropAmount: 90, maxDropAmount: 110),
            new DropContext([ItemID.Meowmere, ItemID.Terrarian, ItemID.StarWrath, ItemID.SDMG, ItemID.Celeb2, ItemID.LastPrism, ItemID.LunarFlareBook, ItemID.RainbowCrystalStaff, ItemID.MoonlordTurretStaff, ItemID.MoonLordWhip], dropAttemptCount: 2),
            .. DevSets(),
        ] },
    };

    private static readonly Dictionary<int, DropContext[]> s_extraDrops = new()
    {
        { ItemID.GrenadeLauncher, [new DropContext(ItemID.RocketI, minDropAmount: 50, maxDropAmount: 149)] },
        { ItemID.Stynger, [new DropContext(ItemID.StyngerBolt, minDropAmount: 60, maxDropAmount: 99)] },
    };

    private static readonly Dictionary<int, CoinAmount> s_coinAmounts = new()
    {
        { ItemID.KingSlimeBossBag, new CoinAmount(0, 2, 50, 0) },
        { ItemID.EyeOfCthulhuBossBag, new CoinAmount(0, 7, 50, 0) },
        { ItemID.EaterOfWorldsBossBag, new CoinAmount(0, 0, 20, 0) },
        { ItemID.BrainOfCthulhuBossBag, new CoinAmount(0, 12, 50, 0) },
        { ItemID.QueenBeeBossBag, new CoinAmount(0, 12, 50, 0) },
        { ItemID.DeerclopsBossBag, new CoinAmount(0, 12, 50, 0) },
        { ItemID.SkeletronBossBag, new CoinAmount(0, 12, 50, 0) },
        { ItemID.WallOfFleshBossBag, new CoinAmount(0, 20, 0, 0) },
        { ItemID.QueenSlimeBossBag, new CoinAmount(0, 15, 0, 0) },
        { ItemID.TwinsBossBag, new CoinAmount(0, 30, 0, 0) },
        { ItemID.DestroyerBossBag, new CoinAmount(0, 30, 0, 0) },
        { ItemID.SkeletronPrimeBossBag, new CoinAmount(0, 30, 0, 0) },
        { ItemID.PlanteraBossBag, new CoinAmount(0, 37, 50, 0) },
        { ItemID.GolemBossBag, new CoinAmount(0, 37, 50, 0) },
        { ItemID.FishronBossBag, new CoinAmount(0, 62, 50, 0) },
        { ItemID.FairyQueenBossBag, new CoinAmount(0, 62, 50, 0) },
        { ItemID.MoonLordBossBag, new CoinAmount(2, 50, 0, 0) },
    };

    public readonly struct CoinAmount
    {
        public readonly int Platinum;
        public readonly int Gold;
        public readonly int Silver;
        public readonly int Copper;
        public int Value => (Platinum * 1000000) + (Gold * 10000) + (Silver * 100) + Copper;

        public CoinAmount(int platinum, int gold, int silver, int copper)
        {
            Platinum = platinum;
            Gold = gold;
            Silver = silver;
            Copper = copper;
        }

        public CoinAmount(int value)
        {
            Platinum = value / 1000000;
            Gold = value % 1000000 / 10000;
            Silver = value % 10000 / 100;
            Copper = value % 100;
        }
    }
}
