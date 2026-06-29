using Dalamud.Plugin;
using PetRenamer.PetNicknames.Hooking;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
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
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing;
using PetRenamer.PetNicknames.ContextMenus;
using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.IPC;

namespace PetRenamer;

// ReSharper disable once UnusedType.Global
public sealed class PetRenamerPlugin : IDalamudPlugin
{
    private readonly DalamudServices        DalamudServices;
    private readonly IPetServices           PetServices;
    private readonly ISharingDictionary     SharingDictionary;
    private readonly IPettableDatabase      PettableDatabase;
    private readonly ILegacyDatabase        LegacyDatabase;
    private readonly IImageDatabase         ImageDatabase;
    private readonly IWindowHandler         WindowHandler;
    private readonly IDataParser            DataParser;
    private readonly IDataWriter            DataWriter;
    private readonly IDataChecker           DataChecker;
    private readonly IpcProvider            IpcProvider;
    private readonly IPenumbraIPC           PenumbraIPC;
    private readonly ContextMenuHandler     ContextMenuHandler;
    private readonly UpdateHandler          UpdateHandler;
    private readonly HookHandler            HookHandler;
    //private readonly ChatHandler            ChatHandler;
    private readonly CommandHandler         CommandHandler;
    private readonly LodestoneNetworker     LodestoneNetworker;
    private readonly SaveHandler            SaveHandler;

    public PetRenamerPlugin(IDalamudPluginInterface dalamud)
    {
        DalamudServices    = DalamudServices.Create(dalamud);

        PetServices        = new PetServices(DalamudServices);

        SharingDictionary  = new SharingDictionary(DalamudServices);

        Translator.Initialise(DalamudServices, PetServices);

        LodestoneNetworker = new LodestoneNetworker(PetServices);

        PettableDatabase   = new PettableDatabase(PetServices);
        LegacyDatabase     = new LegacyPettableDatabase(PetServices);

        ImageDatabase      = new ImageDatabase(DalamudServices, PetServices, LodestoneNetworker);

        DataWriter         = new DataWriter(PetServices);
        DataParser         = new DataParser(DalamudServices, PetServices, PettableDatabase, LegacyDatabase);
        DataChecker        = new DataChecker(PetServices);
        
        IpcProvider        = new IpcProvider(DalamudServices, PetServices, DataParser, DataWriter, DataChecker);
        PenumbraIPC        = new PenumbraIPC(PetServices, DalamudServices.DalamudPlugin, DataWriter, DataParser);

        HookHandler        = new HookHandler(DalamudServices, PetServices, PettableDatabase, LegacyDatabase, SharingDictionary);

        SaveHandler        = new SaveHandler(PetServices, IpcProvider);

        UpdateHandler      = new UpdateHandler(DalamudServices, PetServices, LodestoneNetworker, IpcProvider, ImageDatabase, SaveHandler);
        //ChatHandler        = new ChatHandler(DalamudServices, PetServices, HookHandler.PronounHook);

        WindowHandler      = new WindowHandler(DalamudServices, PetServices, PettableDatabase, LegacyDatabase, ImageDatabase, DataParser, DataWriter, SharingDictionary, HookHandler.PronounHook);

        CommandHandler     = new CommandHandler(DalamudServices, PetServices, WindowHandler);
        ContextMenuHandler = new ContextMenuHandler(DalamudServices, PetServices, WindowHandler);

        PetServices.Configuration.Initialise(DalamudServices.DalamudPlugin, PettableDatabase, LegacyDatabase, PetServices);
    }

    public void Dispose()
    {
        SharingDictionary.Dispose();
        ContextMenuHandler.Dispose();
        IpcProvider.Dispose();
        PenumbraIPC.Dispose();
        LodestoneNetworker.Dispose();
        ImageDatabase.Dispose();
        UpdateHandler.Dispose();
        HookHandler.Dispose();
        //ChatHandler.Dispose();
        CommandHandler.Dispose();
        WindowHandler.Dispose();
        SaveHandler.Dispose();
        PetServices.Dispose();
    }
}
