using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;
using Utils;

namespace DeterministicDrops.Patches;

[HarmonyPatch(
    typeof(CommonDropNotScalingWithLuck),
    nameof(CommonDropNotScalingWithLuck.TryDroppingItem)
)]
internal sealed class CommonDropNotScalingWithLuckPatch : Patch<Mod>
{
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
