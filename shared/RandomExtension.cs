using System;

namespace Utils;

public static class RandomExtension
{
    extension(Random random)
    {
        public void Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
        }
    }
}
