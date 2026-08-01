using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.DropSystem;

internal static class DropScheduler
{
    private const int MaxDenominator = 2 * 10000;

    private static readonly double[] s_harmonicLut = CreateHarmonicLut();

    public readonly struct Drop(int itemId, int amount)
    {
        public int ItemId => itemId;
        public int Amount => amount;
    }

    public static IEnumerable<Drop> ProcessCommonDrop(CommonDrop drop, DropStateStore store)
    => ProcessCommonDrop(drop, store, LuckType.All);

    public static IEnumerable<Drop> ProcessCommonDropNotScalingWithLuck(CommonDropNotScalingWithLuck drop, DropStateStore store)
    => ProcessCommonDrop(drop, store, LuckType.None);

    public static IEnumerable<Drop> ProcessCommonDropScalingWithOnlyBadLuck(CommonDropScalingWithOnlyBadLuck drop, DropStateStore store)
    => ProcessCommonDrop(drop, store, LuckType.Negative);

    public static IEnumerable<Drop> ProcessCommonDropWithRerolls(CommonDropWithRerolls drop, DropStateStore store)
    => ProcessCommonDrop(drop, store, LuckType.All, drop.timesToRoll);

    public static IEnumerable<Drop> ProcessFromOptionsWithoutRepeatsDropRule(FromOptionsWithoutRepeatsDropRule drop, DropStateStore store)
    => ProcessOptionDrop(drop.dropIds, drop.dropCount, 1, 1, store);

    public static IEnumerable<Drop> ProcessOneFromOptionsNotScaledWithLuckDropRule(OneFromOptionsNotScaledWithLuckDropRule drop, DropStateStore store)
    => ProcessOptionDrop(drop.dropIds, 1, drop.chanceNumerator, drop.chanceDenominator, store);

    public static IEnumerable<Drop> ProcessOneFromOptionsDropRule(OneFromOptionsDropRule drop, DropStateStore store)
    => ProcessOptionDrop(drop.dropIds, 1, drop.chanceNumerator, drop.chanceDenominator, store, LuckType.All);

    private static IEnumerable<Drop> ProcessCommonDrop(CommonDrop drop, DropStateStore store, LuckType luckType, int rolls = 1)
    {
        var dropState = store.GetState(drop.itemId);

        var advanceResult = AdvanceState(dropState, (ulong)drop.itemId, drop.chanceNumerator, drop.chanceDenominator, luckType, rolls);
        if (advanceResult.Dropped)
        {
            int dropAmount = GetDropAmount(drop.itemId, advanceResult.DropCycle, drop.amountDroppedMinimum, drop.amountDroppedMaximum);
            yield return new Drop(drop.itemId, dropAmount);
        }
    }

    private static IEnumerable<Drop> ProcessOptionDrop(int[] dropIds, int dropCount, int chanceNumerator, int chanceDenominator, DropStateStore store, LuckType luckType = LuckType.None)
    {
        ulong dropId = Hash64(dropIds);
        var dropState = store.GetState(dropId);

        for (int i = 0; i < dropCount; i++)
        {
            var advanceResult = AdvanceState(dropState, dropId, chanceNumerator, chanceDenominator, luckType);
            if (advanceResult.Dropped)
            {
                int droppedItemId = GetDroppedItemId(dropIds, advanceResult.DropCycle);
                yield return new Drop(droppedItemId, 1);
            }
        }
    }

    private static AdvanceStateResult AdvanceState(DropState dropState, ulong dropId, int numerator, int denominator, LuckType luckType = LuckType.All, int rolls = 1)
    {
        double progress = CalculateProgress(numerator, denominator, luckType, rolls);
        dropState.AddProgress(progress);

        double dropPosition = GetDropPosition(To32(dropId), dropState.NextDropCycle);

        if (dropState.DropProgress < dropPosition)
            return new AdvanceStateResult(false, -1);

        dropState.AdvanceCycle();
        return new AdvanceStateResult(true, dropState.NextDropCycle - 1);
    }

    private readonly struct AdvanceStateResult(bool dropped, int dropCycle)
    {
        public bool Dropped => dropped;
        public int DropCycle => dropCycle;
    }

    private static double CalculateProgress(int numerator, int denominator, LuckType luckType, int rolls = 1)
    {
        double chance = ApplyLuck(numerator, denominator, luckType);
        chance = ApplyRerolls(chance, rolls);

        return chance;
    }

    private static double ApplyLuck(int numerator, int denominator, LuckType luckType)
    {
        double luck = Main.player[Main.myPlayer].luck;
        double baseChance = (double)numerator / denominator;

        if (denominator > MaxDenominator)
        {
            Mod.Instance.Log.Warn("Failed to apply luck: denominator exceeds maximum.");
            return baseChance;
        }

        if (baseChance >= 1.0)
            return 1.0;

        luck = luckType switch
        {
            LuckType.None => 0.0,
            LuckType.All => luck,
            LuckType.Positive => Math.Max(0.0, luck),
            LuckType.Negative => Math.Min(0.0, luck),
            _ => 0.0,
        };

        if (luck == 0.0)
            return baseChance;

        if (luck > 0.0)
        {
            int min = denominator / 2;
            int count = denominator - min;

            double luckyChance = (s_harmonicLut[denominator - 1] - s_harmonicLut[min - 1]) / count;
            luckyChance *= numerator;

            return (baseChance * (1.0 - luck)) + (luckyChance * luck);
        }
        else
        {
            double luckyChance = (s_harmonicLut[(2 * denominator) - 1] - s_harmonicLut[denominator - 1]) / denominator;
            luckyChance *= numerator;

            return (baseChance * (1.0 + luck)) + (luckyChance * -luck);
        }
    }

    private enum LuckType
    {
        None,
        All,
        Positive,
        Negative
    }

    private static double ApplyRerolls(double baseChance, int rolls)
    => rolls <= 1 ? baseChance : 1.0 - Math.Pow(1.0 - baseChance, rolls);

    private static int GetDroppedItemId(int[] itemIds, int dropCycle)
    {
        int itemCycle = dropCycle / itemIds.Length;

        int itemsSeed = Hash32(itemIds);
        int seed = Hash32(Main.ActiveWorldFileData.Seed, itemsSeed, itemCycle);
        var rng = new Random(seed);

        int[] shuffledItemIds = [.. itemIds];
        Shuffle(shuffledItemIds, rng);

        return shuffledItemIds[dropCycle % itemIds.Length];
    }

    private static double GetDropPosition(int itemId, int dropCycle)
    {
        int seed = Hash32(Main.ActiveWorldFileData.Seed, itemId, dropCycle);
        var rng = new Random(seed);
        return dropCycle + rng.NextDouble();
    }

    private static int GetDropAmount(int itemId, int dropCycle, int minAmount, int maxAmount)
    {
        int count = maxAmount - minAmount + 1;
        int amountCycle = dropCycle / count;

        int seed = Hash32(Main.ActiveWorldFileData.Seed, itemId, amountCycle);
        var rng = new Random(seed);

        int[] amounts = [.. Enumerable.Range(minAmount, count)];
        Shuffle(amounts, rng);

        return amounts[dropCycle % count];
    }

    private static double[] CreateHarmonicLut()
    {
        double[] harmonics = new double[MaxDenominator + 1];

        harmonics[0] = 0.0;

        for (int i = 1; i <= MaxDenominator; i++)
            harmonics[i] = harmonics[i - 1] + (1.0 / i);

        return harmonics;
    }

    private static void Shuffle(int[] array, Random random)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[j], array[i]) = (array[i], array[j]);
        }
    }

    private static int Hash32(params int[] values)
    {
        ulong hash = Hash64(values);
        return To32(hash);
    }

    private static int To32(ulong value) => (int)(value ^ (value >> 32));

    private static ulong Hash64(params int[] values)
    {
        ulong hash = 0;

        foreach (int value in values)
            hash = Mix(hash + (ulong)value);

        return hash;
    }

    private static ulong Mix(ulong x)
    {
        x += 0x9E3779B97F4A7C15UL;
        x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
        x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
        return x ^ (x >> 31);
    }
}
