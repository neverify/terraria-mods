using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;
using Utils;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(CommonDropWithRerolls), nameof(CommonDropWithRerolls.TryDroppingItem))]
internal sealed class CommonDropWithRerollsPatch : Patch<Mod>
{
    private static bool Prefix(
        DropAttemptInfo info,
        CommonDropWithRerolls __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessCommonDropWithRerolls,
            ref __result
        );
}
