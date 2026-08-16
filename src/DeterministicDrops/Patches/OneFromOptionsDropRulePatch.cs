using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;
using Utils;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(OneFromOptionsDropRule), nameof(OneFromOptionsDropRule.TryDroppingItem))]
internal sealed class OneFromOptionsDropRulePatch : Patch<Mod>
{
    private static bool Prefix(
        DropAttemptInfo info,
        OneFromOptionsDropRule __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessOneFromOptionsDropRule,
            ref __result
        );
}
