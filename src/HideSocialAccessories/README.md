# Hide Social Accessories

Hide accessories in social slots on the player.

Accessories in normal slots can be hidden, which causes them to not be displayed on the player. However, accessories in social slots cannot, which makes sense since they are intended to alter the player's appearance. However, social slots are also useful for quickly swapping between accessories. In that use case, it is confusing that the accessory not being used is the one displayed.

This mod makes the visibility toggle on each normal accessory slot also control the visibility of the corresponding social slot. Thus you can hide all accessories, social slot or not.

The mod works in multiplayer.

## Development

### Harmony Patches

#### `Terraria.Player.ApplyEquipVanity()`

```cs
private void ApplyEquipVanity(int itemSlot, Item currentItem)
```

This method handles applying some vanity effects of accessories in social slots. The code is not included here because it is not relevant for the patch. Only effects that affect the entire player model, or are external to it are applied in this method. These include the Werewolf and Merman transformations, boot particle effects etc.

The mod applies a prefix patch to this method to conditionally skip it. The method is skipped if the corresponding normal accessory slot is hidden. That information is stored in the `hideVisibleAccessory` array. The normal accessory slots have the IDs 3–9, whereas the social accessory slots have the IDs 13–19. Thus we obtain the visibility of the corresponding normal accessory slot with an offset of -10.

#### `Terraria.Player.UpdateVisibleAccessory()`

```cs
private void UpdateVisibleAccessory(int itemSlot, Item item)
```

This method updates the player model with the effects of the item passed as the parameter. The code is not included here because it is not relevant for the patch.

The mod applies a prefix patch to this method to conditionally skip it. The logic is otherwise the exact same as the other patch, except that because this method handles both normal and social accessory slots, applying the -10 offset blindly would result in trying to access negative indexes for the normal accessory slots, causing an `IndexOutOfRangeException`. This is why an additional condition is applied, which only passes for the slots 13–19.
