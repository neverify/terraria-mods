using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;

namespace DeterministicDrops.DropSystem;

internal sealed class DropContext(
    Drop drop,
    int chanceNumerator = 1,
    int chanceDenominator = 1,
    int chanceRollCount = 1,
    int dropAttemptCount = 1,
    int minDropAmount = 1,
    int maxDropAmount = 1,
    DropChance.LuckType luckType = DropChance.LuckType.None,
    DropProcessor.DropCondition dropCondition = DropProcessor.DropCondition.None,
    Dictionary<int, DropContext> extraDrops = null
)
{
    public Drop Drop { get; } = drop;
    public int Numerator { get; } = chanceNumerator;
    public int Denominator { get; } = chanceDenominator;
    public int ChanceRollCount { get; } = chanceRollCount;
    public int DropAttemptCount { get; } = dropAttemptCount;
    public int MinDropAmount { get; } = minDropAmount;
    public int MaxDropAmount { get; } = maxDropAmount;
    public DropChance.LuckType LuckType { get; } = luckType;
    public DropProcessor.DropCondition Condition { get; } = dropCondition;
    public Dictionary<int, DropContext> ExtraDrops { get; } = extraDrops ?? [];
}

internal sealed class Drop
{
    public int Count => _itemIdGroups.Length;
    public int Id => Hashing.Hash(_itemIdGroups.SelectMany(x => x).Select(x => (int)x));
    public string Name =>
        string.Join(";", _itemIdGroups.Select(itemIds => string.Join(",", itemIds)));

    public ReadOnlyCollection<short> Select(int index) => new(_itemIdGroups[index]);

    private readonly short[][] _itemIdGroups;

    public Drop(short itemId) =>
        _itemIdGroups = [
            [itemId],
        ];

    public Drop(int itemId)
        : this((short)itemId) { }

    public Drop(short[] itemIds) => _itemIdGroups = [.. itemIds.Select(x => new short[] { x })];

    public Drop(int[] itemIds)
        : this([.. itemIds.Select(x => (short)x)]) { }

    public Drop(short[][] itemIdGroups) => _itemIdGroups = itemIdGroups;

    public Drop(int[][] itemIdGroups)
        : this([.. itemIdGroups.Select(group => group.Select(x => (short)x).ToArray())]) { }
}

internal sealed class GameContext
{
    public int WorldSeed { get; } = Main.ActiveWorldFileData.Seed;
    public double Luck { get; } = Main.player[Main.myPlayer].luck;
}
