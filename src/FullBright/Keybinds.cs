using System;

namespace FullBright;

internal static class Keybinds
{
    public static void Register()
    {
        Mod.Instance.Context.RegisterKeybind(
            "toggle-brightness-override",
            "Toggle Brightness Override",
            "Toggle the brightness override.",
            string.Empty,
            ToggleFullBright
        );

        Mod.Instance.Context.RegisterKeybind(
            "increase-brightness",
            "Increase Brightness",
            "Increase fullbright brightness by 10%.",
            string.Empty,
            IncreaseBrightness
        );

        Mod.Instance.Context.RegisterKeybind(
            "decrease-brightness",
            "Decrease Brightness",
            "Decrease fullbright brightness by 10%.",
            string.Empty,
            DecreaseBrightness
        );

        Mod.Instance.Log.Info("Keybinds registered.");
    }

    private static void IncreaseBrightness() => ChangeBrightness(0.1f);

    private static void DecreaseBrightness() => ChangeBrightness(-0.1f);

    private static void ChangeBrightness(float amount) =>
        Mod.Instance.Config.Brightness = Math.Max(
            0,
            Math.Min(1, Mod.Instance.Config.Brightness + amount)
        );

    private static void ToggleFullBright() =>
        Mod.Instance.Config.BrightnessOverride = !Mod.Instance.Config.BrightnessOverride;
}
