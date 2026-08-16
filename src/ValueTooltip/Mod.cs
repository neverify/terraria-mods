using Utils;

namespace ValueTooltip;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "value-tooltip";
    public override string Name => "Value Tooltip";
    public override string Version => "1.0.1";

    public void OnConfigChanged() { }
}
