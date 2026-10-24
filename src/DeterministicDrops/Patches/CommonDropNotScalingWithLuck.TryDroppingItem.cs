using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(
    typeof(CommonDropNotScalingWithLuck),
    nameof(CommonDropNotScalingWithLuck.TryDroppingItem)
)]
internal static class CommonDropNotScalingWithLuckPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static bool Prefix(
        DropAttemptInfo info,
        CommonDropNotScalingWithLuck __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessCommonDropNotScalingWithLuck,
            ref __result
        );
}
