using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace DeterministicDrops.DropSystem;

internal static class DropSelection
{
    public static IEnumerable<DropResult> GetDropResults(DropContext dropContext, GameContext gameContext, int dropCycle)
    {
        var droppedItemIds = GetDroppedItemIds(dropContext.ItemIds, dropCycle, gameContext.WorldSeed);

        foreach (short droppedItemId in droppedItemIds)
        {
            int dropAmount = GetDropAmount(droppedItemId, dropCycle, dropContext.MinDropAmount, dropContext.MaxDropAmount, gameContext.WorldSeed);
            yield return new DropResult(droppedItemId, dropAmount);
        }
    }

    private static IEnumerable<short> GetDroppedItemIds(short[][] itemIds, int dropCycle, int worldSeed)
    {
        int itemCycle = dropCycle / itemIds.Length;

        int itemsSeed = Hashing.Hash(itemIds);
        int seed = Hashing.Hash(worldSeed, itemsSeed, itemCycle);
        var rng = new Random(seed);

        short[][] shuffledItemIds = [.. itemIds];
        rng.Shuffle(shuffledItemIds);

        var itemIdGroup = shuffledItemIds[dropCycle % itemIds.Length];

        foreach (short itemId in itemIdGroup)
            yield return itemId;
    }

    private static int GetDropAmount(short itemId, int dropCycle, int minAmount, int maxAmount, int worldSeed)
    {
        int count = maxAmount - minAmount + 1;
        int amountCycle = dropCycle / count;

        int seed = Hashing.Hash(worldSeed, itemId, minAmount, maxAmount, amountCycle);
        var rng = new Random(seed);

        int[] amounts = [.. Enumerable.Range(minAmount, count)];
        rng.Shuffle(amounts);

        return amounts[dropCycle % count];
    }
}
