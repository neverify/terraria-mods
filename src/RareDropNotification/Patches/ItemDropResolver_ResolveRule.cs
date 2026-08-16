using HarmonyLib;
using RareDropNotification.Features;
using Terraria.GameContent.ItemDropRules;
using Utils;

namespace RareDropNotification.Patches;

[HarmonyPatch(typeof(ItemDropResolver), "ResolveRule")]
internal sealed class ResolveRulePatch : Patch<Mod>
{
    private static void Postfix(IItemDropRule rule, ItemDropAttemptResult __result)
    {
        if (!Mod.Instance.Config.Enabled)
            return;

        if (__result.State is ItemDropAttemptResultState.Success && rule is CommonDrop drop)
            DropNotification.HandleDrop(drop);
    }
}
