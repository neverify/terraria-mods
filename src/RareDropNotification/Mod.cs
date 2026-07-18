using HarmonyLib;
using RareDropNotification.Patches;
using Terraria.GameContent.ItemDropRules;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;
using Utils;

namespace RareDropNotification;

public class Mod : IMod
{
    public string Id => "rare-drop-notification";
    public string Name => "Rare Drop Notification";
    public string Version => "1.0.0";

    internal static ILogger Log;
    internal static Config Config;
    private static Harmony s_harmony;

    public void Initialize(ModContext context)
    {
        Log = context.Logger;
        Config = context.GetConfig<Config>();
        s_harmony = new Harmony("com.neverify.rare-drop-notification");
    }

    public static void OnGameReady()
    {
        if (s_harmony == null)
            return;

        var patcher = new Patcher(s_harmony, Log);

        patcher.Patch(
            typeof(ItemDropResolver),
            "ResolveRule",
            postfix: Patcher.GetHarmonyMethod(typeof(ResolveRulePatch), nameof(ResolveRulePatch.Postfix))
        );
    }

    public void OnConfigChanged() { }

    public void Unload()
    {
        s_harmony.UnpatchAll("com.neverify.rare-drop-notification");
        Log.Info("Unloaded.");
    }
}
