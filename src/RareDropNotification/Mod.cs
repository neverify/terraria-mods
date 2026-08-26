using TerrariaModder.Core;
using Utils;

namespace RareDropNotification;

public class Mod : ModBase<Mod, Config>, IMod
{
    public override string Id => "rare-drop-notification";
    public override string Name => "Rare Drop Notification";
    public override string Version => "1.0.3";

    public void OnConfigChanged() { }
}
