using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace ValueTooltip.Features;

internal static class Coins
{
    public const long Platinum = 1000000;
    public const long Gold = 10000;
    public const long Silver = 100;
    public const long Copper = 1;

    public static long GetValue(IEnumerable<Item> items) =>
        Enumerable.Sum(items, static item => item.value / 5 * item.stack);

    public static IEnumerable<int> FromValue(long value)
    {
        yield return (int)(value / Platinum);
        yield return (int)(value % Platinum / Gold);
        yield return (int)(value % Gold / Silver);
        yield return (int)(value % Silver / Copper);
    }
}
