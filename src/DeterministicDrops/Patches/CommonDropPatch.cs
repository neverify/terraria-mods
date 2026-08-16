using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(CommonDrop), nameof(CommonDrop.TryDroppingItem))]
internal sealed class CommonDropPatch
{
    private static bool Prepare() => Mod.Instance != null;

    private static bool Prefix(
        DropAttemptInfo info,
        CommonDrop __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessCommonDrop,
            ref __result
        );
}
