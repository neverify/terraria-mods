# Deterministic Drops

Reduce variance in item drops while preserving vanilla drop rates.

The NPC drop system in vanilla terraria uses a bernoulli trial to determine if items should be dropped. This results in the drops following a binomial distribution, which allows for cases of extreme luck and unluck. In practise this means that you can kill 1000 Chaos Elementals while not obtaining a single Rod of Discord – annoying!

This mod fixes this issue. Instead of drops being determined by a random chance, each kill of an NPC adds progress towards obtaining each item it can drop according to the drop rate. This means that killing NPCs with higher odds of dropping an item adds more progress than NPCs with a lower drop rate. For example, killing a Flying Dutchman contributes 67 (I'm not joking) regular pirates worth of progress towards obtaining any of the Lucky Ring parts.

This mod does not eliminate randomness completely though. Where each item will be dropped within each progress cycle is randomized, so you won't know exactly when you will obtain each item. The total drop rate remains the same as in vanilla. The randomization is deterministic and is based on the world seed and the item being dropped. Two worlds with the same seed drop items at the same progress in each cycle.

Drop amounts are also made deterministic by the mod. The dropped amounts cycle between all possible values. For example a Blue Slime can drop 1-2 Gel. This means each cycle first drops either 1 or 2 Gel, then the other amount.

This mod also takes luck into account. When an NPC dies, the progress towards each drop is increased or decreased according to the players luck. The effect of luck is exactly that of vanilla.

This mod does not affect all NPC drops – at least not yet. Boss and Mimic drops that choose from a set of possible items are not affected by this mod. Drops that do not scale with luck are also not altered. An example of a drop that is not scaled with luck is Feathers from Harpies with a 1/2 chance. I do intend to implement these features into the mod down the line though.

## Configuration

### Enable Deterministic Drops

Enable the deterministic item drop system.

When this option is enabled, kills count towards the progress of each item, and item drops are handled by the mod. When the setting is disabled, the vanilla systems are used, effectively "pausing" the mod. You can't "miss" drops due to the mod being disabled.

## Development

### Harmony Patches

#### `Terraria.GameContent.ItemDropRules.CommonDrop.TryDroppingItem()`

```cs
public virtual ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
{
    if (info.player.RollLuck(chanceDenominator) < chanceNumerator)
    {
        CommonCode.DropItemFromNPC(info.npc, itemId, info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1));
        return new ItemDropAttemptResult
        {
            State = ItemDropAttemptResultState.Success
        };
    }
    return new ItemDropAttemptResult
    {
        State = ItemDropAttemptResultState.FailedRandomRoll
    };
}
```

This method handles dropping almost all items in the game, bar the ones mentioned in the mod's description. It rolls a random number and compares it to the drop chance to determine if an item should be dropped. The dropped amount is also randomized. The `RollLuck` method applies the effect of luck on the denominator of the drop chance.

The mod applies a prefix patch onto this method to completely replace it with the custom logic.

#### `Terraria.IO.WorldFile.SaveWorld()`

```cs
public static void SaveWorld(bool resetTime = false, bool useTemps = false, bool canBeSkipped = false)
{
    try
    {
        _SaveWorld(_isWorldOnCloud, resetTime, useTemps, canBeSkipped);
    }
    catch (Exception exception)
    {
        FancyErrorPrinter.ShowFileSavingFailError(exception, Main.WorldPath);
        throw;
    }
}
```

This method handles saving the world on exit and when an autosave occurs.

The mod applies a prefix patch onto this method to save the drop progress. Since the save method is also called in the `OnWorldUnload()` lifecycle hook, this patch only handles autosaves. Since the mod's `DropStateStore` instance is cleared in the unload hook, the patch uses a null-conditional operator to call the save method. This call only goes through during autosaves, when the `DropStateStore` instance is not `null`.

### Drop System

This mod is surprisingly complex for what it achieves. Here is a breakdown of all of the main components of the mod.

#### `DropState`

The `DropState` class represents the state of a single item's drop progress. It contains only two properties to store this information, the total progress `DropProgress` and the cycle of the next awarded drop `NextDropCycle`. `DropProgress` is a `double` representing the accumulated chance of each item. `NextDropCycle` is an `int` representing the cycle when the next drop of this item will be awarded. This value is required, because it is possible for a single kill to advance the progress towards an item enough to pass two cycle thresholds at once. If this value was not stored, these situations would only drop the item once.

The `AddProgress()` and `AdvanceCycle()` methods are used to update the drop progress, abstracting the implementation.

Technically per-item progress is stateless, because the mod could compute the total accumulated progress by summing up all NPC kills of the player. However, this would not only be very inefficient, but also would make taking luck into account impossible, since it's not possible to reconstruct the luck of the player during each kill.

#### `DropStateStore`

The `DropStateStore` class serves as an abstraction layer for storing the per-item `DropState` instances.

`DropStateStore` provides the `Get()` method for retrieving the state of a specific item. Since the per-item progress is stored in a `DropState` instance, it is passed as reference and the methods of `DropState` can be used to update the values. This is why there is no `Set()` method.

`DropStateStore` stores the `DropState` instances in a dictionary, keyed by the item ID. Permanent storage is implemented as a per-world json file. The `Load()` and `Save()` methods handle the serialization and deserialization of the drop states as well as reading and writing the json files.

#### `DropScheduler`

The `DropScheduler` class is responsible for the logic of item drops. It has two main responsibilities:

1. Determine the progress at which to award the drop for each cycle and the amount that should be dropped.
2. Given an item, update the progress and determine whether a drop should be awarded.

To deterministically get the progress for each cycle the class uses the hash of the world seed, the item id and the cycle as a seed to an RNG. This allows for deterministically obtaining the progress without needing to store the RNG state.

Updating the progress would be fairly straightforward if it wasn't for the luck mechanic. Accounting for luck is the most complicated part about the logic as the luck mechanic is not trivial to reverse. It is not enough to calculate the average change to the denominator of the drop chance because of the very nice statistical property that `1/E[X] != E[1/X]` (I love statistics). Because of this, we need to effectively simulate the luck rolls.

Luckily enough the beautiful people of the Terraria Wiki have constructed a ready to use formula to calculate the average effect of luck on a given drop chance. Unfortunately the formula includes a sum of size proportional to the drop chance denominator. This means a worst-case scenario of summing 20000 doubles. While this is definitely within the bounds of a reasonable calculation in terms of processing time, it is still not ideal. To counter this, we build a lookup table for all possible sums. Because the elements of the sums form a harmonic series, the table is both trivial to compute and quite efficient to store. In the end storing 20000 elements of the harmonic series which takes approximately 160 KiB of memory – a worthy tradeoff in my opinion to make the calculation O(1) time complexity. To optimize this lookup table, it should be possible to store only the values actually present in the game, since currently the majority of the computed values are unused.

Once luck has been factored into the progress, it is simply added to the total drop progress for that item.

To determine whether a drop should occur, the mod checks if the current total progress is greater than or equal to the progress threshold for that cycle. If it is, the item is dropped and the cycle is advanced by one.

The amount of items dropped is also determined by the cycle. First an array of possible drop amounts is created. It is then shuffled seeded by the floor division of the current cycle and the range of possible amounts. This way the array is exhausted before creating a new one. Then the amount is chosen from the array based on the modulus of the current cycle and the range of possible amounts. The result is that each possible dropped amount is encountered exactly once in a random order, after which they are randomized again.

### Randomization

In order to avoid having to store the RNG state the randomization logic is built to be stateless. To achieve this, only the first few random numbers of each RNG instance are used. To have variance between the instances while staying deterministic, the RNGs have to be seeded. Since the seed should be effectively unique for each combination of world seed, item id and cycle, a hash algorithm is used. The hash algorithm used in the mod is SplitMix64. It is an incredibly simple and fast hash algorithm with a high avalanche effect (small changes in input produce large changes in output). Since SplitMix64 is 64-bit, but the RNG we use (System.Random) takes 32-bit seeds, we XOR the lower and upper 32 bits of the hash value to obtain a 32-bit seed.
