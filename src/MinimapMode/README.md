# Minimap Mode

Set the minimap to a configured mode when loading into a world.

In vanilla Terraria the minimap style is not persisted between game launches. Having to change the minimap style every time you play the game is annoying.

This mod fixes this issue by allowing you to set a default minimap mode that will be applied when loading into a world.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod works in multiplayer.

## Configuration

### Force Minimap Mode

Force the configured minimap mode when loading into a world.

### Default Minimap Mode

The mode to set the minimap to when loading into a world. Possible values: `Hidden`, `Minimap`, `Overlay`.

## Technical Overview

The current map style is stored in the `Main.mapStyle` property, which does not persist between game launches.

The mod simply sets this property to the configured style in the `OnWorldLoad()` lifecycle method when the option is enabled.
