using SettingsKeybind.Features;

namespace SettingsKeybind;

internal static class Keybinds
{
    public static void Register()
    {
        Mod.Instance.Context.RegisterKeybind(
            "toggle-settings",
            "Toggle Settings",
            "Toggle the settings menu",
            "Escape",
            ToggleSettings.Toggle
        );

        Mod.Instance.Log.Info("Keybinds registered.");
    }
}
