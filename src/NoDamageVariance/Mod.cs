using NoDamageVariance.Features;
using Terraria.Testing;
using Utils;

namespace NoDamageVariance;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "no-damage-variance";
    public override string Name => "No Damage Variance";
    public override string Version => "1.0.0";

    protected override void Initialize() =>
        DebugOptions.NoDamageVar = Instance.Config.DisableDamageVariance;

    public void OnConfigChanged() => DamageVariance.Update();
}
