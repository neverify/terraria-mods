# Settings Keybind

Add a configurable keybind to toggle the settings menu.

I have always found the absence of a dedicated settings keybind frustrating. Accessing the settings menu from the inventory is slow, and it is thus hard to pause the game quickly.

This mod solves the problem by adding a keybind to toggle the settings menu. The keybind works regardless of if the inventory or a shop is open.

## Configuration

### Hide Settings Button

Hide the vanilla settings button at the bottom right of the inventory.

If you use the keybind to access the menu, the button becomes useless.

The mod works in multiplayer.

## Keybinds

### Toggle Settings

Toggle the settings menu. Default keybind: `Escape`.

## Development

### Harmony Patches

#### `Main.DrawInterface_29_SettingsButton()`

This method handles drawing the settings button when the inventory is open. The implementation of the method is not relevant, so it is not shown here.

The mod applies a prefix patch to this method to conditionally skip it based on the config setting.

### Keybinds

The property `Main.ingameOptionsWindow` determines whether the settings menu is open. The methods `IngameOptions.Open()` and `IngameOptions.Close()` are used to toggle the settings menu when the keybind is pressed.
