using HarmonyLib;
using RareDropNotification.Features;
using Terraria.GameContent.ItemDropRules;

namespace RareDropNotification.Patches;

[HarmonyPatch(typeof(ItemDropResolver), "ResolveRule")]
internal sealed class ResolveRulePatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Postfix(IItemDropRule rule, ItemDropAttemptResult __result)
    {
        if (!Mod.Instance.Config.Enabled)
            return;

        if (__result.State is ItemDropAttemptResultState.Success && rule is CommonDrop drop)
            DropNotification.HandleDrop(drop);
    }
}
