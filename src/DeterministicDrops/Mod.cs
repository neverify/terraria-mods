using DeterministicDrops.DropEngine;
using TerrariaModder.Core;
using Utils;

namespace DeterministicDrops;

public class Mod : ModBase<Mod, Config>, IModLifecycle
{
    public override string Id => "deterministic-drops";
    public override string Name => "Deterministic Drops";
    public override string Version => "1.1.1";

    internal DropStateStore DropStateStore { get; private set; }

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
}
