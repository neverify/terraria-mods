using Utils;

namespace SelectOres;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "select-ores";
    public override string Name => "Select Ores";
    public override string Version => "1.0.1";

    public void OnConfigChanged() { }
}
