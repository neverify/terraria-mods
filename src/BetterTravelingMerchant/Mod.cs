using BetterTravelingMerchant.Patches;
using HarmonyLib;
using Terraria;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;
using Utils;

namespace BetterTravelingMerchant;

public class Mod : IMod
{
    public string Id => "better-traveling-merchant";
    public string Name => "Better Traveling Merchant";
    public string Version => "1.0.0";

    internal static Mod Instance { get; private set; }

    internal ILogger Log { get; private set; }
    internal Config Config { get; private set; }
    private Harmony _harmony;

    public void Initialize(ModContext context)
    {
        Instance = this;

        Log = context.Logger;
        Config = context.GetConfig<Config>();
        _harmony = new Harmony("com.neverify.deterministic-drops");
    }

    public static void OnGameReady()
    {
        var patcher = new Patcher(Instance._harmony, Instance.Log);

        patcher.Patch(
            typeof(Main),
            "UpdateTime",
            postfix: Patcher.GetHarmonyMethod(typeof(UpdateTimePatch), nameof(UpdateTimePatch.Postfix))
        );

        patcher.Patch(
            typeof(Chest),
            "SetupTravelShop",
            postfix: Patcher.GetHarmonyMethod(typeof(SetupTravelShopPatch), nameof(SetupTravelShopPatch.Postfix))
        );
    }

    public void OnConfigChanged() { }

    public void Unload()
    {
        _harmony.UnpatchAll("com.neverify.better-traveling-merchant");
        Instance = null;

        Log.Info("Unloaded.");
    }
}
