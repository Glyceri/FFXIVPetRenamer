using PetRenamer.PetNicknames.Hooking;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update;
using PetRenamer.PetNicknames.Windowing;
using Dalamud.Plugin;
using PetRenamer.PetNicknames.Chat;
using PetRenamer.PetNicknames.Commands;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.TempWindow;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    DalamudServices _DalamudServices { get; init; }
    readonly IPetServices _PetServices;
    readonly IPettableUserList PettableUserList;
    readonly IPettableDatabase PettableDatabase;
    readonly IPettableDatabase LegacyDatabase;
    readonly IWindowHandler WindowHandler;

    // As long as no other module needs one, they won't be interfaced
    readonly UpdateHandler UpdateHandler;
    readonly HookHandler HookHandler;
    readonly ChatHandler ChatHandler;
    readonly CommandHandler CommandHandler;

    public PetRenamerPlugin(IDalamudPluginInterface dalamud)
    {
        _DalamudServices = DalamudServices.Create(ref dalamud)!;
        Translator.Initialise(_DalamudServices);
        _PetServices = new PetServices(_DalamudServices);
        PettableUserList = new PettableUserList();
        PettableDatabase = new PettableDatabase(in _PetServices);
        LegacyDatabase = new LegacyPettableDatabase(_PetServices.Configuration, in _PetServices);
        UpdateHandler = new UpdateHandler(_DalamudServices, PettableUserList, LegacyDatabase, PettableDatabase, _PetServices);
        HookHandler = new HookHandler(_DalamudServices, _PetServices, PettableUserList, PettableDatabase);
        ChatHandler = new ChatHandler(_DalamudServices, _PetServices, PettableUserList);
        CommandHandler = new CommandHandler(_DalamudServices, _PetServices, PettableUserList);
        WindowHandler = new WindowHandler(_DalamudServices, _PetServices, PettableUserList, PettableDatabase);

        WindowHandler.AddWindow(new PetRenameWindow(_DalamudServices, _PetServices, PettableUserList, PettableDatabase));
        WindowHandler.AddWindow(new PetListWindow(_DalamudServices, _PetServices, PettableUserList, PettableDatabase));
        WindowHandler.Open<PetRenameWindow>();
        WindowHandler.Open<PetListWindow>();

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
    }

    public void Dispose()
    {
        UpdateHandler?.Dispose();
        HookHandler?.Dispose();
        ChatHandler?.Dispose();
        CommandHandler?.Dispose();
        WindowHandler?.Dispose();
    }
}
