# Minimap Mode

Set the minimap to a configured mode when loading into a world.

## Configuration

### Force Minimap Mode

Force the configured minimap mode when loading into a world.

### Default Minimap Mode

The mode to set the minimap to when loading into a world. Possible values are `Hidden`, `Minimap`, and `Overlay`.

## Development

The mode of the minimap is controlled by the `public` property `Main.mapStyle`. The mod sets this property in the `OnWorldLoad()` lifecycle method.
