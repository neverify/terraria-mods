using DeterministicDrops.DropSystem;
using DeterministicDrops.Patches;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;
using Terraria.IO;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;
using Utils;

namespace DeterministicDrops;

public class Mod : IMod, IModLifecycle
{
    public string Id => "deterministic-drops";
    public string Name => "Deterministic Drops";
    public string Version => "1.0.1";

    internal static Mod Instance { get; private set; }

    internal ILogger Log { get; private set; }
    internal Config Config { get; private set; }
    internal ModContext Context { get; private set; }
    private Harmony _harmony;

    internal DropStateStore DropStateStore { get; private set; }

    public void Initialize(ModContext context)
    {
        Instance = this;

        Log = context.Logger;
        Config = context.GetConfig<Config>();
        Context = context;
        _harmony = new Harmony("com.neverify.deterministic-drops");
    }

    public static void OnGameReady()
    {
        var patcher = new Patcher(Instance._harmony, Instance.Log);

        patcher.Patch(
            typeof(CommonDrop),
            "TryDroppingItem",
            prefix: Patcher.GetHarmonyMethod(typeof(TryDroppingItemPatch), nameof(TryDroppingItemPatch.Prefix))
        );

        patcher.Patch(
            typeof(WorldFile),
            "SaveWorld",
            prefix: Patcher.GetHarmonyMethod(typeof(SaveWorldPatch), nameof(SaveWorldPatch.Prefix))
        );
    }

    public void OnContentReady(ModContext context) { }

    public void OnWorldLoad()
    {
        DropStateStore = new DropStateStore(Context.ModFolder);
        DropStateStore.Load();
    }

    public void OnWorldUnload()
    {
        DropStateStore.Save();
        DropStateStore = null;
    }

    public void OnConfigChanged() { }

    public void Unload()
    {
        _harmony.UnpatchAll("com.neverify.deterministic-drops");
        Instance = null;

        Log.Info("Unloaded.");
    }
}
