using FullBright.Features;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace FullBright;

public class Mod : IMod
{
    public string Id => "full-bright";
    public string Name => "Fullbright";
    public string Version => "1.1.0";

    internal static Mod Instance { get; private set; }

    internal ILogger Log { get; private set; }
    internal Config Config { get; private set; }

    public void Initialize(ModContext context)
    {
        Instance = this;

        Log = context.Logger;
        Config = context.GetConfig<Config>();
    }

    public static void OnGameReady() => LightingQuality.SetQuality();

    public void OnConfigChanged() => LightingQuality.SetQuality();

    public void Unload() { }
}
