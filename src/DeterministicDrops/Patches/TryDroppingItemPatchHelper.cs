using System;
using System.Collections.Generic;
using DeterministicDrops.DropEngine;
using Terraria.GameContent.ItemDropRules;
using Utils;

namespace DeterministicDrops.Patches;

internal sealed class TryDroppingItemPatchHelper : Patch<Mod>
{
    public static bool HandleDrop<IItemDropRule>(
        DropAttemptInfo info,
        IItemDropRule __instance,
        Func<IItemDropRule, DropStateStore, IEnumerable<DropResult>> dropRuleProcessor,
        ref ItemDropAttemptResult __result
    )
    {
        if (!Mod.Instance.Config.EnableDeterministicNpcDrops)
            return true;

        var dropResults = dropRuleProcessor(__instance, Mod.Instance.DropStateStore);

        bool dropped = false;

        foreach (var dropResult in dropResults)
        {
            CommonCode.DropItemFromNPC(info.npc, dropResult.ItemId, dropResult.Amount);
            dropped = true;
        }

        __result = new ItemDropAttemptResult
        {
            State = dropped
                ? ItemDropAttemptResultState.Success
                : ItemDropAttemptResultState.FailedRandomRoll,
        };

        return false;
    }
}
