using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.DropSystem;

internal static class DropScheduler
{
    private const int MaxDenominator = 20000;

    private static readonly double[] s_harmonics = CreateHarmonics();

    public static int ProcessKill(CommonDrop drop, DropStateStore store)
    {
        var dropState = store.Get(drop.itemId);

        double progress = CalculateProgress(drop);
        dropState.AddProgress(progress);

        double dropPosition = GetDropPosition(drop.itemId, dropState.NextDropCycle);

        if (dropState.DropProgress < dropPosition)
            return 0;

        int dropAmount = GetDropAmount(drop.itemId, dropState.NextDropCycle, drop.amountDroppedMinimum, drop.amountDroppedMaximum);

        dropState.AdvanceCycle();

        return dropAmount;
    }

    private static double CalculateProgress(CommonDrop drop)
    {
        int numerator = drop.chanceNumerator;
        int denominator = drop.chanceDenominator;

        double baseChance = (double)numerator / denominator;

        if (denominator > MaxDenominator)
            return baseChance;

        if (baseChance >= 1.0)
            return 1.0;

        double luck = Main.player[Main.myPlayer].luck;

        if (luck == 0.0)
            return baseChance;

        if (luck > 0.0)
        {
            int min = denominator / 2;
            int count = denominator - min;

            double luckyChance = (s_harmonics[denominator - 1] - s_harmonics[min - 1]) / count;

            luckyChance *= numerator;

            return (baseChance * (1.0 - luck)) + (luckyChance * luck);
        }
        else
        {
            double luckyChance = (s_harmonics[(2 * denominator) - 1] - s_harmonics[denominator - 1]) / denominator;

            luckyChance *= numerator;

            return (baseChance * (1.0 + luck)) + (luckyChance * -luck);
        }
    }

    private static double GetDropPosition(int itemId, int dropCycle)
    {
        int seed = Hash(Main.ActiveWorldFileData.Seed, itemId, dropCycle);
        var rng = new Random(seed);
        return dropCycle + rng.NextDouble();
    }

    private static int GetDropAmount(int itemId, int dropCycle, int minAmount, int maxAmount)
    {
        int count = maxAmount - minAmount + 1;
        int amountCycle = dropCycle / count;

        int seed = Hash(Main.ActiveWorldFileData.Seed, itemId, amountCycle);
        var rng = new Random(seed);

        int[] amounts = [.. Enumerable.Range(minAmount, count)];
        Shuffle(amounts, rng);

        return amounts[dropCycle % count];
    }

    private static double[] CreateHarmonics()
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

    private static int Hash(params int[] values)
    {
        ulong hash = 0;

        foreach (int value in values)
        {
            hash = Mix(hash + (ulong)value);
        }

        return (int)(hash ^ (hash >> 32));
    }

    private static ulong Mix(ulong x)
    {
        x += 0x9E3779B97F4A7C15UL;
        x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
        x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
        return x ^ (x >> 31);
    }
}
