using System;
using System.Collections.Generic;
using System.Linq;
using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class TryDroppingPatchHelper
{
    public static bool HandleDrop<IItemDropRule>
    (DropAttemptInfo info, IItemDropRule __instance, Func<IItemDropRule, DropStateStore, IEnumerable<DropScheduler.Drop>> dropRuleProcessor, ref ItemDropAttemptResult __result)
    {
        if (!Mod.Instance.Config.EnableDeterministicDrops)
            return true;

        var drops = dropRuleProcessor(__instance, Mod.Instance.DropStateStore).ToArray();

        foreach (var result in drops)
            CommonCode.DropItemFromNPC(info.npc, result.ItemId, result.Amount);

        __result = new ItemDropAttemptResult
        {
            State = drops.Length != 0
                ? ItemDropAttemptResultState.Success
                : ItemDropAttemptResultState.FailedRandomRoll
        };

        return false;
    }
}
