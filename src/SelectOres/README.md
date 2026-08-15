# Select Ores

Select which ores are generated in your world during worldgen and when smashing altars.

In Terraria the alternative ores are usually strictly superior to their original variants. Pickaxes have lower use times and higher pickaxe power, swords and bows deal more damage etc. Thus playing in a world with the alternative ores meaningfully affects game difficulty. For example, having platinum instead of gold will make a pre-boss melee tank build much easier to achieve.

The same applies to hardmode ores, especially when it comes to the tools. And even worse, the result of smashing altars is determined randomly, independent of the world seed. This means that it is possible to get the ores you want by smashing an altar and force-quitting the game repeatedly, which is just dumb.

And because the shimmer exists, there are no downsides to having the alternative ores either, since you can just transmute the ores to their original counterparts.

This mod fixes these frustrations by allowing selecting which ores are generated. The generation of pre-hardmode ores takes place during worldgen; outside of that the mod has no effect on them. **It won't change ores in existing worlds**. For hardmode ores, the mod intercepts the altar smashing event.

## Configuration

### Override Generation

Whether the selected ores are enforced during world generation and when smashing altars.

### Tier 1 Ore

Which tier 1 ore to generate. Possible values: `Copper`, `Tin`.

### Tier 2 Ore

Which tier 2 ore to generate. Possible values: `Iron`, `Lead`.

### Tier 3 Ore

Which tier 3 ore to generate. Possible values: `Silver`, `Tungsten`.

### Tier 4 Ore

Which tier 4 ore to generate. Possible values: `Gold`, `Platinum`.

### Tier 1 Hardmode Ore

Which tier 1 hardmode ore to generate. Possible values: `Cobalt`, `Palladium`.

### Tier 2 Hardmode Ore

Which tier 2 hardmode ore to generate. Possible values: `Mythril`, `Orichalcum`.

### Tier 3 Hardmode Ore

Which tier 3 hardmode ore to generate. Possible values: `Adamantite`, `Titanium`.

## Development

### Harmony Patches

#### `Terraria.WorldGen.Reset()`

```cs
public static void Reset()
{
    // ...

    GenVars.copper = 7;
    GenVars.iron = 6;
    GenVars.silver = 9;
    GenVars.gold = 8;

    // ...

    WorldGen.SavedOreTiers.Copper = 7;
    WorldGen.SavedOreTiers.Iron = 6;
    WorldGen.SavedOreTiers.Silver = 9;
    WorldGen.SavedOreTiers.Gold = 8;
    GenVars.copperBar = 20;
    GenVars.ironBar = 22;
    GenVars.silverBar = 21;
    GenVars.goldBar = 19;

    // ...

    if (WorldGen.genRand.Next(2) == 0)
    {
        GenVars.copper = 166;
        GenVars.copperBar = 703;
        WorldGen.SavedOreTiers.Copper = 166;
    }
    if ((!WorldGen.dontStarveWorldGen || WorldGen.drunkWorldGen) && WorldGen.genRand.Next(2) == 0)
    {
        GenVars.iron = 167;
        GenVars.ironBar = 704;
        WorldGen.SavedOreTiers.Iron = 167;
    }
    if (WorldGen.genRand.Next(2) == 0)
    {
        GenVars.silver = 168;
        GenVars.silverBar = 705;
        WorldGen.SavedOreTiers.Silver = 168;
    }
    if ((!WorldGen.dontStarveWorldGen || WorldGen.drunkWorldGen) && WorldGen.genRand.Next(2) == 0)
    {
        GenVars.gold = 169;
        GenVars.goldBar = 706;
        WorldGen.SavedOreTiers.Gold = 169;
    }

    // ...
}
```

This method resets the state of the worldgen engine. It first sets the generated ores to their original ids, then randomly assigns the new ores in their place. The values are randomized again later for secret seeds, such as the drunk seed, but that doesn't change the patch logic.

The mod applies a postfix patch to this method to override the `GenVars` and `WorldGen.SavedOreTiers` values to the configured ones.

The patch uses a dictionary to map the config keys to the tile and item ids.

#### `Terraria.WorldGen.SmashAltar()`

```cs
public static void SmashAltar(int i, int j) {
    // ...

    if (WorldGen.SavedOreTiers.Cobalt == -1)
    {
        WorldGen.SavedOreTiers.Cobalt = 107;
        if (WorldGen.genRand.Next(2) == 0)
        {
            WorldGen.SavedOreTiers.Cobalt = 221;
        }
    }

    // ...

    if (WorldGen.SavedOreTiers.Mythril == -1)
    {
        WorldGen.SavedOreTiers.Mythril = 108;
        if (WorldGen.genRand.Next(2) == 0)
        {
            WorldGen.SavedOreTiers.Mythril = 222;
        }
    }

    // ...

    if (WorldGen.SavedOreTiers.Adamantite == -1)
    {
        WorldGen.SavedOreTiers.Adamantite = 111;
        if (WorldGen.genRand.Next(2) == 0)
        {
            WorldGen.SavedOreTiers.Adamantite = 223;
        }
    }

    // ...
}
```

This method runs whenever an altar is smashed. It excecutes these paths based on the total number of altars smashed. When a given tier is selected, the method first checks if the related `WorldGen.SavedOreTiers` value is -1 (indicating it is the first altar of that tier), and if so, randomizes the generated ore.

The mod applies a prefix patch onto this method, which checks whether the `WorldGen.SavedOreTiers` value for each ore is -1. If it is, the value is overridden to the configured ore. This way the randomization step is skipped altogether. This check also prevents unnecessarily overriding the value each time the method is called. While the performance impact would obviously be negligible, the check makes the logic a bit cleaner.

The patch uses a dictionary to map the config keys to the tile ids.
