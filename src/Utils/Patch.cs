namespace Utils;

public abstract class Patch<TMod>
    where TMod : ModBase<TMod>
{
    protected static bool Prepare() => ModBase<TMod>.Instance != null;
}
