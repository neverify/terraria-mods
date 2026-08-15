using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace DeterministicDrops.DropEngine;

internal static class DropProcessor
{
    public static IEnumerable<DropResult> ProcessCommonDrop(
        CommonDrop drop,
        DropStateStore store
    ) => ProcessCommonDrop(drop, store, DropChance.LuckType.All);

    public static IEnumerable<DropResult> ProcessCommonDropNotScalingWithLuck(
        CommonDropNotScalingWithLuck drop,
        DropStateStore store
    ) => ProcessCommonDrop(drop, store, DropChance.LuckType.None);

    public static IEnumerable<DropResult> ProcessCommonDropScalingWithOnlyBadLuck(
        CommonDropScalingWithOnlyBadLuck drop,
        DropStateStore store
    ) => ProcessCommonDrop(drop, store, DropChance.LuckType.Negative);

    public static IEnumerable<DropResult> ProcessCommonDropWithRerolls(
        CommonDropWithRerolls drop,
        DropStateStore store
    ) => ProcessCommonDrop(drop, store, DropChance.LuckType.All, drop.timesToRoll);

    private static IEnumerable<DropResult> ProcessCommonDrop(
        CommonDrop drop,
        DropStateStore store,
        DropChance.LuckType luckType,
        int chanceRollCount = 1
    ) =>
        ProcessDrop(
            new DropContext(
                new(drop.itemId),
                chanceNumerator: drop.chanceNumerator,
                chanceDenominator: drop.chanceDenominator,
                chanceRollCount: chanceRollCount,
                minDropAmount: drop.amountDroppedMinimum,
                maxDropAmount: drop.amountDroppedMaximum,
                luckType: luckType
            ),
            store
        );

    public static IEnumerable<DropResult> ProcessFromOptionsWithoutRepeatsDropRule(
        FromOptionsWithoutRepeatsDropRule drop,
        DropStateStore store
    ) => ProcessDrop(new DropContext(new(drop.dropIds), dropAttemptCount: drop.dropCount), store);

    public static IEnumerable<DropResult> ProcessOneFromOptionsNotScaledWithLuckDropRule(
        OneFromOptionsNotScaledWithLuckDropRule drop,
        DropStateStore store
    ) =>
        ProcessDrop(
            new DropContext(
                new(drop.dropIds),
                chanceNumerator: drop.chanceNumerator,
                chanceDenominator: drop.chanceDenominator
            ),
            store
        );

    public static IEnumerable<DropResult> ProcessOneFromOptionsDropRule(
        OneFromOptionsDropRule drop,
        DropStateStore store
    ) =>
        ProcessDrop(
            new DropContext(
                new(drop.dropIds),
                chanceNumerator: drop.chanceNumerator,
                chanceDenominator: drop.chanceDenominator,
                luckType: DropChance.LuckType.All
            ),
            store
        );

    public static IEnumerable<DropResult> ProcessMechBossSpawnersDropRule(
        MechBossSpawnersDropRule _,
        DropStateStore store
    )
    {
        var mechanicalEyeDropContext = new DropContext(
            new(ItemID.MechanicalEye),
            chanceDenominator: 2500,
            luckType: DropChance.LuckType.All,
            dropCondition: DropCondition.NotDefeatedTheTwins
        );

        var mechanicalWormDropContext = new DropContext(
            new(ItemID.MechanicalWorm),
            chanceDenominator: 2500,
            luckType: DropChance.LuckType.All,
            dropCondition: DropCondition.NotDefeatedTheDestroyer
        );

        var mechanicalSkullDropContext = new DropContext(
            new(ItemID.MechanicalSkull),
            chanceDenominator: 2500,
            luckType: DropChance.LuckType.All,
            dropCondition: DropCondition.NotDefeatedSkeletronPrime
        );

        var result = ProcessDrop(mechanicalEyeDropContext, store);
        if (result.Any())
            return result;

        result = ProcessDrop(mechanicalWormDropContext, store);
        if (result.Any())
            return result;

        result = ProcessDrop(mechanicalSkullDropContext, store);
        return result.Any() ? result : [];
    }

    public static IEnumerable<DropResult> ProcessDrop(DropContext dropContext, DropStateStore store)
    {
        if (!CanDrop(dropContext))
            yield break;

        var dropState = store.GetState(dropContext.Drop);
        var gameContext = new GameContext();

        for (int i = 0; i < dropContext.DropAttemptCount; i++)
        {
            if (DropAttempt.TryAdvanceState(dropState, dropContext, gameContext, out int dropCycle))
            {
                var dropResults = DropSelection.GetDropResults(dropContext, gameContext, dropCycle);

                foreach (var dropResult in dropResults)
                {
                    if (
                        dropContext.ExtraDrops.TryGetValue(
                            dropResult.ItemId,
                            out DropContext extraDropContext
                        )
                    )
                    {
                        var extraDropResults = ProcessDrop(extraDropContext, store);

                        foreach (var extraDropResult in extraDropResults)
                            yield return extraDropResult;
                    }

                    yield return dropResult;
                }
            }
        }
    }

    private static bool CanDrop(DropContext dropContext)
    {
        var condition = dropContext.Condition;

        var player = Main.player[Main.myPlayer];

        return (!condition.HasFlag(DropCondition.MasterMode) || Main.masterMode)
            && (!condition.HasFlag(DropCondition.NotMasterMode) || !Main.masterMode)
            && (!condition.HasFlag(DropCondition.Crimson) || WorldGen.crimson)
            && (!condition.HasFlag(DropCondition.Corruption) || !WorldGen.crimson)
            && (!condition.HasFlag(DropCondition.DemonHeart) || player.extraAccessory)
            && (!condition.HasFlag(DropCondition.NoDemonHeart) || !player.extraAccessory)
            && (!condition.HasFlag(DropCondition.CelebrationMK10) || Main.tenthAnniversaryWorld)
            && (!condition.HasFlag(DropCondition.NotCelebrationMK10) || !Main.tenthAnniversaryWorld)
            && (!condition.HasFlag(DropCondition.NoPortalGun) || !player.HasItem(ItemID.PortalGun))
            && (!condition.HasFlag(DropCondition.NotDefeatedTheTwins) || !NPC.downedMechBoss1)
            && (!condition.HasFlag(DropCondition.NotDefeatedTheDestroyer) || !NPC.downedMechBoss2)
            && (
                !condition.HasFlag(DropCondition.NotDefeatedSkeletronPrime) || !NPC.downedMechBoss3
            );
    }

    [Flags]
    public enum DropCondition
    {
        None = 0,
        MasterMode = 1,
        NotMasterMode = 1 << 1,
        Crimson = 1 << 2,
        Corruption = 1 << 3,
        DemonHeart = 1 << 4,
        NoDemonHeart = 1 << 5,
        CelebrationMK10 = 1 << 6,
        NotCelebrationMK10 = 1 << 7,
        NoPortalGun = 1 << 8,
        NotDefeatedTheTwins = 1 << 9,
        NotDefeatedTheDestroyer = 1 << 10,
        NotDefeatedSkeletronPrime = 1 << 11,
    }
}
