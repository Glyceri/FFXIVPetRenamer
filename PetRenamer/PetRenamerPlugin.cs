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
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing;
using PetRenamer.PetNicknames.ContextMenus;
using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.IPC;
using System.Reflection;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    public readonly string Version;

    private readonly DalamudServices        _DalamudServices;
    private readonly IPetServices           _PetServices;
    private readonly ISharingDictionary     SharingDictionary;
    private readonly IPettableUserList      PettableUserList;
    private readonly IPettableDatabase      PettableDatabase;
    private readonly ILegacyDatabase        LegacyDatabase;
    private readonly IImageDatabase         ImageDatabase;
    private readonly IWindowHandler         WindowHandler;
    private readonly IDataParser            DataParser;
    private readonly IDataWriter            DataWriter;
    private readonly IpcProvider            IpcProvider;

    // As long as no other module needs one, they won't be interfaced
    private readonly ContextMenuHandler     ContextMenuHandler;
    private readonly UpdateHandler          UpdateHandler;
    private readonly HookHandler            HookHandler;
    private readonly ChatHandler            ChatHandler;
    private readonly CommandHandler         CommandHandler;
    private readonly LodestoneNetworker     LodestoneNetworker;
    private readonly ILodestoneNetworker    LodestoneNetworkerInterface;

    private readonly PettableDirtyHandler   DirtyHandler;

    private readonly SaveHandler            SaveHandler;

    public PetRenamerPlugin(IDalamudPluginInterface dalamud)
    {
        Version                     = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown Version";

        _DalamudServices            = DalamudServices.Create(dalamud, this)!;

        PettableUserList            = new PettableUserList();

        _PetServices                = new PetServices(_DalamudServices, PettableUserList);

        SharingDictionary           = new SharingDictionary(_DalamudServices);

        Translator.Initialise(_DalamudServices, _PetServices.Configuration);

        LodestoneNetworkerInterface = LodestoneNetworker = new LodestoneNetworker();

        DirtyHandler                = new PettableDirtyHandler();

        PettableDatabase            = new PettableDatabase(_PetServices, DirtyHandler);
        LegacyDatabase              = new LegacyPettableDatabase(_PetServices, DirtyHandler);

        ImageDatabase               = new ImageDatabase(_DalamudServices, _PetServices, LodestoneNetworkerInterface);

        DataWriter                  = new DataWriter(PettableUserList);
        DataParser                  = new DataParser(_DalamudServices, PettableUserList, PettableDatabase, LegacyDatabase);

        IpcProvider                 = new IpcProvider(_DalamudServices, _DalamudServices.DalamudPlugin, DataParser, DataWriter);

        HookHandler                 = new HookHandler(_DalamudServices, _PetServices, PettableUserList, DirtyHandler, PettableDatabase, LegacyDatabase, SharingDictionary, DirtyHandler);
        UpdateHandler               = new UpdateHandler(_DalamudServices, PettableUserList, LodestoneNetworker, IpcProvider, ImageDatabase, _PetServices, HookHandler.IslandHook, DirtyHandler, PettableDatabase);
        ChatHandler                 = new ChatHandler(_DalamudServices, _PetServices, PettableUserList);

        WindowHandler               = new WindowHandler(_DalamudServices, _PetServices, PettableUserList, PettableDatabase, LegacyDatabase, ImageDatabase, DirtyHandler, DataParser, DataWriter);

        CommandHandler              = new CommandHandler(_DalamudServices, WindowHandler, _PetServices, PettableUserList, PettableDatabase);
        ContextMenuHandler          = new ContextMenuHandler(_DalamudServices, _PetServices, PettableUserList, WindowHandler, HookHandler.ActionTooltipHook);

        _PetServices.Configuration.Initialise(_DalamudServices.DalamudPlugin, PettableDatabase, LegacyDatabase);

        SaveHandler                 = new SaveHandler(_PetServices, PettableUserList, IpcProvider, DirtyHandler);
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
