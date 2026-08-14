using System;
using System.Collections.Generic;
using Terraria.ID;

namespace DeterministicDrops.DropSystem;

internal static class BossBagDatabase
{
    public static DropContext[] GetDropContexts(int bossBagId) =>
        s_bossBags.TryGetValue(bossBagId, out var contexts) ? contexts : [];

    public static CoinAmount GetCoinAmount(int bossBagId)
    {
        s_coinAmounts.TryGetValue(bossBagId, out var amount);

        int value = (int)(amount.Value * Math.Pow(1.015, 4));
        return new CoinAmount(value);
    }

    private static DropContext[] DevSets() =>
        [
            new(
                new(s_devSets),
                chanceDenominator: 8,
                dropCondition: DropProcessor.DropCondition.CelebrationMK10
            ),
            new(
                new(s_devSets),
                chanceDenominator: 16,
                dropCondition: DropProcessor.DropCondition.NotCelebrationMK10
            ),
        ];

    // csharpier-ignore-start
    private static readonly short[][] s_devSets =
    [
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
    // csharpier-ignore-end

    private static readonly Dictionary<int, DropContext[]> s_bossBags = new()
    {
        {
            ItemID.KingSlimeBossBag,
            [
                new(new(ItemID.RoyalGel)),
                new(new(ItemID.Solidifier)),
                new(new(ItemID.SlimySaddle), chanceDenominator: 2),
                new(new(ItemID.SlimeGun), chanceDenominator: 2),
                new(new(ItemID.SlimeHook), chanceDenominator: 2),
                new(new(ItemID.KingSlimeMask), chanceDenominator: 7),
                new(new(ItemID.SlimeStaff), chanceDenominator: 30),
                new(
                    new([ItemID.NinjaHood, ItemID.NinjaShirt, ItemID.NinjaPants]),
                    dropAttemptCount: 2
                ),
            ]
        },
        {
            ItemID.EyeOfCthulhuBossBag,
            [
                new(new(ItemID.EoCShield)),
                new(new(ItemID.EyeMask), chanceDenominator: 7),
                new(new(ItemID.Binoculars), chanceDenominator: 30),
                new(new(ItemID.UnholyArrow), minDropAmount: 20, maxDropAmount: 50),
                new(
                    new(ItemID.CrimtaneOre),
                    dropCondition: DropProcessor.DropCondition.Crimson,
                    minDropAmount: 30,
                    maxDropAmount: 90
                ),
                new(
                    new(ItemID.CrimsonSeeds),
                    dropCondition: DropProcessor.DropCondition.Crimson,
                    minDropAmount: 1,
                    maxDropAmount: 3
                ),
                new(
                    new(ItemID.DemoniteOre),
                    dropCondition: DropProcessor.DropCondition.Corruption,
                    minDropAmount: 30,
                    maxDropAmount: 90
                ),
                new(
                    new(ItemID.CorruptSeeds),
                    dropCondition: DropProcessor.DropCondition.Corruption,
                    minDropAmount: 1,
                    maxDropAmount: 3
                ),
            ]
        },
        {
            ItemID.EaterOfWorldsBossBag,
            [
                new(new(ItemID.WormScarf)),
                new(new(ItemID.EaterMask), chanceDenominator: 7),
                new(new(ItemID.EatersBone), chanceDenominator: 20),
                new(
                    new(ItemID.DemoniteOre),
                    minDropAmount: 80,
                    maxDropAmount: 110,
                    dropCondition: DropProcessor.DropCondition.Crimson
                        | DropProcessor.DropCondition.NotMasterMode
                ),
                new(
                    new(ItemID.DemoniteOre),
                    minDropAmount: 110,
                    maxDropAmount: 135,
                    dropCondition: DropProcessor.DropCondition.Crimson
                        | DropProcessor.DropCondition.MasterMode
                ),
                new(
                    new(ItemID.ShadowScale),
                    minDropAmount: 20,
                    maxDropAmount: 40,
                    dropCondition: DropProcessor.DropCondition.Corruption
                        | DropProcessor.DropCondition.NotMasterMode
                ),
                new(
                    new(ItemID.ShadowScale),
                    minDropAmount: 30,
                    maxDropAmount: 50,
                    dropCondition: DropProcessor.DropCondition.Corruption
                        | DropProcessor.DropCondition.MasterMode
                ),
            ]
        },
        {
            ItemID.BrainOfCthulhuBossBag,
            [
                new(new(ItemID.BrainOfConfusion)),
                new(new(ItemID.BrainMask), chanceDenominator: 7),
                new(new(ItemID.BoneRattle), chanceDenominator: 20),
                new(
                    new(ItemID.CrimtaneOre),
                    minDropAmount: 80,
                    maxDropAmount: 110,
                    dropCondition: DropProcessor.DropCondition.Crimson
                        | DropProcessor.DropCondition.NotMasterMode
                ),
                new(
                    new(ItemID.CrimtaneOre),
                    minDropAmount: 110,
                    maxDropAmount: 135,
                    dropCondition: DropProcessor.DropCondition.Crimson
                        | DropProcessor.DropCondition.MasterMode
                ),
                new(
                    new(ItemID.TissueSample),
                    minDropAmount: 20,
                    maxDropAmount: 40,
                    dropCondition: DropProcessor.DropCondition.Corruption
                        | DropProcessor.DropCondition.NotMasterMode
                ),
                new(
                    new(ItemID.TissueSample),
                    minDropAmount: 30,
                    maxDropAmount: 50,
                    dropCondition: DropProcessor.DropCondition.Corruption
                        | DropProcessor.DropCondition.MasterMode
                ),
            ]
        },
        {
            ItemID.QueenBeeBossBag,
            [
                new(new(ItemID.HiveBackpack)),
                new(new(ItemID.HiveWand)),
                new(new(ItemID.HoneyComb), chanceDenominator: 3),
                new(new(ItemID.BeeMask), chanceDenominator: 7),
                new(new(ItemID.Nectar), chanceDenominator: 9),
                new(new(ItemID.QueenOfBees), chanceDenominator: 9),
                new(new(ItemID.HoneyedGoggles), chanceDenominator: 9),
                new(new(ItemID.Beenade), minDropAmount: 10, maxDropAmount: 29),
                new(new(ItemID.BeeWax), minDropAmount: 17, maxDropAmount: 30),
                new(new([ItemID.BeeGun, ItemID.BeeKeeper, ItemID.BeesKnees])),
                new(new([ItemID.BeeHat, ItemID.BeeShirt, ItemID.BeePants])),
            ]
        },
        {
            ItemID.DeerclopsBossBag,
            [
                new(new(ItemID.BoneHelm)),
                new(new(ItemID.ChesterPetItem), chanceDenominator: 3),
                new(new(ItemID.Eyebrella), chanceDenominator: 3),
                new(new(ItemID.DontStarveShaderItem), chanceDenominator: 3),
                new(new(ItemID.DizzyHat), chanceDenominator: 14),
                new(
                    new([
                        ItemID.PewMaticHorn,
                        ItemID.WeatherPain,
                        ItemID.HoundiusShootius,
                        ItemID.LucyTheAxe,
                    ])
                ),
            ]
        },
        {
            ItemID.SkeletronBossBag,
            [
                new(new(ItemID.BoneGlove)),
                new(new([ItemID.SkeletronMask, ItemID.SkeletronHand, ItemID.BookofSkulls])),
            ]
        },
        {
            ItemID.WallOfFleshBossBag,
            [
                new(
                    new(ItemID.DemonHeart),
                    dropCondition: DropProcessor.DropCondition.NotDemonHeart
                ),
                new(new(ItemID.Pwnhammer)),
                new(new(ItemID.FleshMask), chanceDenominator: 7),
                new(
                    new([
                        ItemID.WarriorEmblem,
                        ItemID.RangerEmblem,
                        ItemID.SorcererEmblem,
                        ItemID.SummonerEmblem,
                    ])
                ),
                new(
                    new([
                        ItemID.BreakerBlade,
                        ItemID.ClockworkAssaultRifle,
                        ItemID.LaserRifle,
                        ItemID.FireWhip,
                    ])
                ),
            ]
        },
        {
            ItemID.QueenSlimeBossBag,
            [
                new(new(ItemID.VolatileGelatin)),
                new(new(ItemID.QueenSlimeMountSaddle), chanceDenominator: 2),
                new(new(ItemID.QueenSlimeHook), chanceDenominator: 2),
                new(new(ItemID.Smolstar), chanceDenominator: 3),
                new(new(ItemID.QueenSlimeMask), chanceDenominator: 7),
                new(new(ItemID.GelBalloon), minDropAmount: 25, maxDropAmount: 74),
                new(
                    new([
                        ItemID.CrystalNinjaHelmet,
                        ItemID.CrystalNinjaChestplate,
                        ItemID.CrystalNinjaLeggings,
                    ]),
                    dropAttemptCount: 2
                ),
            ]
        },
        {
            ItemID.TwinsBossBag,
            [
                new(new(ItemID.MechanicalWheelPiece)),
                new(new(ItemID.TwinMask), chanceDenominator: 7),
                new(new(ItemID.SoulofSight), minDropAmount: 25, maxDropAmount: 40),
                new(new(ItemID.HallowedBar), minDropAmount: 20, maxDropAmount: 35),
                .. DevSets(),
            ]
        },
        {
            ItemID.DestroyerBossBag,
            [
                new(new(ItemID.MechanicalWagonPiece)),
                new(new(ItemID.DestroyerMask), chanceDenominator: 7),
                new(new(ItemID.SoulofMight), minDropAmount: 25, maxDropAmount: 40),
                new(new(ItemID.HallowedBar), minDropAmount: 20, maxDropAmount: 35),
                .. DevSets(),
            ]
        },
        {
            ItemID.SkeletronPrimeBossBag,
            [
                new(new(ItemID.MechanicalBatteryPiece)),
                new(new(ItemID.SkeletronPrimeMask), chanceDenominator: 7),
                new(new(ItemID.SoulofFright), minDropAmount: 25, maxDropAmount: 40),
                new(new(ItemID.HallowedBar), minDropAmount: 20, maxDropAmount: 35),
                .. DevSets(),
            ]
        },
        {
            ItemID.PlanteraBossBag,
            [
                new(new(ItemID.SporeSac)),
                new(new(ItemID.TempleKey)),
                new(new(ItemID.PygmyStaff), chanceDenominator: 2),
                new(new(ItemID.ThornHook), chanceDenominator: 10),
                new(new(ItemID.PlanteraMask), chanceDenominator: 7),
                new(new(ItemID.Seedling), chanceDenominator: 15),
                new(new(ItemID.TheAxe), chanceDenominator: 20),
                new(
                    new([
                        ItemID.GrenadeLauncher,
                        ItemID.VenusMagnum,
                        ItemID.NettleBurst,
                        ItemID.LeafBlower,
                        ItemID.FlowerPow,
                        ItemID.WaspGun,
                        ItemID.Seedler,
                        ItemID.FlowerWhip,
                    ]),
                    extraDrops: new()
                    {
                        [ItemID.GrenadeLauncher] = new(
                            new(ItemID.RocketI),
                            minDropAmount: 50,
                            maxDropAmount: 149
                        ),
                    }
                ),
                .. DevSets(),
            ]
        },
        {
            ItemID.GolemBossBag,
            [
                new(new(ItemID.ShinyStone)),
                new(new(ItemID.Picksaw), chanceDenominator: 3),
                new(new(ItemID.GolemMask), chanceDenominator: 7),
                new(new(ItemID.BeetleHusk), minDropAmount: 18, maxDropAmount: 23),
                new(
                    new([
                        ItemID.Stynger,
                        ItemID.PossessedHatchet,
                        ItemID.SunStone,
                        ItemID.EyeoftheGolem,
                        ItemID.HeatRay,
                        ItemID.StaffofEarth,
                        ItemID.GolemFist,
                    ]),
                    extraDrops: new()
                    {
                        [ItemID.Stynger] = new(
                            new(ItemID.StyngerBolt),
                            minDropAmount: 60,
                            maxDropAmount: 99
                        ),
                    }
                ),
                .. DevSets(),
            ]
        },
        {
            ItemID.FishronBossBag,
            [
                new(new(ItemID.ShrimpyTruffle)),
                new(new(ItemID.DukeFishronMask), chanceDenominator: 7),
                new(new(ItemID.FishronWings), chanceDenominator: 10),
                new(
                    new([
                        ItemID.BubbleGun,
                        ItemID.Flairon,
                        ItemID.RazorbladeTyphoon,
                        ItemID.TempestStaff,
                        ItemID.Tsunami,
                        ItemID.EelWhip,
                    ])
                ),
                .. DevSets(),
            ]
        },
        {
            ItemID.FairyQueenBossBag,
            [
                new(new(ItemID.EmpressFlightBooster)),
                new(
                    new(ItemID.HallowBossDye),
                    chanceDenominator: 4,
                    minDropAmount: 3,
                    maxDropAmount: 3
                ),
                new(new(ItemID.FairyQueenMask), chanceDenominator: 7),
                new(new(ItemID.FairyWings), chanceDenominator: 10),
                new(new(ItemID.SparkleGuitar), chanceDenominator: 20),
                new(new(ItemID.RainbowCursor), chanceDenominator: 20),
                new(
                    new([
                        ItemID.FairyQueenMagicItem,
                        ItemID.PiercingStarlight,
                        ItemID.RainbowWhip,
                        ItemID.FairyQueenRangedItem,
                    ])
                ),
                .. DevSets(),
            ]
        },
        {
            ItemID.BossBagBetsy,
            [
                new(new(ItemID.BetsyWings), chanceDenominator: 4),
                new(new(ItemID.BossMaskBetsy), chanceDenominator: 7),
                new(new(ItemID.DefenderMedal), minDropAmount: 30, maxDropAmount: 49),
                new(
                    new([
                        ItemID.DD2BetsyBow,
                        ItemID.MonkStaffT3,
                        ItemID.ApprenticeStaffT3,
                        ItemID.DD2SquireBetsySword,
                    ])
                ),
                .. DevSets(),
            ]
        },
        {
            ItemID.MoonLordBossBag,
            [
                new(new(ItemID.GravityGlobe)),
                new(new(ItemID.SuspiciousLookingTentacle)),
                new(new(ItemID.LongRainbowTrailWings)),
                new(new(ItemID.PortalGun), dropCondition: DropProcessor.DropCondition.NoPortalGun),
                new(new(ItemID.BossMaskMoonlord), chanceDenominator: 7),
                new(new(ItemID.MeowmereMinecart), chanceDenominator: 10),
                new(new(ItemID.LunarOre), minDropAmount: 90, maxDropAmount: 110),
                new(
                    new([
                        ItemID.Meowmere,
                        ItemID.Terrarian,
                        ItemID.StarWrath,
                        ItemID.SDMG,
                        ItemID.Celeb2,
                        ItemID.LastPrism,
                        ItemID.LunarFlareBook,
                        ItemID.RainbowCrystalStaff,
                        ItemID.MoonlordTurretStaff,
                        ItemID.MoonLordWhip,
                    ]),
                    dropAttemptCount: 2
                ),
                .. DevSets(),
            ]
        },
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
