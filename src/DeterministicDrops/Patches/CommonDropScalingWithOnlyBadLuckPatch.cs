using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class CommonDropScalingWithOnlyBadLuckPatch
{
    public static bool TryDroppingItemPrefix(
        DropAttemptInfo info,
        CommonDropScalingWithOnlyBadLuck __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessCommonDropScalingWithOnlyBadLuck,
            ref __result
        );
}
