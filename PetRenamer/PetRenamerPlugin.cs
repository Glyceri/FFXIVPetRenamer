using Dalamud.Logging;
using Dalamud.Plugin;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    public string Name => PluginConstants.pluginName;
    public PetRenamerPlugin(DalamudPluginInterface dalamud) 
    {
        PluginLog.Log($"{Name} is Starting.");
        new StartHandler().Start(ref dalamud, this); 
    }
    public void Dispose() 
    { 
        PluginLog.Log($"{Name} is Quitting."); 
        PluginLink.QuitHandler.Quit(); 
    }
}
