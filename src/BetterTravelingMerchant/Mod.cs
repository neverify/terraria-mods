using Utils;

namespace BetterTravelingMerchant;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "better-traveling-merchant";
    public override string Name => "Better Traveling Merchant";
    public override string Version => "1.0.2";

    public void OnConfigChanged() { }
}
