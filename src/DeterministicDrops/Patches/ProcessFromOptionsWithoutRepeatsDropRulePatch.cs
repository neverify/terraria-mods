using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class FromOptionsWithoutRepeatsDropRulePatch
{
    public static bool TryDroppingItemPrefix(DropAttemptInfo info, FromOptionsWithoutRepeatsDropRule __instance, ref ItemDropAttemptResult __result)
    => TryDroppingPatchHelper.HandleDrop(info, __instance, DropProcessor.ProcessFromOptionsWithoutRepeatsDropRule, ref __result);
}
