using FullBright.CustomLighting;
using Utils;

namespace FullBright;

public class Mod : ModBase<Mod, Config>
{
    protected override void OnInitialize()
    {
        Keybinds.Register();
        LightSmoothing.Update();
    }

    public void OnConfigChanged()
    {
        LightSmoothing.Update();
        LightingController.Refresh();
    }
}
