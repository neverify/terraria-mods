using System;

namespace DeterministicDrops.DropEngine;

internal static class DropAttempt
{
    public static bool TryAdvanceState(
        DropState dropState,
        DropContext dropContext,
        GameContext gameContext,
        out int dropCycle
    )
    {
        double progress = DropChance.CalculateProgress(dropContext, gameContext);
        dropState.AddProgress(progress);

        double dropPosition = GetDropPosition(
            dropContext.Drop.Id,
            dropState.NextDropCycle,
            gameContext.WorldSeed
        );

        if (dropState.DropProgress < dropPosition)
        {
            dropCycle = -1;
            return false;
        }

        dropState.AdvanceCycle();
        dropCycle = dropState.NextDropCycle - 1;
        return true;
    }

    private static double GetDropPosition(int dropId, int dropCycle, int worldSeed)
    {
        int seed = Hashing.Hash(worldSeed, dropId, dropCycle);
        var rng = new Random(seed);
        return dropCycle + rng.NextDouble();
    }
}
