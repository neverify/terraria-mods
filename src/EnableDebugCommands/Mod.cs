using EnableDebugCommands.Features;
using Utils;

namespace EnableDebugCommands;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "enable-debug-commands";
    public override string Name => "Enable Debug Commands";
    public override string Version => "1.0.0";

    protected override void Initialize() => DebugCommands.Update();

    public void OnConfigChanged() => DebugCommands.Update();
}
