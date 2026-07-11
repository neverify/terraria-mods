using Terraria;

namespace SettingsKeybind.Features;

internal static class ToggleSettings
{
    internal static void Toggle()
    {
        switch (Main.ingameOptionsWindow)
        {
            case true:
                IngameOptions.Close();
                break;
            case false:
                IngameOptions.Open();
                break;
        }
    }
}
