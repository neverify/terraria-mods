# Deterministic Drops

Reduce variance in item drops while preserving vanilla drop rates.

The NPC drop system in vanilla Terraria uses a bernoulli trial to determine if items should be dropped. This results in the drops following a binomial distribution, which allows for cases of extreme luck and unluck. In practise this means that you can kill 1000 Chaos Elementals while not obtaining a single Rod of Discord – annoying!

This mod fixes this issue. Instead of drops being determined by a random chance, each kill of an NPC adds progress towards obtaining each item it can drop according to the drop rate. This means that killing NPCs with higher odds of dropping an item adds more progress than NPCs with a lower drop rate. For example, killing a Flying Dutchman contributes 67 regular pirates worth of progress towards obtaining any of the Lucky Ring parts.

The mod does not eliminate randomness completely though. Where each item will be dropped within each progress cycle is randomized, so you won't know exactly when you will obtain each item. The total drop rate remains the same as in vanilla. The randomization is deterministic and is based on the world seed and the item being dropped. Two worlds with the same seed drop items at the same progress in each cycle.

Drop amounts are also deterministic. The dropped amounts cycle between all possible values. For example a Blue Slime can drop 1-2 Gel. This means each cycle first drops either 1 or 2 Gel, then the other amount.

Luck is also taken into account. When an NPC dies, the progress towards each drop is increased or decreased according to the players luck. The effect of luck is exactly that of vanilla.

The mod supports all types of NPC drops. Normal drops will simply drop a single item. Drops that choose from a set of items – such as mimic drops and many boss drops – cycle between all the items. The placement of drops is randomized within each cycle, so you won't always get the drops in the same order. Boss bags are also supported and they follow the same logic as normal drops. There are many more drop variants within the game, and I won't list them all here. They all function in the same vein as the two basic drop types with their respective tweaks.

The drop logic attempts to follow vanilla as closely as possible, trying to not be too blatant. However, the amount of randomization is a personal preference question, so my take on how the drops should be distributed might not align with everyone's opinions. Making drops NPC-agnostic is an example of an opinionated design choice. The average drop rates are _exactly_ the same as vanilla, though. Any discrepancy is considered a bug.

## Configuration

### Enable Deterministic NPC Drops

Enable the deterministic item drop system for NPCs.

When this option is enabled, kills count towards the progress of each item, and item drops are handled by the mod. When this setting is disabled, the vanilla system is used, effectively "pausing" the mod. You can't "miss" NPC drops due to this option being disabled.

### Enable Deterministic Boss Bag Drops

Enable the deterministic item drop system for boss bags.

When this option is enabled, boss bag drops are handled by the mod. When this setting is disabled, the vanilla system is used, effectively "pausing" the mod. You can't "miss" boss bag drops due to this option being disabled.

## Development

### Harmony Patches

#### `Terraria.GameContent.ItemDropRules.IItemDropRule.TryDroppingItem()`

```cs
ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info);
```

This method handles dropping items from NPCs. The implementation varies wildly across different drop rules, which is why the code is not included here. The `DropContext` class abstracts all of the properties exhibited by the different implementations, which allows the generic drop engine to handle all the different drops.

The mod applies prefix patches onto these methods to replace them. The patches forward the `IItemDropRule` instance to the generic `TryDroppingItemPatchHelper` method. That method passes the instance with other arguments to the pseudo-overloads of `ProcessDrop`. These methods extract the relevant info from the instances to form the `DropContext` object and pass it to the generic `ProcessDrop` method.

#### `Terraria.Player.OpenBossBag()`

```cs
public void OpenBossBag(int type)
{
    bool masterMode = Main.masterMode;
    IEntitySource itemSource_OpenItem = GetItemSource_OpenItem(type);

    // Select the boss bag
    switch (type)
    {
        // Spawn the items of that boss bag
        case 3318:
        {
            if (Main.rand.Next(2) == 0)
            {
                QuickSpawnItem(itemSource_OpenItem, 2430);
            }

            if (Main.rand.Next(7) == 0)
            {
                QuickSpawnItem(itemSource_OpenItem, 2493);
            }

            // Rest of the items
        }

        // Rest of the boss bags
    }

    int num10 = -1;

    // Map the boss bag type to the corresponding NPC ID
    if (type == 3318)
    {
        num10 = 50;
    }

    // Rest of the mappings

    // Get the NPC value
    NPC nPC = new NPC();
    nPC.SetDefaults(num10);
    float value = nPC.value;

    // Initial 80%–120% multiplier
    value *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;

    // Additional increase multipliers
    if (Main.rand.Next(5) == 0)
    {
        value *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
    }

    if (Main.rand.Next(10) == 0)
    {
        value *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
    }

    if (Main.rand.Next(15) == 0)
    {
        value *= 1f + (float)Main.rand.Next(15, 31) * 0.01f;
    }

    if (Main.rand.Next(20) == 0)
    {
        value *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
    }

    // Spawn the coins
}
```

This method handles opening boss bags. It contains the items of each boss bag and the spawning logic inline. This is why all of the boss bag drops have to be manually registered in the mod.

The method contains a massive switch statement for each type of boss bag. Inside each case, the items for that boss bag are spawned if their respective random roll succeeds.

At the end the coins are spawned. The coin amount is calculated from the NPC value, which is mapped manually. The value is then randomized before being turned into coins and spawned into the player's inventory.

The mod applies a prefix patch onto this method to replace it. The patch fetches the appropriate `DropContext` array from `BossBagDatabase`, then calls the `ProcessDrop` method with them. It then spawns all of the dropped items. Finally it fetches the amount of coins from the database and spawns them.

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

The mod uses a data-driven abstraction of item drops and a generic engine to process these drops. Here is a breakdown of the different components of the engine.

#### `Drop`

The `Drop` class represents the persistent identity of a drop. This identity is used to track the progress of each drop. A drop consists of a collection of items. The collection can be any of the following types:

- A single item
- Multiple items
- Multiple item groups

A single item is the simplest case: that item will be awarded every time the drop occurs. Most drops in the game fall into this category.

Multiple items represent a drop where each of the items has an equal probability of being awarded when a drop occurs. Mimic drops and many boss drops fall into this category, for example.

Multiple item groups is currently only used by developer sets dropped from boss bags. They function the same as multiple items with the exception that instead of a single item being awarded when a drop occurs, all of the items of that group are awarded.

Internally all of these drop types are encoded by a jagged array (`short[][]`), which is the native representation of multiple item groups.

The `Drop` class implements a few properties and methods used by the drop state and selection parts of the engine. The `Count` property and the `Select()` method provide the API for accessing the items in the drop. The `Id` and `Name` properties are used to seed RNGs in the randomization parts and serialize the drop in the `DropStateStore` class respectively.

#### `DropContext`

The `DropContext` class represents the context in which a drop occurs. These include the probability of the drop, probability modifiers, drop amounts, luck type, condition and extra drops. These context properties characterize the _drop source_. Context properties are extensively used by the drop engine.

#### `GameContext`

The `GameContext` class represents the context of the game in which a drop occurs. It includes the world seed and the player's luck. These are separated from `DropContext` because they are _time-variable_.

#### `DropState`

The `DropState` class represents the state of a single item's drop progress. It contains only two properties to store this information, the total progress `DropProgress` and the cycle of the next awarded drop `NextDropCycle`. `DropProgress` is a `double` representing the accumulated chance of each item. `NextDropCycle` is an `int` representing the cycle when the next drop of this item will be awarded. This value is required, because it is possible for a single kill to advance the progress towards an item enough to pass two cycle thresholds at once. If this value was not stored, these situations would only drop the item once.

The `AddProgress()` and `AdvanceCycle()` methods are used to update the drop progress, abstracting the implementation.

Technically per-item progress is stateless, because the mod could compute the total accumulated progress by summing up all NPC kills of the player. However, this would not only be quite inefficient, but also would make taking luck into account impossible, since it's not possible to reconstruct the luck of the player during each kill.

#### `DropStateStore`

The `DropStateStore` class serves as an abstraction layer for storing the per-item `DropState` instances.

`DropStateStore` provides the `Get()` method for retrieving the state of a specific item. Since the per-item progress is stored in a `DropState` instance, it is passed as reference and the methods of `DropState` can be used to update the values. This is why there is no `Set()` method.

`DropStateStore` stores the `DropState` instances in a dictionary, keyed by `Drop.Name`. Permanent storage is implemented as a per-world json file. The `Load()` and `Save()` methods handle the serialization and deserialization of the drop states as well as reading from and writing to the json file.

#### `DropProcessor`

The `DropProcessor` class is the entry point for processing drops. There are numerous pseudo-overloads of this method that serve as the entry-points for different types of drops. These methods extract the relevant information of the drop from the `IItemDropRule` implementation. This process allows the drop engine to remain entirely generic.

`DropProcessor` uses the `DropAttempt` and `DropSelection` classes to determine if a drop should be awarded and to get which items to drop respectively.

#### `DropAttempt`

The `DropAttempt` class handles advancing the state of a drop and determining if a drop should occur. It uses the `DropChance` class to determine how much progress should be added.

A drop is awarded if the current total progress is greater than or equal to the progress threshold for that cycle. If that is the case, the cycle is advanced by one.

The threshold for each cycle is determined using an RNG seeded with the hash of the world seed, `Drop.Id` and the cycle. This allows for deterministically obtaining the progress without needing to store the RNG state.

#### `DropChance`

The `DropChance` class calculates how much progress is gained. It uses the `chanceNumerator` and `chanceDenominator` properties of the `DropContext` instance to calculate the base progress. It then applies the effect of luck and extra rolls according to their relevant modifiers.

Accounting for luck is the most complicated part about the logic as the luck mechanic is not trivial to reverse. It is not enough to calculate the average change to the denominator of the drop chance because of the very nice statistical property that `1/E[X] ≠ E[1/X]` (I love statistics). Because of this, we need to effectively simulate the luck rolls.

Luckily enough the beautiful people of the Terraria Wiki have constructed a ready to use formula to calculate the average effect of luck on a given drop chance. Unfortunately the formula includes a sum of size proportional to the drop chance denominator. This means a worst-case scenario of summing 20000 doubles. While this is definitely within the bounds of a reasonable calculation in terms of processing time, it is still not ideal. To counter this, we build a lookup table for all possible sums. Because the elements of the sums form a harmonic series, the table is both trivial to compute and quite efficient to store. In the end the mod stores the 20000 first elements of the harmonic series which takes approximately 160 KiB of memory – a worthy tradeoff in my opinion to make the calculation `O(1)` time complexity.

To optimize this lookup table, it should be possible to store only the values actually present in the game, since currently the majority of the computed values are unused. Alternatively the LUT could be made sparse, containing only every n:th value. The intermediate values could then be computed as needed. This approach would serve as a tunable compromise between memory and computation.

Next the effect of extra rolls is calculated. This is a simple enough calculation, taking advantage of the complement rule.

#### `DropSelection`

The `DropSelection` class determines which items should be dropped and how many of each item to drop.

Both the item and drop amount selection logic is very similar. They first establish which selection cycle is currently active, then shuffle the options with the selection cycle being part of the RNG seed, and finally select the correct item from the shuffled cycle.

The item selection logic is abstract from the implementation details of the `Drop` class. It uses the `Count` property to determine which index to select, and the `Select()` method as the API to get the actual dropped items.

#### `DropResult`

The `DropResult` class represents the outcome of a single drop operation, containing the ID of the dropped item and the quantity dropped. This class functions as the DTO between the drop engine and the patch item spawning logic.

#### `BossBagDatabase`

The `BossBagDatabase` class contains the drop tables of boss bags represented by `DropContext` arrays. The reason this database has to exist is because the vanilla code that handles opening boss bags does not utilize the pre-existing drop system, but instead is a massive switch statement with item spawning determined inline. Thus it is not feasible to extract any information about the drops from that method.

The class also provides the coin drops for each boss bag since the drop system is not capable of handling them. To simulate the effect of the vanilla randomization of the value, the base value is always multiplied by the average multiplier (`1.015^4`).

### Randomization

In order to avoid having to store the RNG state the randomization logic is built to be stateless. To achieve this, RNG is not shared between drops, but instead initialized with unique seeds for each drop.

To preserve variance between the instances while staying deterministic the RNGs have to be seeded. Since the seed should be effectively unique for each combination of world seed, drop ID and cycle, a hash algorithm is used.

The hash algorithm used in the mod is SplitMix64. It is an incredibly simple and fast hash algorithm with a high avalanche effect (small changes in input produce large changes in output). Since SplitMix64 is 64-bit, but the RNG we use (System.Random) takes 32-bit seeds, we XOR the lower and upper 32 bits of the hash value to obtain a 32-bit seed.
