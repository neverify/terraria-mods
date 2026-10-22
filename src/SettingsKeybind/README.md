# Settings Keybind

Add a configurable keybind to toggle the settings menu.

I have always found the absence of a dedicated settings keybind frustrating. Accessing the settings menu from the inventory is slow, and it is thus hard to pause the game quickly.

This mod solves the problem by adding a keybind to toggle the settings menu. The keybind works regardless of if the inventory or a shop is open.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod works in multiplayer.

## Configuration

### Hide Settings Button

Hide the vanilla settings button at the bottom right of the inventory.

If you use the keybind to access the menu, the button becomes useless.

## Keybinds

### Toggle Settings

Toggle the settings menu. Default: `Escape`.

## Technical Overview

### Settings Button

The inventory settings button is drawn by the method `Main.DrawInterface_29_SettingsButton()`.

The mod simply conditionally skips this method according to the config.

### Settings Keybind

The state of the settings menu is stored in the property `Main.ingameOptionsWindow`, and it is opened and closed with the `IngameOptions.Open()` and `IngameOptions.Close()` methods.

The mod uses those methods to toggle the menu when the configured keybind is pressed.
