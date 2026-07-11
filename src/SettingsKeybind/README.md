# Settings Keybind

Add a configurable keybind to toggle the settings menu.

## Configuration

### Hide Settings Button

Hide the vanilla settings button at the bottom right of the screen.

## Keybinds

### Toggle Settings

Toggle the settings menu. Default keybind: `Escape`.

## Development

### Harmony Patches

#### `Main.DrawInterface_29_SettingsButton()`

This method handles drawing the settings button when the inventory is open. The mod applies a prefix patch onto this method to conditionally skip it based on the configuration setting.

### Keybinds

The property `Main.ingameOptionsWindow` determines whether the settings menu is open. The methods `IngameOptions.Open()` and `IngameOptions.Close()` are used to toggle the settings menu when the keybind is pressed.
