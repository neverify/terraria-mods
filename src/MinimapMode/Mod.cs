using MinimapMode.Features;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace MinimapMode;

public class Mod : IMod, IModLifecycle
{
    public string Id => "minimap-mode";
    public string Name => "Minimap Mode";
    public string Version => "1.0.0";

    internal static Mod Instance { get; private set; }

    internal ILogger Log { get; private set; }
    internal Config Config { get; private set; }

    public void Initialize(ModContext context)
    {
        Instance = this;

        Log = context.Logger;
        Config = context.GetConfig<Config>();
    }

    public void OnWorldLoad()
    {
        if (!Config.ForceMinimapMode)
            return;

        SetMinimapMode.SetMode();
    }

    public void OnContentReady(ModContext context) { }

    public void OnWorldUnload() { }

    public void OnConfigChanged() { }

    public void Unload() { }
}
