using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class CommonDropWithRerollsPatch
{
    public static bool TryDroppingItemPrefix(DropAttemptInfo info, CommonDropWithRerolls __instance, ref ItemDropAttemptResult __result)
    => TryDroppingPatchHelper.HandleDrop(info, __instance, DropProcessor.ProcessCommonDropWithRerolls, ref __result);
}
