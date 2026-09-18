using EnableDebugCommands.Features;
using Utils;

namespace EnableDebugCommands;

public class Mod : ModBase<Mod, Config>
{
    protected override void OnInitialize() => DebugCommands.Update();

    public void OnConfigChanged() => DebugCommands.Update();
}
