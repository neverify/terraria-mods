using RareDropNotification.Features;
using Terraria.GameContent.ItemDropRules;

namespace RareDropNotification.Patches;

internal sealed class ResolveRulePatch
{
    internal static void Postfix(IItemDropRule rule, ItemDropAttemptResult __result)
    {
        if (!Mod.Config.Enabled)
            return;

        if (__result.State is ItemDropAttemptResultState.Success && rule is CommonDrop drop)
            DropNotification.HandleDrop(drop);
    }
}
