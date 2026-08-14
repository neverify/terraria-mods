using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace SelectOres;

public class Mod : IMod
{
    public string Id => "select-ores";
    public string Name => "Select Ores";
    public string Version => "1.0.0";

    internal static ILogger Log;
    internal static Config Config;

    public void Initialize(ModContext context)
    {
        Log = context.Logger;
        Config = context.GetConfig<Config>();
    }

    public void Unload() { }
}
