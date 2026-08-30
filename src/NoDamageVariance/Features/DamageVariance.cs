using Terraria.Testing;

namespace NoDamageVariance.Features;

internal static class DamageVariance
{
    public static void Update() =>
        DebugOptions.NoDamageVar = Mod.Instance.Config.DisableDamageVariance;
}
