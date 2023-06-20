using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using PetRenamer.Windows;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game;
using PetRenamer.Core;

namespace PetRenamer
{
    public sealed class PetRenamerPlugin : IDalamudPlugin
    {
        public const int ffxivNameSize = 64;

        public string Name => "Pet Nicknames";
        private const string CommandName = "/petname";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("Pet Nicknames");
        public Framework framework { get; init; }
        public SigScanner sigScanner { get; init; }

        private ConfigWindow ConfigWindow { get; init; }
        private MainWindow MainWindow { get; init; }


        public byte[] petName = new byte[PetRenamerPlugin.ffxivNameSize];

        Test test { get; init; }

        public PetRenamerPlugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            [RequiredVersion("1.0")] Framework framework,
            [RequiredVersion("1.0")] SigScanner sigScanner)
        {

            test = new Test(this, sigScanner);
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface, this);


            // you might normally want to embed resources and load them from the manifest stream

            ConfigWindow = new ConfigWindow(this);
            MainWindow = new MainWindow(this);
            
            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Type /petname to open the petname window. \n" +
                "Leave the field blank to add no custom name to your pet. \n" +
                "Enter a name if you DO want a pet name.\n" +
                "You may need to resummon your pet for the name to update."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            framework.Update += OnUpdate;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();

            //framework.Update -= OnUpdate;
            
            ConfigWindow.Dispose();
            MainWindow.Dispose();

            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }

        public void OnUpdate(Framework frameWork)
        {
            test.Update(framework);
        }
    }
}
