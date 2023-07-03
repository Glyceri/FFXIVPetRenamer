using Dalamud.Plugin;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    public string Name => PluginConstants.pluginName;
    public bool Debug => true;

    public PetRenamerPlugin(DalamudPluginInterface dalamud)
    {
        PluginHandlers.Start(dalamud);
        PluginLink.Start(dalamud, this);
    }

    public void Dispose()
    {
        PluginLink.WindowHandler.RemoveAllWindows();
        PluginLink.CommandHandler.ClearAllCommands();
        PluginLink.UpdatableHandler.ClearAllUpdatables();
    }  
}
