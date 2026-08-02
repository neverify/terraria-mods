using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.DropSystem;

internal static class DropScheduler
{
    public static IEnumerable<DropResult> ProcessCommonDrop(CommonDrop drop, DropStateStore store)
    => ProcessCommonDrop(drop, store, DropChance.LuckType.All);

    public static IEnumerable<DropResult> ProcessCommonDropNotScalingWithLuck(CommonDropNotScalingWithLuck drop, DropStateStore store)
    => ProcessCommonDrop(drop, store, DropChance.LuckType.None);

    public static IEnumerable<DropResult> ProcessCommonDropScalingWithOnlyBadLuck(CommonDropScalingWithOnlyBadLuck drop, DropStateStore store)
    => ProcessCommonDrop(drop, store, DropChance.LuckType.Negative);

    public static IEnumerable<DropResult> ProcessCommonDropWithRerolls(CommonDropWithRerolls drop, DropStateStore store)
    => ProcessCommonDrop(drop, store, DropChance.LuckType.All, drop.timesToRoll);

    public static IEnumerable<DropResult> ProcessFromOptionsWithoutRepeatsDropRule(FromOptionsWithoutRepeatsDropRule drop, DropStateStore store)
    => ProcessDrop(
        new DropContext(
            itemIds: drop.dropIds,
            dropAttemptCount: drop.dropCount,
            luckType: DropChance.LuckType.None),
        store);

    public static IEnumerable<DropResult> ProcessOneFromOptionsNotScaledWithLuckDropRule(OneFromOptionsNotScaledWithLuckDropRule drop, DropStateStore store)
    => ProcessDrop(
        new DropContext(
            itemIds: drop.dropIds,
            chanceNumerator: drop.chanceNumerator,
            chanceDenominator: drop.chanceDenominator,
            luckType: DropChance.LuckType.None),
        store);

    public static IEnumerable<DropResult> ProcessOneFromOptionsDropRule(OneFromOptionsDropRule drop, DropStateStore store)
    => ProcessDrop(
        new DropContext(
            itemIds: drop.dropIds,
            chanceNumerator: drop.chanceNumerator,
            chanceDenominator: drop.chanceDenominator),
        store);

    private static IEnumerable<DropResult> ProcessCommonDrop(CommonDrop drop, DropStateStore store, DropChance.LuckType luckType, int chanceRollCount = 1)
    => ProcessDrop(
        new DropContext(
            itemIds: [drop.itemId],
            chanceNumerator: drop.chanceNumerator,
            chanceDenominator: drop.chanceDenominator,
            chanceRollCount: chanceRollCount,
            minDropAmount: drop.amountDroppedMinimum,
            maxDropAmount: drop.amountDroppedMaximum,
            luckType: luckType),
        store);

    private static IEnumerable<DropResult> ProcessDrop(DropContext dropContext, DropStateStore store)
    {
        var dropState = store.GetState(dropContext.ItemIds);
        var gameContext = new GameContext();

        for (int i = 0; i < dropContext.DropAttemptCount; i++)
        {
            bool dropped = DropAttempt.TryAdvanceState(dropState, dropContext, gameContext, out int dropCycle);

            if (dropped)
            {
                var dropResult = DropSelection.GetDropResult(dropContext, gameContext, dropCycle);
                yield return dropResult;
            }
        }
    }
}
