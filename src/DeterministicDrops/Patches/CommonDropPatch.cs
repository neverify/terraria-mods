using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class CommonDropPatch
{
    public static bool TryDroppingItemPrefix(DropAttemptInfo info, CommonDrop __instance, ref ItemDropAttemptResult __result)
    => TryDroppingPatchHelper.HandleDrop(info, __instance, DropScheduler.ProcessCommonDrop, ref __result);
}
