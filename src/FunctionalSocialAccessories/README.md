# Functional Social Accessories

Make accessories in social slots functional.

This mod provides a simple way to increase the amount of accessories available. It makes accessories in social slots function the same as in normal slots.

The vanity effects of accessories in social slots are unchanged. They are also always visible on the character, like in vanilla.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod works in multiplayer.

## Configuration

### Functional Social Slots

Make accessories in social slots function the same as in normal slots.

## Technical Overview

### Accessory Effects

The effects of accessories are applied in the `Player.UpdateEquips()` method. This method calls the following three methods for each accessory in normal slots: `Player.GrantPrefixBenefits()`, `Player.GrantArmorBenefits()` and `ApplyEquipFunctional()`. It then additionally applies some effects by itself according to flags set in those methods.

The mod patches `ApplyEquipFunctional()` to call those three methods for the accessory in the corresponding social slot. Social slots use IDs 13-19 and normal slots use IDs 3-9, so the corresponding social slot index is found with an offset of 10. The social slot is processed first in order for the normal slot to retain priority in case of overlapping effects, which is particularly important for wings.

### Social Slot Tooltip

The vanilla method `Main.MouseText_DrawItemTooltip_GetLinesInfo()` constructs the tooltips of items. If the `social` property of the item is `true`, a line will be added to the tooltip informing that the item has no effect in a social slot.

The mod sets this property to `false` for the duration of the method. This prevents the inaccurate warning from being shown.
