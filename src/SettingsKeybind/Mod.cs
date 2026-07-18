using HarmonyLib;
using SettingsKeybind.Patches;
using Terraria;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;
using Utils;

namespace SettingsKeybind;

public class Mod : IMod
{
    public string Id => "settings-keybind";
    public string Name => "Settings Keybind";
    public string Version => "1.0.0";

    internal static ILogger Log;
    internal static ModContext Context;
    internal static Config Config;
    private static Harmony s_harmony;

    public void Initialize(ModContext context)
    {
        Log = context.Logger;
        Context = context;
        Config = context.GetConfig<Config>();
        s_harmony = new Harmony("com.neverify.settings-keybind");

        Keybinds.Register();
    }

    public static void OnGameReady()
    {
        if (s_harmony == null)
            return;

        var patcher = new Patcher(s_harmony, Log);

        patcher.Patch(
            typeof(Main),
            "DrawInterface_29_SettingsButton",
            prefix: Patcher.GetHarmonyMethod(typeof(SettingsButtonPatch), nameof(SettingsButtonPatch.Prefix))
        );
    }

    public void OnConfigChanged() { }

    public void Unload()
    {
        s_harmony.UnpatchAll("com.neverify.settings-keybind");
        Log.Info("Unloaded.");
    }
}
