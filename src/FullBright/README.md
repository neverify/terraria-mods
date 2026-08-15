# Fullbright

Force all tiles to render at a configured brightness.

This mod is a performant implementation of fullbright. The vanilla rendering engine can't normally handle rendering all the tiles at full brightness at 60fps. This is why most naive fullbright implementations lag the game considerably. This mod implements performance optimizations to minimize the impact of rendering more tiles than intended. You should be able to retain 60fps even on weaker hardware.

The mod also allows overriding the brightness of the map, so it will actually reflect what you see in-game.

The mod modifies the lighting engine of the "Color" lighting mode, so other lighting modes are unaffected.

The mod should work in multiplayer.

## Configuration

### Brightness Override

Enable the brightness override. Only works with the "color" lighting mode.

### Brightness

The brightness at which to render tiles at. Decimal number between `0` and `1` (e.g. `0.1` = 10%).

### Disable Light Smoothing

Disable vanilla light smoothing.

Recommended with brightness override to improve performance. This option has no visual impact with brightness override on, since all tiles are the same brightness.

### Map Lighting Override

Override the brightness of map lighting.

### Map Lighting Brightness

The brightness to set map tiles to. Decimal number between `0` and `1` (e.g. 0.1 = 10%).

## Development

### Harmony Patches

#### `Graphics.Lighting.LightingEngine.GetColor()`

```cs
public Vector3 GetColor(int x, int y)
{
    // Check if the tile is visible.
    if (!this._activeProcessedArea.Contains(x, y))
    {
        return Vector3.Zero;
    }
    x -= this._activeProcessedArea.X;
    y -= this._activeProcessedArea.Y;
    return this._activeLightMap[x, y];
}
```

This method is used to query the color of tiles. It fetches the tile in question from the `_activeLightMap` field, which contains the lighting information of each tile. It disqualifies tiles outside the `_activeProcessedArea` rectangle by returning a zero vector, which represents a completely black tile. Since the rendering engine renders any tile that is not completely black, this method has double duty by also defining the rendering area.

The mod applies a prefix patch to this method to replace it. The patch skips the `_activeLightMap` field entirely, and just returns a color according to the configured brightness. Since it is not possible to extract the information of whether the tile is visible from the result of the original method (unlit tiles result in the same value), the patch also runs the check. If all tiles processed by this method were given a non-zero brightness, the game would have to render many more tiles than necessary, causing a noticeable performance hit.

However, the `_activeProcessedArea` field is private and cannot thus be accessed directly without reflection. Since `GetColor()` is called _tens of thousands_ of times per frame, this approach is, lets say, not ideal for performance. Therefore the field has to be cached.

#### `Lighting.LightingEngine.ProcessScan()`

```cs
private void ProcessScan(Rectangle area)
{
    // Add a padding around the viewbox.
    area.Inflate(28, 28);
    this._workingProcessedArea = area;

    this._workingLightMap.SetSize(area.Width, area.Height);
    this._workingLightMap.NonVisiblePadding = 18;

    this._tileScanner.Update();
    this._tileScanner.ExportTo(area, this._workingLightMap, new TileLightScannerOptions
    {
        DrawInvisibleWalls = Main.ShouldShowInvisibleBlocksAndWalls()
    });
}
```

This method updates the `_workingProcessedArea` field.

The mod applies a postfix patch to the this method to cache the updated value for use in the `GetColor()` prefix.

Why not prefix the method and cache the `area` argument directly? This is simply to prevent the mod breaking for future versions. If the logic for determining the area ever changes, it won't affect the mod. One reflection per 3 frames (the update interval) has effectively no performance impact.

#### `Map.WorldMap.UpdateLighting()`

```cs
public bool UpdateLighting(int x, int y, byte light)
{
    // Get the map tile.
    MapTile mapTile = this._tiles[x, y];

    // Don't update unlit tiles.
    if (light == 0 && mapTile.Light == 0)
    {
        return false;
    }

    // Don't lower the brightness of the tile.
    MapTile mapTile2 = MapHelper.CreateMapTile(x, y, Math.Max(mapTile.Light, light), 0);
    if (mapTile2.Equals(mapTile))
    {
        return false;
    }

    // Update the tile.
    this._tiles[x, y] = mapTile2;
    return true;
}
```

This method updates the lighting of one tile on the map. A simple prefix overriding the `light` parameter would allow making the map brighter, but not dimmer. This is because of the statement `Math.Max(mapTile.Light, light)`, which disallows the brightness of a tile from being reduced. To get around this, we replace the method entirely instead. Since the `_tiles` field is `private`, we use the handy `public` setter method `SetTile()` instead of directly accessing the field.

### Performance Improvements

With these patches alone, the performance is still not ideal. In fact, this has nothing to do with the mod!

You can try this in vanilla Terraria by filling an area populated with blocks slightly larger than the minimum zoom (≈120x60 tiles), specifically 28 tiles in each direction (≈176x116), with gemspark walls and standing in the middle. Gemspark walls cause each tile to be rendered at full brightness, which simulates the mod.

As you will notice, performance is the same as with the mod without any optimizations! Terraria's rendering engine simply isn't built to render so many tiles at once. And notice that I said the rendering engine, not the lighting engine. That's right, the lighting engine is actually not the problem here, it is actually just the rendering of too many tiles. Let's see why.

`GameContent.Drawing.TileDrawing.Draw()`

```cs
public void Draw(bool solidLayer, bool intoRenderTargets, int waterStyleOverride = -1)
{
    // ...

    // Determine the brightness threshold for high quality lighting.
    float num = 255f * (1f - Main.gfxQuality) + 30f * Main.gfxQuality;
    this._highQualityLightingRequirement.R = (byte)num;
    this._highQualityLightingRequirement.G = (byte)((double)num * 1.1);
    this._highQualityLightingRequirement.B = (byte)((double)num * 1.2);

    // Determine the brightness threshold for medium quality lighting.
    float num2 = 50f * (1f - Main.gfxQuality) + 2f * Main.gfxQuality;
    this._mediumQualityLightingRequirement.R = (byte)num2;
    this._mediumQualityLightingRequirement.G = (byte)((double)num2 * 1.1);
    this._mediumQualityLightingRequirement.B = (byte)((double)num2 * 1.2);

    // Make the requirements impossible to reach.
    if (DebugOptions.devLightTilesCheat)
    {
        this._highQualityLightingRequirement.R = byte.MaxValue;
        this._highQualityLightingRequirement.G = byte.MaxValue;
        this._highQualityLightingRequirement.B = byte.MaxValue;
        this._mediumQualityLightingRequirement.R = byte.MaxValue;
        this._mediumQualityLightingRequirement.G = byte.MaxValue;
        this._mediumQualityLightingRequirement.B = byte.MaxValue;
    }

    // ...
}
```

In this method, there is a section which calculates two values, `_highQualityLightingRequirement` and `_mediumQualityLightingRequirement`. These values are calculated using the `Main.gfxQuality` property, and encode the required per-channel brightness for "high quality lighting" and "medium quality lighting". Note that the higher the `Main.gfxQuality` value, the lower the floor for all of these requirements.

Well, where are these requirements used?

`GameContent.Drawing.TileDrawing.DrawSingleTile_SlicedBlock()`

```cs
private void DrawSingleTile_SlicedBlock(Vector2 normalTilePosition, int tileX, int tileY, TileDrawInfo drawData)
{
    // ...

    // Check if the tile qualifies for high-quality lighting.
    if (drawData.tileLight.R > this._highQualityLightingRequirement.R || drawData.tileLight.G > this._highQualityLightingRequirement.G || drawData.tileLight.B > this._highQualityLightingRequirement.B) {
        // ...

        Main.tileBatch.Draw(...)
    }

    // Check if the tile qualifies for medium-quality lighting.
    if (drawData.tileLight.R > this._mediumQualityLightingRequirement.R || drawData.tileLight.G > this._mediumQualityLightingRequirement.G || drawData.tileLight.B > this._mediumQualityLightingRequirement.B) {
        // ...

        Main.tileBatch.Draw(...)
    }

    // Otherwise just draw the tile.
    Main.tileBatch.Draw(...);
}
```

Despite its somewhat non-generic name, this method is responsible for drawing every tile in the game. And as a major feature, there are three different paths for rendering tiles based on the aformentioned lighting requirements. The bodies of these paths are too long and irrelevant to include here, but I'll explain what they do.

Both of the paths after the conditions apply _light smoothing_ for each tile, based on the lighting of the surrounding tiles. Basically, they do a whole bunch of fairly expensive math. This means that tiles that pass the thresholds in terms of lighting take much longer to render.

So, what does this all mean for the mod? Since the mod makes all tiles render at the same brightness, light smoothing is effectively wasted computation time. So how can we disable it?

Looking back at the `GameContent.Drawing.TileDrawing.Draw()` method, we can see the answer right in front of our eyes. There is a condition that checks for the property `DebugOptions.devLightTilesCheat`. If the property is `true`, the lighting requirements will be set to a value so high that the smoothing paths will never be excecuted.

By setting this (nicely enough, `public`) property to `true`, we can effectively disable light smoothing entirely! This causes a high enough boost in performance that the game manages 60fps at all times (depending on hardware though, of course). This is done in the `OnGameReady()` lifecycle method and on-demand when the config option is changed. This option is not persisted between game restarts, so there is no need to reset it when unloading.

### Footnote

There is much more optimization to be had in the lighting engine. Fundamentally a fullbright mod tries to "disable" the lighting engine, so almost all methods could just be skipped. This mod is built around the modern "color" lighting engine, which is much more complex than the legacy lighting engine (used in "white", "retro" and "trippy" lighting modes). In hindsight, this mod should definitely have been implemented for the legacy engine, as it is generally much less demanding. But as you might guess, I did not know any of these things before having a working implementation.

And as for the `gfxQuality` variable, as far as I know that corresponds to the extremely vague "quality" option in the game settings. While I have not confirmed this, having a lower setting there probably mitigates some of the performance losses (while obviously affecting the visual quality).

The logic behind calculating the thresholds is actually fairly straightforward. Light smoothing has a less pronounced effect on dimmer tiles, so to save computation time, they are smoothed at a lower resolution.

And finally, why does the `devLightTilesCheat` property even exist? Without it the mod would have to use reflection to change these values. My guess is that it has been used to override the quality option in development to tune the smoothing. Either way, its existence is a godsend for this specific purpose :​D
