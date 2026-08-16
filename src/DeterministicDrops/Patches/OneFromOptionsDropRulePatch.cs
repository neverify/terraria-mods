using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(OneFromOptionsDropRule), nameof(OneFromOptionsDropRule.TryDroppingItem))]
internal sealed class OneFromOptionsDropRulePatch
{
    private static bool Prepare() => Mod.Instance is not null;

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
