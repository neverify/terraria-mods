using System.Linq;
using Terraria;

namespace DeterministicDrops.DropSystem;

internal sealed class DropContext
{
    public short[][] ItemIdGroups { get; }
    public int Numerator { get; }
    public int Denominator { get; }
    public int ChanceRollCount { get; }
    public int DropAttemptCount { get; }
    public int MinDropAmount { get; }
    public int MaxDropAmount { get; }
    public DropChance.LuckType LuckType { get; }
    public DropProcessor.DropCondition Condition { get; }

    public DropContext(
        short itemId,
        int chanceNumerator = 1,
        int chanceDenominator = 1,
        int chanceRollCount = 1,
        int dropAttemptCount = 1,
        int minDropAmount = 1,
        int maxDropAmount = 1,
        DropChance.LuckType luckType = DropChance.LuckType.None,
        DropProcessor.DropCondition dropCondition = DropProcessor.DropCondition.None)
    {
        ItemIdGroups = [[itemId]];
        Numerator = chanceNumerator;
        Denominator = chanceDenominator;
        ChanceRollCount = chanceRollCount;
        DropAttemptCount = dropAttemptCount;
        MinDropAmount = minDropAmount;
        MaxDropAmount = maxDropAmount;
        LuckType = luckType;
        Condition = dropCondition;
    }

    public DropContext(
        short[] itemIds,
        int chanceNumerator = 1,
        int chanceDenominator = 1,
        int chanceRollCount = 1,
        int dropAttemptCount = 1,
        int minDropAmount = 1,
        int maxDropAmount = 1,
        DropChance.LuckType luckType = DropChance.LuckType.None,
        DropProcessor.DropCondition dropCondition = DropProcessor.DropCondition.None)
    {
        ItemIdGroups = [.. itemIds.Select(x => new short[] { x })];
        Numerator = chanceNumerator;
        Denominator = chanceDenominator;
        ChanceRollCount = chanceRollCount;
        DropAttemptCount = dropAttemptCount;
        MinDropAmount = minDropAmount;
        MaxDropAmount = maxDropAmount;
        LuckType = luckType;
        Condition = dropCondition;
    }

    public DropContext(
        short[][] itemIdGroups,
        int chanceNumerator = 1,
        int chanceDenominator = 1,
        int chanceRollCount = 1,
        int dropAttemptCount = 1,
        int minDropAmount = 1,
        int maxDropAmount = 1,
        DropChance.LuckType luckType = DropChance.LuckType.None,
        DropProcessor.DropCondition dropCondition = DropProcessor.DropCondition.None)
    {
        ItemIdGroups = itemIdGroups;
        Numerator = chanceNumerator;
        Denominator = chanceDenominator;
        ChanceRollCount = chanceRollCount;
        DropAttemptCount = dropAttemptCount;
        MinDropAmount = minDropAmount;
        MaxDropAmount = maxDropAmount;
        LuckType = luckType;
        Condition = dropCondition;
    }
}

internal sealed class DropItems
{
    public int Length => _itemIdGroups.Length;

    public short[] GetItemIds(int index) => _itemIdGroups[index];

    private readonly short[][] _itemIdGroups;

    public DropItems(short itemId)
    {
        _itemIdGroups = [[itemId]];
    }

    public DropItems(short[] itemIds)
    {
        _itemIdGroups = [.. itemIds.Select(x => new short[] { x })];
    }

    public DropItems(short[][] itemIdGroups)
    {
        _itemIdGroups = itemIdGroups;
    }
}

internal sealed class GameContext
{
    public int WorldSeed { get; } = Main.ActiveWorldFileData.Seed;
    public double Luck { get; } = Main.player[Main.myPlayer].luck;
}
