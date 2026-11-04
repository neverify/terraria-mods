using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Light;

namespace FullBright.CustomLighting;

internal sealed class FullBrightLightingEngine : ILightingEngine
{
    private Rectangle _area;

    public static FullBrightLightingEngine Instance { get; } = new FullBrightLightingEngine();

    public void Rebuild() { }

    public void AddLight(int x, int y, Vector3 color) { }

    public void ProcessArea(Rectangle area)
    {
        _area = area;

        int offset = Mod.Instance.Config.LightingPadding;
        _area.Inflate(offset, offset);
    }

    public Vector3 GetColor(int x, int y) =>
        _area.Contains(x, y) ? new Vector3(Mod.Instance.Config.Brightness) : Vector3.Zero;

    public void UpdateMinimap()
    {
        var area = Rectangle.Intersect(_area, new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY));

        for (int x = area.Left; x < area.Right; x++)
        {
            for (int y = area.Top; y < area.Bottom; y++)
            {
                _ = Main.Map.UpdateLighting(x, y, byte.MaxValue);
            }
        }

        Main.updateMap = area;
    }

    public void Clear() { }
}
