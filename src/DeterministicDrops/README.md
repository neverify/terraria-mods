# Deterministic Drops

Reduce variance in item drops while preserving vanilla drop rates.

The drop system in vanila Terraria follows a binomial distribution. This means that it is possible to go very long periods without getting a desired drop. For example, killing 1000 Chaos Elementals, yet not obtaining a single Rod of Discord – annoying!

This mod fixes this issue. Instead of drops being determined by a random chance, each kill of an NPC adds progress towards obtaining each item it can drop according to the drop rate. This means that killing NPCs with higher odds of dropping an item adds more progress than NPCs with a lower drop rate. For example, killing a Flying Dutchman contributes 67 regular pirates worth of progress towards obtaining any of the Lucky Ring parts.

The mod does not eliminate randomness completely though. Where each item will be dropped within each progress cycle is randomized, so you won't know exactly when you will obtain each item. The total drop rate remains the same as in vanilla. The randomization is deterministic and is based on the world seed and the item being dropped. Two worlds with the same seed drop items at the same progress in each cycle.

Drop amounts are also deterministic. The dropped amounts cycle between all possible values. For example a Blue Slime can drop 1-2 Gel. This means each cycle first drops either 1 or 2 Gel, then the other amount.

Luck is also taken into account. When an NPC dies, the progress towards each drop is increased or decreased according to the player's luck. The effect of luck is exactly that of vanilla.

The mod supports all types of NPC drops. Normal drops will simply drop a single item. Drops that choose from a set of items – such as mimic drops and many boss drops – cycle between all the items. The placement of drops is randomized within each cycle, so you won't always get the drops in the same order. Treasure bags are also supported and they follow the same logic as normal drops. There are many more drop variants within the game, and I won't list them all here. They all function in the same vein as the two basic drop types with their respective tweaks.

The drop logic attempts to follow vanilla as closely as possible, trying to not be too blatant. However, the amount of randomization is a personal preference question, so my take on how the drops should be distributed might not align with everyone's opinions. Making drops NPC-agnostic is an example of an opinionated design choice. The average drop rates are _exactly_ the same as vanilla, though. Any discrepancy is considered a bug.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod probably does not work in multiplayer.

## Configuration

### Enable Deterministic NPC Drops

Enable the deterministic item drop system for NPCs.

When this option is enabled, kills count towards the progress of each item, and item drops are handled by the mod. When this setting is disabled, the vanilla system is used, effectively "pausing" the mod. You can't "miss" NPC drops due to this option being disabled.

### Enable Deterministic Treasure Bags

Enable the deterministic item drop system for treasure bags.

When this option is enabled, treasure bag drops are handled by the mod. When this setting is disabled, the vanilla system is used, effectively "pausing" the mod. You can't "miss" treasure bag drops due to this option being disabled.

## Technical Overview

### NPC Drops

NPC drops implement the `GameContent.ItemDropRules.IItemDropRule` interface, with drops being resolved through the `TryDroppingItem()` method. Both the interface and method implementations vary between different kinds of drops, resulting in different logic for randomization, item selection, drop conditions, and other modifiers.

The mod replaces the vanilla resolution with a common drop engine. Each rule is converted into a shared drop description containing its items, chance, amounts, and modifiers. Instead of rolling for drops randomly, the engine adds progress towards a drop according to its drop chance, including modifiers such as luck. An item is dropped whenever the next randomized progress threshold is reached.

The progress threshold and item selection are randomized using the world seed, the drop, and the current cycle. This keeps the placement of drops unpredictable while making them deterministic: worlds with the same seed produce the same sequence of drops. Item quantities cycle through all possible values in the same way.

The system supports almost all implementations of `IItemDropRule`, including normal drops and drops that select from multiple items, as well as their variants (luck, rerolls, conditions, extra drops, etc.). The average drop rate remains the same as in vanilla.

### Treasure Bags

Treasure Bags are opened by the `Player.OpenBossBag()` method. Unlike NPC drops, the contents of each bag are written directly into a large switch statement in that method, so they do not use the normal item drop rule system. Thus they have to be manually registered in the mod.

The mod replaces this method with the same drop engine used by NPC drops. The coin drops are not randomized, instead simply dropping the average randomized amount each time.

### Saving Drop Progress

The world is saved by the `IO.WorldFile.SaveWorld()` method, whether due to an autosave or leaving a world.

The mod saves the per-world drop progress in a `json`-file before this method. The `OnWorldUnload()` lifecycle hook is used to save the progress when leaving a world, so the patch only handles autosaves. The drop progress is loaded in the `OnWorldLoad()` lifecycle hook.

### Randomization

In order to avoid having to store RNG state, the randomization logic is built to be stateless. To achieve this, RNG is not shared between drops, but instead initialized with unique seeds for each drop.

To preserve variance between the instances while staying deterministic the RNGs have to be seeded. Since the seed should be effectively unique for each combination of world seed, drop ID and cycle, a hash algorithm is used.

The hash algorithm used in the mod is SplitMix64. It is an incredibly simple and fast hash algorithm with a high avalanche effect (small changes in input produce large changes in output). Since SplitMix64 is 64-bit, but the RNG used (`System.Random`) takes 32-bit seeds, the lower and upper 32 bits of the hash value are XOR'd to obtain a 32-bit seed.
