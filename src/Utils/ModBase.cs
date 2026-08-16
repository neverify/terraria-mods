using TerrariaModder.Core;
using TerrariaModder.Core.Config;
using TerrariaModder.Core.Logging;

namespace Utils;

public abstract class ModBase<TMod>
    where TMod : ModBase<TMod>
{
    internal static TMod Instance { get; private set; }

    protected void SetInstance() => Instance = (TMod)this;
}

public abstract class ModBase<TMod, TConfig> : ModBase<TMod>, IMod
    where TMod : ModBase<TMod, TConfig>
    where TConfig : ModConfig
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Version { get; }

    internal ILogger Log { get; private set; }
    internal TConfig Config { get; private set; }
    internal ModContext Context { get; private set; }

    void IMod.Initialize(ModContext context)
    {
        SetInstance();

        Log = context.Logger;
        Config = context.GetConfig<TConfig>();
        Context = context;

        Initialize();
    }

    protected virtual void Initialize() { }

    // Awaiting https://github.com/Inidar1/terraria-modder/issues/15.
    // public virtual void OnConfigChanged() { }

    public virtual void Unload() { }
}
