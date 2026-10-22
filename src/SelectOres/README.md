# Select Ores

Select which ores are generated in your world during worldgen and when smashing altars.

In Terraria the alternative ores are usually strictly superior to their original variants. Pickaxes have lower use times and higher pickaxe power, swords and bows deal more damage etc. Thus playing in a world with the alternative ores meaningfully affects game difficulty. For example, having platinum instead of gold will make a pre-boss melee tank build much easier to achieve.

The same applies to hardmode ores, especially when it comes to the tools. And even worse, the result of smashing altars is determined randomly, independent of the world seed. This means that it is possible to get the ores you want by smashing an altar and force-quitting the game repeatedly, which is just dumb.

And because the shimmer exists, there are no downsides to having the alternative ores either, since you can just transmute the ores to their original counterparts.

This mod fixes these frustrations by allowing selecting which ores are generated. The generation of pre-hardmode ores takes place during worldgen; outside of that the mod has no effect on them. **It won't change ores in existing worlds**. For hardmode ores, the mod intercepts the altar smashing event.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod works at least partially in multiplayer. World generation is always client-side, so pre-hardmode ore generation works. Altar smashing might work in multiplayer.

## Configuration

### Override World Generation Ores

Whether the selected ores are enforced during world generation.

### Tier 1 Ore

Which tier 1 ore to generate. Possible values: `Copper`, `Tin`.

### Tier 2 Ore

Which tier 2 ore to generate. Possible values: `Iron`, `Lead`.

### Tier 3 Ore

Which tier 3 ore to generate. Possible values: `Silver`, `Tungsten`.

### Tier 4 Ore

Which tier 4 ore to generate. Possible values: `Gold`, `Platinum`.

### Override Altar Smashing Ores

Whether the selected ores are enforced when smashing altars.

### Tier 1 Hardmode Ore

Which tier 1 hardmode ore to generate. Possible values: `Cobalt`, `Palladium`.

### Tier 2 Hardmode Ore

Which tier 2 hardmode ore to generate. Possible values: `Mythril`, `Orichalcum`.

### Tier 3 Hardmode Ore

Which tier 3 hardmode ore to generate. Possible values: `Adamantite`, `Titanium`.

## Technical Overview

### Pre-Hardmode Ores

The method `WorldGen.Reset()` randomizes the ores that will be used in world generation.

The mod overrides these randomized ores with the configured ones. The `Reset()` method is the only place where this randomization occurs, so it is sufficient to simply overwrite it once.

### Hardmode Ores

Hardmode ore tiers are randomized by the `WorldGen.SmashAltar()` method. The randomization only takes place when the `WorldGen.SavedOreTiers` value for the current tier is unset.

The mod prevents this randomization from occurring by setting the `SavedOreTiers` values to the configured ores before the method is run.
