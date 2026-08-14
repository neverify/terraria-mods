using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(
    typeof(CommonDropScalingWithOnlyBadLuck),
    nameof(CommonDropScalingWithOnlyBadLuck.TryDroppingItem)
)]
internal static class CommonDropScalingWithOnlyBadLuckPatch
{
    public static bool Prefix(
        DropAttemptInfo info,
        CommonDropScalingWithOnlyBadLuck __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessCommonDropScalingWithOnlyBadLuck,
            ref __result
        );
}
