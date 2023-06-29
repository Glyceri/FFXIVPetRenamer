using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using PetRenamer.Windows;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game;
using PetRenamer.Core;
using Dalamud.Data;

namespace PetRenamer
{
    public sealed class PetRenamerPlugin : IDalamudPlugin
    {
        public const int ffxivNameSize = 64;

        public string Name =>
#if DEBUG
            "Pet Nicknames [DEBUG]";
#else
            "Pet Nicknames";
#endif

        public bool Debug =>
#if DEBUG
            true;
#else
            false;
#endif

        const string CommandName = "/petname";

        public DalamudPluginInterface PluginInterface { get; init; }
        CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new WindowSystem("Pet Nicknames");
        public Framework framework { get; init; }
        public DataManager dataManager { get; init; }

        Utils utils { get; set; }

        ConfigWindow ConfigWindow { get; init; }
        MainWindow MainWindow { get; init; }
        public CreditsWindow CreditsWindow { get; init; }

        CompanionNamer companionNamer { get; init; }

        public PetRenamerPlugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            [RequiredVersion("1.0")] Framework framework,
            [RequiredVersion("1.0")] SigScanner sigScanner,
            [RequiredVersion("1.0")] DataManager dataManager)
        {


            PluginInterface = pluginInterface;
            CommandManager = commandManager;

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface, this);

            utils = new Utils(this, dataManager);
            companionNamer = new CompanionNamer(this, utils, sigScanner);

            ConfigWindow = new ConfigWindow(this);
            MainWindow = new MainWindow(this, utils);
            CreditsWindow = new CreditsWindow(this);
            
            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);
            WindowSystem.AddWindow(CreditsWindow);

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Type /petname to open the petname window. \n" +
                "Leave the field blank to add no custom name to your pet. \n" +
                "Enter a name if you DO want a pet name.\n" +
                "You may need to resummon your pet/or look away from it for a moment for the name to update."
            });

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            framework.Update += OnUpdate;
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();
            
            MainWindow.Dispose();
            CreditsWindow.Dispose();

            CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }

        public void OnUpdate(Framework frameWork)
        {
            Globals.CurrentIDChanged = false;
            companionNamer.Update(framework);
        }
    }
}
