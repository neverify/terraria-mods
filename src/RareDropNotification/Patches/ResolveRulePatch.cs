using RareDropNotification.Features;
using Terraria.GameContent.ItemDropRules;

namespace RareDropNotification.Patches;

internal sealed class ResolveRulePatch
{
#pragma warning disable IDE1006 // Naming Styles
    internal static void Postfix(IItemDropRule rule, ItemDropAttemptResult __result)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (!Mod.Config.Enabled)
            return;

        if (__result.State is ItemDropAttemptResultState.Success && rule is CommonDrop drop)
            DropNotification.HandleDrop(drop);
    }
}
