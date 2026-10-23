# Functional Social Slots

Make accessories and armor in social slots functional.

This mod makes accessories and armor in social slots function just like in normal slots, effectively doubling the accessory capacity and allowing for a second set of armor. The visual effects of both accessories and armor are not affected.

The functionality of social accessory slots and social armor slots can be toggled independently.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod works in multiplayer.

## Configuration

### Functional Social Accessory Slots

Make accessories in social slots function the same as in normal slots.

### Functional Social Armor Slots

Make armor in social slots function the same as in normal slots.

## Technical Overview

### Accessory Effects

The effects of accessories are applied in the `Player.UpdateEquips()` method. This method calls the following three methods for each accessory in normal slots: `Player.GrantPrefixBenefits()`, `Player.GrantArmorBenefits()` and `Player.ApplyEquipFunctional()`. It then additionally applies some effects by itself according to flags set in those methods.

The mod patches the `ApplyEquipFunctional()` method to call those three methods for the accessory in the corresponding social slot. Social slots use IDs 13-19 and normal slots use IDs 3-9, so the corresponding social slot index is found with an offset of 10. The social slot is processed first in order for the normal slot to retain priority in case of overlapping effects, which is particularly important for wings.

### Armor Stat Boosts

The stat boosts of armor (defense and damage) are applied in the method `UpdateEquips()` by the `GrantArmorBenefits()` method.

The mod simply patches the `UpdateEquips()` method to call the `GrantArmorBenefits()` method for the three social armor slots.

### Armor Set Bonuses

The system used to apply armor set bonuses is much more complex. The method `Player.UpdateArmorSets()` first creates a struct `DataStructures.ArmorSetBonus.QueryContext` for the `Player` instance, which contains the item IDs of the player's equipped armor pieces. It then passes that struct to the method `DataStructures.ArmorSetBonuses.GetCompleteSet()`, which returns a `DataStructures.ArmorSetBonus` instance. The `Effect` delegate of that instance is then called, which applies the actual set bonus effects.

The mod patches the `UpdateArmorSets()` method, to run this pipeline for the social slots. The patch first constructs a modified `QueryContext`, where the item IDs of the equipped armor pieces are replaced with the IDs of the armor pieces in the social slots. It then proceeds through the rest of the pipeline, which acts as if those replaced armor pieces were the ones currently equipped in the normal slots. The original method is allowed to run, causing the set bonus from the armor in normal slots to be applied normally.

### Armor Lighting

Light emitted by certain armor pieces is processed in the `Player.UpdateArmorLights()` method. It consists of three switch statements, one for each normal armor slot as encoded by the `Player.head`, `Player.body` and `Player.legs` properties, which contain the item IDs of the equipped armor pieces for those slots.

The mod patches the `UpdateArmorLights()` method to call it again with those properties replaced. The patch first stores the original values of the properties, then overwrites them with the IDs of the armor pieces in the social slots. It then calls the method and reverts the properties to their saved values. The original method is allowed to run, causing the lighting from the armor pieces in normal slots to be applied normally.

### Social Slot Tooltip

The method `Main.MouseText_DrawItemTooltip_GetLinesInfo()` constructs the tooltips of items. If the `social` property of the item is `true`, a line will be added to the tooltip informing that the item has no effect in a social slot. This property is set by the `UI.ItemSlot.MouseHover()` method, if the hover context is either `9` or `11`, corresponding to social armor and accessory slots respectively.

The mod patches the `MouseHover()` method to overwrite the `social` property of the `Main.HoverItem` item instance, if the corresponding feature of the context is enabled. Thus the tooltip is only displayed when accurate.
