using Dalamud.Plugin;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    public string Name => PluginConstants.pluginName;
    public PetRenamerPlugin(DalamudPluginInterface dalamud) => new StartHandler().Start(ref dalamud, this);
    public void Dispose() => PluginLink.QuitHandler.Quit();
}
