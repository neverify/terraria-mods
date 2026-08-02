using System;

namespace DeterministicDrops.DropSystem;

internal static class DropChance
{
    private const int MaxDenominator = 2 * 10000;

    private static readonly double[] s_harmonicLut = CreateHarmonicLut();

    public enum LuckType
    {
        None,
        All,
        Positive,
        Negative
    }

    public static double CalculateProgress(DropContext dropContext, GameContext gameContext)
    {
        double chance = ApplyLuck(dropContext.Numerator, dropContext.Denominator, gameContext.Luck, dropContext.LuckType);
        chance = ApplyRerolls(chance, dropContext.ChanceRollCount);

        return chance;
    }

    private static double ApplyLuck(int numerator, int denominator, double luck, LuckType luckType)
    {
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

    private static double ApplyRerolls(double baseChance, int rolls)
    => rolls <= 1 ? baseChance : 1.0 - Math.Pow(1.0 - baseChance, rolls);

    private static double[] CreateHarmonicLut()
    {
        double[] harmonics = new double[MaxDenominator + 1];

        harmonics[0] = 0.0;

        for (int i = 1; i <= MaxDenominator; i++)
            harmonics[i] = harmonics[i - 1] + (1.0 / i);

        return harmonics;
    }
}
