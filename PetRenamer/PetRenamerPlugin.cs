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

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    DalamudServices _DalamudServices { get; init; }
    IPetServices _PetServices { get; init; }
    IPettableUserList PettableUserList { get; init; }
    IPettableDatabase PettableDatabase { get; init; }
    IPettableDatabase LegacyDatabase {  get; init; }

    UpdateHandler UpdateHandler { get; init; }
    HookHandler HookHandler { get; init; }
    ChatHandler ChatHandler { get; init; }

    WindowSystem WindowSystem { get; init; }
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

        WindowSystem = new WindowSystem("Pet Nicknames");

        _DalamudServices.PetNicknamesPlugin.UiBuilder.Draw += WindowSystem.Draw;

        window = new TempWindow(PettableUserList, PettableDatabase);

        WindowSystem.AddWindow(window);
        window.IsOpen = true;
    }

    public void Dispose()
    {
        _DalamudServices.PetNicknamesPlugin.UiBuilder.Draw -= WindowSystem.Draw;
        UpdateHandler?.Dispose();
        HookHandler?.Dispose();
        ChatHandler?.Dispose();
    }
}
