namespace DeterministicDrops.DropEngine;

internal readonly struct DropResult(int itemId, int amount)
{
    public int ItemId => itemId;
    public int Amount => amount;
}
