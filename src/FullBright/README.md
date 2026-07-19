# Fullbright

Force all tiles to render at a configured brightness.

## Configuration

### Brightness Override

Enable the brightness override. This only works with the "color" lighting mode.

### Brightness

The brightness at which to render tiles at. Decimal number between `0` and `1` (e.g. 0.1 = 10%).

### Disable Light Smoothing

Disable vanilla light smoothing.

Recommended with brightness override to improve performance. This option has no visual impact with brightness override on, since all tiles are the same brightness.

### Map Lighting Override

Override the brightness of map lighting.

### Map Lighting Brightness

The brightness to set map tiles to. Decimal number between `0` and `1` (e.g. 0.1 = 10%).

## Development

### Harmony Patches

This mod implements two harmony patches.

#### `Graphics.Lighting.LightingEngine.GetColor()`

```cs
public Vector3 GetColor(int x, int y)
{
    if (!this._activeProcessedArea.Contains(x, y))
    {
        return Vector3.Zero;
    }
    x -= this._activeProcessedArea.X;
    y -= this._activeProcessedArea.Y;
    return this._activeLightMap[x, y];
}
```

This method is used to query the color of each tile.

The mod applies a prefix patch onto that method to skip it and instead return the configured brightness value.

However, as you can see from the method, there is a check: `!this._activeProcessedArea.Contains(x, y)`. This check is absolutely crucial, since this method is queried for tiles outside the rendered area. Bypassing this check would return a non-zero brightness value for those tiles as well, which causes the game to render a massive amount of off-screen tiles, causing a noticeable performance hit.

Because of this, the prefix also runs this check.

#### `Lighting.LightingEngine.ProcessScan()`

```cs
private void ProcessScan(Rectangle area)
{
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

This method updates the `private` `_workingProcessedArea` field, which is used in the `GetColor()` method to determine which tiles to color black (skip rendering). However, since the field is `private`, it would need to be accessed with reflection in the `GetColor()` prefix. Since `GetColor()` is called _tens of thousands_ of times per frame, this approach is, lets say, not ideal for performance.

Instead a postfix patch is applied for this method, which uses reflection to obtain the updated value once, caching it for the `GetColor()` prefix.

Why not prefix the method and cache the `area` argument directly? This is simply to prevent the mod breaking for future versions. If the logic for determining the area ever changes, it won't affect the mod. One reflection per 3 frames (the update interval) has effectively no performance impact.

#### `Map.WorldMap.UpdateLighting()`

```cs
public bool UpdateLighting(int x, int y, byte light)
{
    MapTile mapTile = this._tiles[x, y];
    if (light == 0 && mapTile.Light == 0)
    {
        return false;
    }
    MapTile mapTile2 = MapHelper.CreateMapTile(x, y, Math.Max(mapTile.Light, light), 0);
    if (mapTile2.Equals(mapTile))
    {
        return false;
    }
    this._tiles[x, y] = mapTile2;
    return true;
}
```

This method updates the lighting of one tile on the map. A simple prefix overriding the `light` parameter would allow making the map brighter, but not dimmer. This is because of the statement `Math.Max(mapTile.Light, light)`, which disallows the lighting becoming dimmer. To get around this, we replace the method entirely instead. Since the `_tiles` field is `private`, we use the handy `public` setter method `SetTile()` instead of directly accessing the field.

### Performance Improvements

With these patches alone, the performance is still not ideal. In fact, this has nothing to do with the mod!

You can try this in vanilla Terraria by filling an area populated with blocks slightly larger than the minimum zoom (≈120x60 tiles), specifically 28 tiles in each direction (≈176x116), with gemspark walls and standing in the middle. Gemspark walls cause each tile to be rendered at full brightness, which simulates the mod.

As you will notice, performance is the same as with the mod! Terraria's rendering engine simply isn't built to render so many tiles at once. And notice that I said the rendering engine, not the lighting engine. That's right, the lighting engine is actually not the problem here, it is actually just the rendering of too many tiles. Let's see why.

`GameContent.Drawing.TileDrawing.Draw()`

```cs
public void Draw(bool solidLayer, bool intoRenderTargets, int waterStyleOverride = -1)
{
    // ...

    float num = 255f * (1f - Main.gfxQuality) + 30f * Main.gfxQuality;
    this._highQualityLightingRequirement.R = (byte)num;
    this._highQualityLightingRequirement.G = (byte)((double)num * 1.1);
    this._highQualityLightingRequirement.B = (byte)((double)num * 1.2);
    float num2 = 50f * (1f - Main.gfxQuality) + 2f * Main.gfxQuality;
    this._mediumQualityLightingRequirement.R = (byte)num2;
    this._mediumQualityLightingRequirement.G = (byte)((double)num2 * 1.1);
    this._mediumQualityLightingRequirement.B = (byte)((double)num2 * 1.2);
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

In this method, there is a section which calculates two values, `_highQualityLightingRequirement` and `_mediumQualityLightingRequirement`. These values are calculated using the `Main.gfxQuality` property, and encode the required per-channel brightness for "medium quality lighting" and "high quality lighting". Note that the higher the `Main.gfxQuality` value, the lower the floor for all of these requirements.

Well, where are these requirements used?

`GameContent.Drawing.TileDrawing.DrawSingleTile_SlicedBlock()`

```cs
private void DrawSingleTile_SlicedBlock(Vector2 normalTilePosition, int tileX, int tileY, TileDrawInfo drawData)
{
    // ...

    if (drawData.tileLight.R > this._highQualityLightingRequirement.R || drawData.tileLight.G > this._highQualityLightingRequirement.G || drawData.tileLight.B > this._highQualityLightingRequirement.B) {
        // ...

        Main.tileBatch.Draw(...)
    }

    if (drawData.tileLight.R > this._mediumQualityLightingRequirement.R || drawData.tileLight.G > this._mediumQualityLightingRequirement.G || drawData.tileLight.B > this._mediumQualityLightingRequirement.B) {
        // ...

        Main.tileBatch.Draw(...)
    }

    Main.tileBatch.Draw(...);
}
```

Despite its somewhat non-generic name, this method is responsible for drawing every tile in the game. And as a major feature, there are three different paths for rendering tiles based on the aformentioned lighting requirements. The bodies of these paths are too long (and honestly useless) to include here, but I'll explain what they do.

Both of the paths after the conditions apply _light smoothing_ for each tile, based on the lighting of the surrounding tiles. Basically, they do a whole bunch of fairly expensive math. This means that tiles that pass the thresholds in terms of lighting take much longer to render.

So, what does this all mean for the mod? Since the mod makes all tiles render at the same brightness, light smoothing is effectively wasted computation time. So how can we disable it?

Looking back at the `GameContent.Drawing.TileDrawing.Draw()` method, we can see the answer right in front of our eyes. There is a condition that checks for the property `DebugOptions.devLightTilesCheat`. If the property is `true`, the lighting requirements will be set to a value so high that the smoothing paths will never be excecuted.

By setting this (nicely enough, `public`) property to `true`, we can effectively disable light smoothing entirely! This causes a high enough boost in performance that the game manages 60fps at all times (depending on hardware though, of course). This is done in the `OnGameReady()` lifecycle method and on-demand when the config option is changed. This option is not saved between game restarts, so there is no need to reset it when unloading.

### Footnote

There is much more optimization to be had in the lighting engine. Fundamentally a fullbright mod tries to "disable" the lighting engine, so almost all methods could just be skipped. This mod is built around the modern "color" lighting engine, which is much more complex than the legacy lighting engine (used in "white", "retro" and "trippy" lighting modes). In hindsight, this mod should definitely have been implemented for the legacy engine, as it is generally much less demanding. But as you might guess, I did not know any of these things before having a working implementation.

And as for the `gfxQuality` variable, as far as I know that corresponds to the extremely vague "quality" option in the game settings. While I have not confirmed this, having a lower setting there probably mitigates some of the performance losses (while obviously affecting the visual quality).

The logic behind calculating the thresholds is actually fairly straightforward. Light smoothing has a less pronounced effect on dimmer tiles, so to save computation time, they are smoothed at a lower resolution.

And finally, why does the `devLightTilesCheat` property even exist? Without it the mod would have to use reflection to change these values. My guess is that it has been used to override the quality option in development to tune the smoothing. Either way, its existence is a godsend for this specific purpose :D
