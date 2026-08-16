using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(CommonDropWithRerolls), nameof(CommonDropWithRerolls.TryDroppingItem))]
internal sealed class CommonDropWithRerollsPatch
{
    private static bool Prepare() => Mod.Instance != null;

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
