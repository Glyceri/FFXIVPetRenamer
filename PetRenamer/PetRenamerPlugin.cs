using PetRenamer.PetNicknames.Hooking;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update;
using PetRenamer.PetNicknames.Windowing;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using PetRenamer.PetNicknames.Chat;
using PetRenamer.PetNicknames.Commands;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    DalamudServices _DalamudServices { get; init; }
    readonly IPetServices _PetServices;
    readonly IPettableUserList PettableUserList;
    readonly IPettableDatabase PettableDatabase;
    readonly IPettableDatabase LegacyDatabase;

    // As long as no other module needs one, they won't be interfaced
    readonly UpdateHandler UpdateHandler;
    readonly HookHandler HookHandler;
    readonly ChatHandler ChatHandler;
    readonly CommandHandler CommandHandler;

    // VVVVVVVVVVVVVVVVVVVVVVVVVVVVVV EXTREMLEY TEMPORARY! VVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
    WindowSystem WindowSystem;
    TempWindow window;

    public PetRenamerPlugin(IDalamudPluginInterface dalamud)
    {
        _DalamudServices = DalamudServices.Create(ref dalamud)!;
        _PetServices = new PetServices(_DalamudServices);
        PettableUserList = new PettableUserList();
        PettableDatabase = new PettableDatabase(_PetServices.PetLog);
        LegacyDatabase = new LegacyPettableDatabase(_PetServices.Configuration, _PetServices.PetLog);
        UpdateHandler = new UpdateHandler(_DalamudServices, PettableUserList, LegacyDatabase, PettableDatabase, _PetServices);
        HookHandler = new HookHandler(_DalamudServices, _PetServices, PettableUserList, PettableDatabase);
        ChatHandler = new ChatHandler(_DalamudServices, _PetServices, PettableUserList);
        CommandHandler = new CommandHandler(_DalamudServices, _PetServices, PettableUserList);

        WindowSystem = new WindowSystem("Pet Nicknames");

        _DalamudServices.PetNicknamesPlugin.UiBuilder.Draw += WindowSystem.Draw;

        window = new TempWindow(_DalamudServices, PettableUserList, PettableDatabase);

        WindowSystem.AddWindow(window);
        window.IsOpen = true;
    }

    public void Dispose()
    {
        _DalamudServices.PetNicknamesPlugin.UiBuilder.Draw -= WindowSystem.Draw;
        UpdateHandler?.Dispose();
        HookHandler?.Dispose();
        ChatHandler?.Dispose();
        CommandHandler?.Dispose();
    }
}
