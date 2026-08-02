using System;
using System.Linq;
using Utils;

namespace DeterministicDrops.DropSystem;

internal static class DropSelection
{
    public static DropResult GetDropResult(DropContext dropContext, GameContext gameContext, int dropCycle)
    {
        int droppedItemId = GetDroppedItemId(dropContext.ItemIds, dropCycle, gameContext.WorldSeed);
        int dropAmount = GetDropAmount(droppedItemId, dropCycle, dropContext.MinDropAmount, dropContext.MaxDropAmount, gameContext.WorldSeed);

        return new DropResult(droppedItemId, dropAmount);
    }

    private static int GetDroppedItemId(int[] itemIds, int dropCycle, int worldSeed)
    {
        int itemCycle = dropCycle / itemIds.Length;

        int itemsSeed = Hashing.Hash(itemIds);
        int seed = Hashing.Hash(worldSeed, itemsSeed, itemCycle);
        var rng = new Random(seed);

        int[] shuffledItemIds = [.. itemIds];
        rng.Shuffle(shuffledItemIds);

        return shuffledItemIds[dropCycle % itemIds.Length];
    }

    private static int GetDropAmount(int itemId, int dropCycle, int minAmount, int maxAmount, int worldSeed)
    {
        int count = maxAmount - minAmount + 1;
        int amountCycle = dropCycle / count;

        int seed = Hashing.Hash(worldSeed, itemId, amountCycle);
        var rng = new Random(seed);

        int[] amounts = [.. Enumerable.Range(minAmount, count)];
        rng.Shuffle(amounts);

        return amounts[dropCycle % count];
    }
}
