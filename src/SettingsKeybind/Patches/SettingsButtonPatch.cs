namespace SettingsKeybind.Patches;

internal static class SettingsButtonPatch
{
    internal static bool Prefix() => !Mod.Config.HideSettingsButton;
}
