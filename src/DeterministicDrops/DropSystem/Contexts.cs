using Terraria;

namespace DeterministicDrops.DropSystem;

internal sealed class DropContext(
    int[] itemIds,
    int chanceNumerator = 1,
    int chanceDenominator = 1,
    int chanceRollCount = 1,
    int dropAttemptCount = 1,
    int minDropAmount = 1,
    int maxDropAmount = 1,
    DropChance.LuckType luckType = DropChance.LuckType.All)
{
    public int[] ItemIds => itemIds;
    public int Numerator => chanceNumerator;
    public int Denominator => chanceDenominator;
    public int ChanceRollCount => chanceRollCount;
    public int DropAttemptCount => dropAttemptCount;
    public int MinDropAmount => minDropAmount;
    public int MaxDropAmount => maxDropAmount;
    public DropChance.LuckType LuckType => luckType;
}

internal sealed class GameContext
{
    public int WorldSeed { get; }
    public double Luck { get; }

    public GameContext()
    {
        WorldSeed = Main.ActiveWorldFileData.Seed;
        Luck = Main.player[Main.myPlayer].luck;
    }
}
