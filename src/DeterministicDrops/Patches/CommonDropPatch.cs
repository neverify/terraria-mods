using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;
using Utils;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(CommonDrop), nameof(CommonDrop.TryDroppingItem))]
internal sealed class CommonDropPatch : Patch<Mod>
{
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
