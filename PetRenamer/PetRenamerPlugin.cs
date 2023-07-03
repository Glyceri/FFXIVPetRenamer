using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Game;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.PetWindows;

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

            PluginHandlers.CommandManager.AddHandler(PluginConstants.mainCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "Type /petname to open the petname window. \n" +
                "Leave the field blank to add no custom name to your pet. \n" +
                "Enter a name if you DO want a pet name.\n" +
                "You may need to resummon your pet/or look away from it for a moment for the name to update."
            });

            PluginHandlers.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            PluginHandlers.Framework.Update += OnUpdate;
        }

        public void Dispose()
        {
            PluginLink.WindowHandler.RemoveAllWindows();

            PluginHandlers.CommandManager.RemoveHandler(PluginConstants.mainCommand);
        }

       

        private void OnCommand(string command, string args)
        {
            PluginLink.WindowHandler.GetWindow<MainWindow>().IsOpen = true;
        }

        public void DrawConfigUI()
        {
            PluginLink.WindowHandler.GetWindow<ConfigWindow>().IsOpen = true;
        }

        public void OnUpdate(Framework frameWork)
        {
            Globals.CurrentIDChanged = false;
            companionNamer.Update(PluginHandlers.Framework);
        }
    }
}
