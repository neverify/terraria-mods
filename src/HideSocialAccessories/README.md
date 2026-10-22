# Hide Social Accessories

Hide accessories in social slots on the player.

Accessories in normal slots can be hidden, which causes them to not be displayed on the player. However, accessories in social slots cannot, which makes sense since they are intended to alter the player's appearance. However, social slots are also useful for quickly swapping between accessories. In that use case, it is confusing that the accessory not being used is the one displayed.

This mod makes the visibility toggle on each normal accessory slot also control the visibility of the corresponding social slot. Thus you can hide all accessories, social slot or not.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod works in multiplayer.

## Technical Overview

### Vanity Effects

Some visual effects of accessories are applied with the `Player.ApplyEquipVanity()` method. These include the Werewolf and Merman transformations, boot effects, etc.

The mod skips `ApplyEquipVanity()` for social slots when the corresponding normal slot has been hidden. The visibility of normal accessory slots is controlled by the `hideVisibleAccessory` array. Social slots use IDs 13-19 and normal slots use IDs 3-9, so the corresponding normal slot index is found with an offset of -10.

### Player Model Changes

The method `Player.UpdateVisibleAccessory()` applies the player model changes of an accessory.

The mod applies the same visibility check here, but only for social slots. This avoids applying the offset to normal slots, which would produce invalid indexes.
