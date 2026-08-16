using MinimapMode.Features;
using TerrariaModder.Core;
using Utils;

namespace MinimapMode;

public class Mod : ModBase<Mod, Config>, IModLifecycle
{
    public override string Id => "minimap-mode";
    public override string Name => "Minimap Mode";
    public override string Version => "1.0.2";

    public void OnWorldLoad()
    {
        if (!Config.ForceMinimapMode)
            return;

        SetMinimapMode.SetMode();
    }

    public void OnContentReady(ModContext context) { }

    public void OnWorldUnload() { }

    public void OnConfigChanged() { }
}
