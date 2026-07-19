using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utils;

public static class FileUtils
{
    private static readonly HashSet<char> s_invalidFileNameChars = [.. Path.GetInvalidFileNameChars()];

    public static string SanitizeFilename(string fileName)
    {
        var sb = new StringBuilder(fileName.Length);

        foreach (char c in fileName)
        {
            sb.Append(c switch
            {
                ' ' => '_',
                '.' => '_',
                '*' => '_',
                _ when s_invalidFileNameChars.Contains(c) => '-',
                _ => c
            });
        }

        return sb.ToString();
    }
}
