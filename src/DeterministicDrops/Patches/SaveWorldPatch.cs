namespace DeterministicDrops.Patches;

internal static class SaveWorldPatch
{
    internal static void Prefix() => Mod.Instance.DropStateStore?.Save();
}
