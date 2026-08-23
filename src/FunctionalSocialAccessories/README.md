# Functional Social Accessories

Make accessories in social slots functional.

This mod provides a simple way to increase the amount of accessories available. It makes accessories function in social slots just like they do in regular accessory slots.

The vanity effects of accessories in social slots are unchanged – they are always visible, just like in vanilla.

The mod works in multiplayer.

## Configuration

### Enable Functional Social Accessories

Make social accessories function the same as regular accessories.

## Development

### Harmony Patches

#### `Terraria.Player.ApplyEquipFunctional()`

```cs
private void ApplyEquipFunctional(int itemSlot, Item currentItem)
```

This method handles applying the effects of accessories. The code is not included here, because it is not relevant to the patch. The method is basically a huge switch statement for every type of accessory in the game. Some of the cases apply direct modifications to player stats, such as damage and critical strike chance, but most of them simply turn on a switch. These switches are then handled in the `UpdateEquips()` method.

```cs
public void UpdateEquips(int i)
{
    // ...

    // Apply accessory prefix effects (warding, menacing, lucky etc.).
    // Apply armor stats (defense, damage boosts etc.).
    for (int k = 0; k < 10; k++)
    {
        Item effectiveArmor = GetEffectiveArmor(k);
        if (!effectiveArmor.IsAir && IsItemSlotUnlockedAndUsable(k) && (!effectiveArmor.expertOnly || Main.expertMode) && UpdateEquips_CanItemGrantBenefits(k, effectiveArmor))
        {
            if (effectiveArmor.accessory)
            {
                GrantPrefixBenefits(effectiveArmor);
            }
            GrantArmorBenefits(effectiveArmor);
        }
    }

    // ...

    // Apply the effects of accessories in functional accessory slots (3-9).
    for (int m = 3; m < 10; m++)
    {
        if (IsItemSlotUnlockedAndUsable(m))
        {
            ApplyEquipFunctional(m, GetEffectiveArmor(m));
        }
    }

    // Apply the effects of some accessories based on the flags set by `ApplyEquipFunctional()`.

    // Apply the vanity effects of accessories in social slots (13-19).
    for (int n = 13; n < 20; n++)
    {
        if (IsItemSlotUnlockedAndUsable(n))
        {
            ApplyEquipVanity(n, GetEffectiveArmor(n));
        }
    }

    // Apply the rest of the accessory effects based on the flags set by `ApplyEquipFunctional()`.
}
```

This method handles the application of stat boosts and other effects of both armor and accessories. The method first applies the stat boosts of prefixes and armor-specific effects of both armor and accessories. It then enumerates the other effects of the equipped accessories using the `ApplyEquipFunctional()` method. After that some accessories have their effects applied, based on the flags set by `ApplyEquipFunctional()`. Notably the items that have their effects applied at this point is only a subset of all items, and I have no idea why they are handled at this point. After those few items are handled, the vanity effects of accessories in social slots are handled. Only then are the rest of the flag-based effects applied.

Because of this weird order in which the effects are applied, patching the `ApplyEquipVanity()` would mean those few accessories that are handled before the vanity effects of accessories in the social slots are handled wouldn't be handled properly. This is why the `ApplyEquipFunctional()` method is patched instead.

The mod applies a prefix patch to the `ApplyEquipFunctional()` method to apply the functional effects of the accessory in the social counterpart of each functional slot. The patch first checks if the item in the corresponding social slot fulfills the necessary conditions, after which the `GrantPrefixBenefits()`, `GrantArmorBenefits()` and `ApplyEquipFunctional()` methods are invoked for it. Because the patch is a prefix, the `ApplyEquipFunctional()` method of the functional accessory slot is called after the social slot. This allows wings to retain their normal logic of the stats of the wings in the functional slot being used.

The `ApplyEquipFunctional()` method uses the `itemSlot` parameter to determine if the accessory equipped in that slot is currently hidden. This is done via the `hideVisibleAccessory` array, which is a boolean array of size 10, covering the armor slots and the functional accessory slots. Passing the slot ID of the social slot would cause an `IndexOutOfRangeException`, since the slot IDs of social accessory slots extend beyond the array. This is why the `ìtemSlot` parameter of the functional slot is passed instead. Because the `ApplyEquipVanity()` method is ran after the functional effects have been added, the effects of that flag get overridden anyway.

Since the `ApplyEquipFunctional()`, `GrantPrefixBenefits()` and `GrantArmorBenefits()` methods are `private`, the mod uses a reverse harmony patch to access them. This allows the invocations to incur no performance penalties, which the naive implementation of using reflection would have. This is especially important because the `ApplyEquipVanity()` method is called every frame. This is the recommended pattern for calling private methods in harmony patches.

#### `Terraria.Main.MouseText_DrawItemTooltip_GetLinesInfo()`

```cs
public static void MouseText_DrawItemTooltip_GetLinesInfo(Item item, ref int yoyoLogo, float oldKB, ref int numLines, string[] toolTipLine, Color[] lineColors)
{
    // ...

    // Add the "Not Functional in Social Slot" tooltip
    if (item.social && !item.vanity && !item.hasVanityEffects)
    {
        toolTipLine[numLines] = Lang.tip[61].Value;
        numLines++;
    }
}
```

This method handles building the tooltips of items. When an item is in a social slot but not a vanity item, it adds a tooltip indicating that it is not functional in that slot. Because the mod changes this behavior, that tooltip is inaccurate.

The mod applies a prefix and postfix patch to the method to prevent this tooltip line from being added. This is achieved by making the condition to add that line never pass. To do that, the `item.social` property is set `false` in the prefix. Because object instances are always passed as references in C#, forgoing cleanup would alter the item in-game. This is why a postfix patch is implemented to revert the value of the property after the method has ran. To do this the prefix patch stores and passes the original value of the property using the harmony `__state` parameter. Because the method does not use the `item.social` property anywhere else, the patches have no side-effects.
