# Minimap Mode

Set the minimap to a configured mode when loading into a world.

In vanilla Terraria the minimap style is not persisted between game launches. Having to change the minimap style every time you play the game is annoying.

This mod fixes this issue by allowing you to set a default minimap mode that will be applied when loading into a world.

The mod works in multiplayer.

## Configuration

### Force Minimap Mode

Force the configured minimap mode when loading into a world.

### Default Minimap Mode

The mode to set the minimap to when loading into a world. Possible values: `Hidden`, `Minimap`, and `Overlay`.

## Development

The mode of the minimap is controlled by the `public` property `Main.mapStyle`. The mod sets this property in the `OnWorldLoad()` lifecycle method.
