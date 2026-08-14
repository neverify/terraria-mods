using DeterministicDrops.DropEngine;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

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

    internal DropStateStore DropStateStore { get; private set; }

    public void Initialize(ModContext context)
    {
        Instance = this;

        Log = context.Logger;
        Config = context.GetConfig<Config>();
        Context = context;
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

    public void Unload() { }
}
