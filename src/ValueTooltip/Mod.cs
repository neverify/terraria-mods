using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace ValueTooltip;

public class Mod : IMod
{
    public string Id => "value-tooltip";
    public string Name => "Value Tooltip";
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
