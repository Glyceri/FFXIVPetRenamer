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
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.ImageDatabase;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.Parsing;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing;
using PetRenamer.PetNicknames.ContextMenus;
using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.IPC;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    readonly DalamudServices _DalamudServices;
    readonly IPetServices _PetServices;
    readonly ISharingDictionary SharingDictionary;
    readonly IPettableUserList PettableUserList;
    readonly IPettableDatabase PettableDatabase;
    readonly ILegacyDatabase LegacyDatabase;
    readonly IImageDatabase ImageDatabase;
    readonly IWindowHandler WindowHandler;
    readonly IDataParser DataParser;
    readonly IDataWriter DataWriter;
    readonly IpcProvider IpcProvider;

    // As long as no other module needs one, they won't be interfaced
    readonly ContextMenuHandler ContextMenuHandler;
    readonly UpdateHandler UpdateHandler;
    readonly HookHandler HookHandler;
    readonly ChatHandler ChatHandler;
    readonly CommandHandler CommandHandler;
    readonly LodestoneNetworker LodestoneNetworker;
    readonly ILodestoneNetworker LodestoneNetworkerInterface;

    readonly PettableDirtyHandler DirtyHandler;

    readonly SaveHandler SaveHandler;

    public PetRenamerPlugin(IDalamudPluginInterface dalamud)
    {
        _DalamudServices = DalamudServices.Create(dalamud)!;
        _PetServices = new PetServices(_DalamudServices);

        SharingDictionary = new SharingDictionary(_DalamudServices);

        Translator.Initialise(_DalamudServices, _PetServices.Configuration);

        LodestoneNetworkerInterface = LodestoneNetworker = new LodestoneNetworker();

        DirtyHandler = new PettableDirtyHandler();

        PettableUserList = new PettableUserList();
        PettableDatabase = new PettableDatabase(_PetServices, DirtyHandler);
        LegacyDatabase = new LegacyPettableDatabase(_PetServices, DirtyHandler);

        ImageDatabase = new ImageDatabase(_DalamudServices, _PetServices, LodestoneNetworkerInterface);

        DataWriter = new DataWriter(PettableUserList);
        DataParser = new DataParser(_DalamudServices, PettableUserList, PettableDatabase, LegacyDatabase);

        IpcProvider = new IpcProvider(_DalamudServices, _DalamudServices.PetNicknamesPlugin, DataParser, DataWriter);

        HookHandler = new HookHandler(_DalamudServices, _PetServices, PettableUserList, DirtyHandler, PettableDatabase, LegacyDatabase, SharingDictionary, DirtyHandler);
        UpdateHandler = new UpdateHandler(_DalamudServices, PettableUserList, LodestoneNetworker, IpcProvider, ImageDatabase);
        ChatHandler = new ChatHandler(_DalamudServices, _PetServices, PettableUserList);

        // UI is the most DOGSHIT thing in this whole plugin. I hate EVERY SINGLE LINE OF CODE from it...
        // If I had know how unreadable Una.Drawing would make my UI code I wouldve never done it like this....
        WindowHandler = new WindowHandler(_DalamudServices, _PetServices, PettableUserList, PettableDatabase, LegacyDatabase, ImageDatabase, DirtyHandler, DataParser, DataWriter, HookHandler.MapHook);

        CommandHandler = new CommandHandler(_DalamudServices, _PetServices.Configuration, WindowHandler);
        ContextMenuHandler = new ContextMenuHandler(_DalamudServices, _PetServices, PettableUserList, WindowHandler, HookHandler.ActionTooltipHook);

        _PetServices.Configuration.Initialise(_DalamudServices.PetNicknamesPlugin, PettableDatabase, LegacyDatabase);

        SaveHandler = new SaveHandler(_PetServices, PettableUserList, IpcProvider, DirtyHandler);
    }

    public void Dispose()
    {
        SharingDictionary?.Dispose();
        ContextMenuHandler?.Dispose();
        IpcProvider?.Dispose();
        LodestoneNetworker?.Dispose();
        ImageDatabase?.Dispose();
        UpdateHandler?.Dispose();
        HookHandler?.Dispose();
        ChatHandler?.Dispose();
        CommandHandler?.Dispose();
        WindowHandler?.Dispose();
        SaveHandler.Dispose();
    }
}
