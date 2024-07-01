using Dalamud.Plugin;
using PetRenamer.Core.Handlers;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    public PetRenamerPlugin(IDalamudPluginInterface dalamud) => StartHandler.Start(ref dalamud, this);
    public void Dispose() => QuitHandler.Quit();
}
