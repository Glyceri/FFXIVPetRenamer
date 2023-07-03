using Dalamud.Plugin;
using Dalamud.Game;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;

namespace PetRenamer
{
    public sealed class PetRenamerPlugin : IDalamudPlugin
    {
        public string Name => PluginConstants.pluginName;
        public bool Debug => true;

        CompanionNamer companionNamer { get; init; }

        public PetRenamerPlugin(DalamudPluginInterface dalamud)
        {
            PluginHandlers.Start(dalamud);
            PluginLink.Start(dalamud, this);

            companionNamer = new CompanionNamer();

            PluginHandlers.Framework.Update += OnUpdate;
        }

        public void Dispose()
        {
            PluginLink.WindowHandler.RemoveAllWindows();
            PluginLink.CommandHandler.ClearAllCommands();
        }  

        public void OnUpdate(Framework frameWork)
        {
            Globals.CurrentIDChanged = false;
            companionNamer.Update(PluginHandlers.Framework);
        }
    }
}
