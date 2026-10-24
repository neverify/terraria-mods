using HarmonyLib;
using Terraria;

namespace SettingsKeybind.Patches;

[HarmonyPatch(typeof(Main), nameof(Main.DrawInterface_29_SettingsButton))]
internal static class DrawInterface_29_SettingsButtonPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    internal static bool Prefix() => !Mod.Instance.Config.HideSettingsButton;
}
