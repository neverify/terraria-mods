using System.Text.RegularExpressions;

namespace Utils;

public static class PlaceholderFormatter
{
    public static string Format(string template, params (string Name, string Value)[] replacements)
    {
        return Regex.Replace(
            template,
            @"\{(\w+)\}",
            match =>
            {
                string placeholder = match.Groups[1].Value;

                foreach ((string name, string value) in replacements)
                {
                    if (placeholder == name)
                        return value;
                }

                return match.Value;
            }
        );
    }
}
