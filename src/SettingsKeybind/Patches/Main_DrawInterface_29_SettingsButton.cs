using HarmonyLib;
using Terraria;

namespace SettingsKeybind.Patches;

[HarmonyPatch(typeof(Main), nameof(Main.DrawInterface_29_SettingsButton))]
internal static class DrawInterface_29_SettingsButtonPatch
{
    internal static bool Prefix() => !Mod.Config.HideSettingsButton;
}
