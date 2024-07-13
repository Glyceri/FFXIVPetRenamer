using Dalamud.Plugin;
using PetRenamer.PetNicknames.Hooking;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update;
using PetRenamer.PetNicknames.Windowing;
using PetRenamer.PetNicknames.Chat;
using PetRenamer.PetNicknames.Commands;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.TempWindow;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.ImageDatabase;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.Parsing;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing;
using PetRenamer.PetNicknames.Windowing.Windows.PetDev;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    readonly DalamudServices _DalamudServices;
    readonly IPetServices _PetServices;
    readonly IPettableUserList PettableUserList;
    readonly IPettableDatabase PettableDatabase;
    readonly ILegacyDatabase LegacyDatabase;
    readonly IImageDatabase ImageDatabase;
    readonly IWindowHandler WindowHandler;
    readonly IDataParser DataParser;
    readonly IDataWriter DataWriter;
    readonly IpcProvider IpcProvider;

    // As long as no other module needs one, they won't be interfaced
    readonly UpdateHandler UpdateHandler;
    readonly HookHandler HookHandler;
    readonly ChatHandler ChatHandler;
    readonly CommandHandler CommandHandler;
    readonly LodestoneNetworker LodestoneNetworker;
    readonly ILodestoneNetworker LodestoneNetworkerInterface;


    public PetRenamerPlugin(IDalamudPluginInterface dalamud)
    {
        _DalamudServices = DalamudServices.Create(ref dalamud)!;
        _PetServices = new PetServices(_DalamudServices);
        Translator.Initialise(_DalamudServices);

        LodestoneNetworkerInterface = LodestoneNetworker = new LodestoneNetworker();

        PettableUserList = new PettableUserList();

        PettableDatabase = new PettableDatabase(in _PetServices);

        LegacyDatabase = new LegacyPettableDatabase(in _PetServices);
        ImageDatabase = new ImageDatabase(in _DalamudServices, in _PetServices, in LodestoneNetworkerInterface);

        DataWriter = new DataWriter(in PettableUserList);
        DataParser = new DataParser(in _DalamudServices, in PettableUserList, in PettableDatabase, in LegacyDatabase);
        IpcProvider = new IpcProvider(in _PetServices, _DalamudServices.PetNicknamesPlugin, in DataParser, in DataWriter);

        UpdateHandler = new UpdateHandler(in _DalamudServices, _PetServices.Configuration, in PettableUserList, LegacyDatabase, in PettableDatabase, in _PetServices, in LodestoneNetworker, IpcProvider);
        HookHandler = new HookHandler(in _DalamudServices, in _PetServices, in PettableUserList);
        ChatHandler = new ChatHandler(_DalamudServices, _PetServices, PettableUserList);
        CommandHandler = new CommandHandler(_DalamudServices, _PetServices, PettableUserList);
        WindowHandler = new WindowHandler(in _DalamudServices, in PettableDatabase);


        WindowHandler.AddWindow(new PetRenameWindow(_DalamudServices, _PetServices, PettableUserList));
        WindowHandler.AddWindow(new PetListWindow(_DalamudServices, _PetServices, PettableUserList, PettableDatabase, LegacyDatabase, ImageDatabase));
        WindowHandler.AddWindow(new PetDevWindow(_DalamudServices,  IpcProvider));
        //WindowHandler.AddWindow(new EmptyWindow(_DalamudServices, _PetServices, PettableUserList, PettableDatabase, LegacyDatabase, ImageDatabase));
        WindowHandler.Open<PetListWindow>();
        //WindowHandler.Open<PetRenameWindow>();
        //WindowHandler.Open<EmptyWindow>();
        //WindowHandler.Open<PetDevWindow>();

        _DalamudServices.CommandManager.AddHandler("/petname", new Dalamud.Game.Command.CommandInfo((s, ss) => WindowHandler.Open<PetRenameWindow>())
        {
            HelpMessage = "Temporary",
            ShowInHelp = true
        });

        _DalamudServices.CommandManager.AddHandler("/petlist", new Dalamud.Game.Command.CommandInfo((s, ss) => WindowHandler.Open<PetListWindow>())
        {
            HelpMessage = "Temporary",
            ShowInHelp = true
        });

        _DalamudServices.CommandManager.AddHandler("/petdev", new Dalamud.Game.Command.CommandInfo((s, ss) => WindowHandler.Open<PetDevWindow>())
        {
            HelpMessage = "Temporary",
            ShowInHelp = true
        });

        _PetServices.Configuration.Initialise(_DalamudServices.PetNicknamesPlugin, PettableDatabase, LegacyDatabase);
        IpcProvider.Prepare();
    }

    public void Dispose()
    {
        IpcProvider?.Dispose();
        LodestoneNetworker?.Dispose();
        ImageDatabase?.Dispose();
        UpdateHandler?.Dispose();
        HookHandler?.Dispose();
        ChatHandler?.Dispose();
        CommandHandler?.Dispose();
        WindowHandler?.Dispose();
    }
}
