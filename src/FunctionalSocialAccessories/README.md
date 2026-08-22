# Functional Social Accessories

Make accessories in social slots functional.

This mod provides a simple way to increase the amount of accessories available. It makes accessories function in the social slots just like they do in the regular accessory slots.

The current implementation has the following shortcomings:

- When having wings in both a regular slot and its corresponding social slot, the stats of the wings in the social slot are used instead of the vanilla behavior of using the stats from the regular slot.
- There is no way to disable the effects of accessories in social slots.
- The tooltip "Not functional in social slot" is still displayed.

I will probably address these issues at some point. There are probably other side-effects as well – I haven't tested the mod extensively. Please report any issues you encounter using the instructions below :​)

The mod works in multiplayer.

## Configuration

### Enable Functional Social Accessories

Make social accessories function the same as regular accessories.

## Development

### Harmony Patches

#### `Terraria.Player.ApplyEquipVanity()`

```cs
private void ApplyEquipVanity(int itemSlot, Item currentItem)
```

This method handles applying the visual effects of an accessory in a social accessory slot. The implementation of the method is quite long and irrelevant, so it is not included here. This method is the counterpart of the `ApplyEquipFunctional()` method, which handles the functional effects of accessories in regular slots.

The mod applies a prefix patch to this method to invoke the `ApplyEquipFunctional()` method before applying the vanity effects. Additionally the mod invokes the `GrantPrefixBenefits()` method on the item, which handles the accessory prefix effects.

The `ApplyEquipVanity()` method includes the `itemSlot` parameter, but it is actually unused within the method. This parameter likely exists to provide parity with the signature of the `ApplyEquipFunctional()` method. In that method, the parameter is used solely to determine if the accessory equipped in that slot is currently hidden. This is done via the `hideVisibleAccessory` array, which is a boolean array of size 10, covering the armor slots and the functional accessory slots. Passing the `itemSlot` argument of the `ApplyEquipVanity()` method would therefore cause an `IndexOutOfRangeException`, since the slot IDs of social accessory slots extend beyond the array. To prevent this crash, the argument to that parameter is always passed as 0. Because the original `ApplyEquipVanity()` method is ran after the functional effects have been added, all of the vanity effects are overridden anyway.

Since the `ApplyEquipFunctional()` and `GrantPrefixBenefits()` methods are `private`, the mod uses a reverse harmony patch to access them. This allows the invocations to incur no performance penalties, which the naive implementation of using reflection would have. This is especially important because the `ApplyEquipVanity()` method is called every frame. This is the recommended pattern for calling private methods in Harmony patches.
