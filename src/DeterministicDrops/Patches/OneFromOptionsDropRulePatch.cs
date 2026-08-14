using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class OneFromOptionsDropRulePatch
{
    public static bool TryDroppingItemPrefix(
        DropAttemptInfo info,
        OneFromOptionsDropRule __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessOneFromOptionsDropRule,
            ref __result
        );
}
