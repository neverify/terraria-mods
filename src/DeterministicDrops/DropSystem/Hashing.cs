namespace DeterministicDrops.DropSystem;

internal static class Hashing
{
    public static int Hash(params int[] values)
    {
        ulong hash = 0;

        foreach (int value in values)
            hash = Mix(hash + (ulong)value);

        return (int)(hash ^ (hash >> 32));
    }

    private static ulong Mix(ulong x)
    {
        x += 0x9E3779B97F4A7C15UL;
        x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
        x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
        return x ^ (x >> 31);
    }
}
