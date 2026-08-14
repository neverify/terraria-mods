using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class OneFromOptionsNotScaledWithLuckDropRulePatch
{
    public static bool TryDroppingItemPrefix(
        DropAttemptInfo info,
        OneFromOptionsNotScaledWithLuckDropRule __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessOneFromOptionsNotScaledWithLuckDropRule,
            ref __result
        );
}
