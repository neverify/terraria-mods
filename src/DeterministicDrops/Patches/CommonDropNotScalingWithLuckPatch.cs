using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class CommonDropNotScalingWithLuckPatch
{
    public static bool TryDroppingItemPrefix(DropAttemptInfo info, CommonDropNotScalingWithLuck __instance, ref ItemDropAttemptResult __result)
    => TryDroppingPatchHelper.HandleDrop(info, __instance, DropScheduler.ProcessCommonDropNotScalingWithLuck, ref __result);
}
