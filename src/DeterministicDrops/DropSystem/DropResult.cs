namespace DeterministicDrops.DropSystem;

internal readonly struct DropResult(int itemId, int amount)
{
    public int ItemId => itemId;
    public int Amount => amount;
}
