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

    private readonly DalamudServices        DalamudServices;
    private readonly IPetServices           PetServices;
    private readonly ISharingDictionary     SharingDictionary;
    private readonly IPettableUserList      PettableUserList;
    private readonly IPettableDatabase      PettableDatabase;
    private readonly ILegacyDatabase        LegacyDatabase;
    private readonly IImageDatabase         ImageDatabase;
    private readonly IWindowHandler         WindowHandler;
    private readonly IDataParser            DataParser;
    private readonly IDataWriter            DataWriter;
    private readonly IpcProvider            IpcProvider;
    private readonly IPenumbraIPC           PenumbraIPC;

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

        DalamudServices             = DalamudServices.Create(dalamud, this)!;

        PettableUserList            = new PettableUserList();

        PetServices                 = new PetServices(DalamudServices, PettableUserList);

        SharingDictionary           = new SharingDictionary(DalamudServices);

        Translator.Initialise(DalamudServices, PetServices.Configuration);

        LodestoneNetworkerInterface = LodestoneNetworker = new LodestoneNetworker();

        DirtyHandler                = new PettableDirtyHandler();

        PettableDatabase            = new PettableDatabase(PetServices, DirtyHandler);
        LegacyDatabase              = new LegacyPettableDatabase(PetServices, DirtyHandler);

        ImageDatabase               = new ImageDatabase(DalamudServices, PetServices, LodestoneNetworkerInterface);

        DataWriter                  = new DataWriter(PettableUserList);
        DataParser                  = new DataParser(DalamudServices, PetServices, PettableUserList, PettableDatabase, LegacyDatabase);

        IpcProvider                 = new IpcProvider(DalamudServices, DalamudServices.DalamudPlugin, DataParser, DataWriter);
        PenumbraIPC                 = new PenumbraIPC(PetServices, DalamudServices.DalamudPlugin, DataWriter, DataParser);

        HookHandler                 = new HookHandler(DalamudServices, PetServices, PettableUserList, DirtyHandler, PettableDatabase, LegacyDatabase, SharingDictionary, DirtyHandler);
        UpdateHandler               = new UpdateHandler(DalamudServices, PettableUserList, LodestoneNetworker, IpcProvider, ImageDatabase, PetServices, HookHandler.IslandHook, DirtyHandler, PettableDatabase);
        ChatHandler                 = new ChatHandler(DalamudServices, PetServices, PettableUserList);

        WindowHandler               = new WindowHandler(DalamudServices, PetServices, PettableUserList, PettableDatabase, LegacyDatabase, ImageDatabase, DirtyHandler, DataParser, DataWriter);

        CommandHandler              = new CommandHandler(DalamudServices, WindowHandler, PetServices, PettableUserList, PettableDatabase);
        ContextMenuHandler          = new ContextMenuHandler(DalamudServices, PetServices, PettableUserList, WindowHandler, HookHandler.ActionTooltipHook);

        PetServices.Configuration.Initialise(DalamudServices.DalamudPlugin, PettableDatabase, LegacyDatabase);

        SaveHandler                 = new SaveHandler(PetServices, PettableUserList, IpcProvider, DirtyHandler);
    }

    public void Dispose()
    {
        SharingDictionary?.Dispose();
        ContextMenuHandler?.Dispose();
        IpcProvider?.Dispose();
        PenumbraIPC?.Dispose();
        LodestoneNetworker?.Dispose();
        ImageDatabase?.Dispose();
        UpdateHandler?.Dispose();
        HookHandler?.Dispose();
        ChatHandler?.Dispose();
        CommandHandler?.Dispose();
        WindowHandler?.Dispose();
        SaveHandler.Dispose();
        PetServices?.Dispose();
    }
}
