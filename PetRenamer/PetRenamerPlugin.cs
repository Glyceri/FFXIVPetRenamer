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
using System;
using System.Reflection;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.IPC;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.ColourProfiling;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    // Quite frankly this can be static
    public static readonly Version PuginVersion = Assembly.GetAssembly(typeof(PetRenamerPlugin))?.GetName()?.Version ?? new Version("vERROR");

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
    readonly IColourProfileHandler ColourProfileHandler;

    readonly SaveHandler SaveHandler;

    public PetRenamerPlugin(IDalamudPluginInterface dalamud)
    {
        _DalamudServices = DalamudServices.Create(ref dalamud)!;
        _PetServices = new PetServices(_DalamudServices);

        SharingDictionary = new SharingDictionary(_DalamudServices);

        Translator.Initialise(_DalamudServices, _PetServices.Configuration);

        LodestoneNetworkerInterface = LodestoneNetworker = new LodestoneNetworker();

        DirtyHandler = new PettableDirtyHandler(_DalamudServices);

        ColourProfileHandler = new ColourProfileHandler(_PetServices.Configuration);

        PettableUserList = new PettableUserList();
        PettableDatabase = new PettableDatabase(in _PetServices, DirtyHandler);
        LegacyDatabase = new LegacyPettableDatabase(in _PetServices, DirtyHandler);

        ImageDatabase = new ImageDatabase(in _DalamudServices, in _PetServices, in LodestoneNetworkerInterface);

        DataWriter = new DataWriter(in PettableUserList);
        DataParser = new DataParser(in _DalamudServices, in PettableUserList, in PettableDatabase, in LegacyDatabase);

        IpcProvider = new IpcProvider(_DalamudServices.PetNicknamesPlugin, in DataParser, in DataWriter);

        UpdateHandler = new UpdateHandler(in _DalamudServices, in SharingDictionary, in PettableUserList, LegacyDatabase, in PettableDatabase, in _PetServices, in LodestoneNetworker, in ImageDatabase, DirtyHandler);
        HookHandler = new HookHandler(in _DalamudServices, in _PetServices, in PettableUserList, DirtyHandler);
        ChatHandler = new ChatHandler(in _DalamudServices, in _PetServices, in PettableUserList);

        // UI is the most DOGSHIT thing in this whole plugin. I hate EVERY SINGLE LINE OF CODE from it...
        // If I had know how unreadable Una.Drawing would make my UI code I wouldve never done it like this....
        // This project was such a pleasure to work on before UI
        // So fun, so great
        // Then UI came along...
        // Ive lost interest in writing this code
        // I do not even care if it looks good anymore
        // If some UI element shows weird or doesnt align properly
        // Help me find the motivation to fix it, because I truly couldnt care less anymore
        WindowHandler = new WindowHandler(in _DalamudServices, _PetServices.Configuration, in _PetServices, in PettableUserList, in PettableDatabase, in LegacyDatabase, in ImageDatabase, DirtyHandler, in DataParser, in DataWriter, in ColourProfileHandler);

        ColourProfileHandler.RegisterWindowHandler(in WindowHandler);
        CommandHandler = new CommandHandler(in _DalamudServices, in WindowHandler);
        ContextMenuHandler = new ContextMenuHandler(in _DalamudServices, in _PetServices, in PettableUserList, in WindowHandler, HookHandler.ActionTooltipHook);

        _PetServices.Configuration.Initialise(_DalamudServices.PetNicknamesPlugin, PettableDatabase, LegacyDatabase, ColourProfileHandler);
        IpcProvider.Prepare();

        SaveHandler = new SaveHandler(_DalamudServices, _PetServices.Configuration, in PettableUserList, IpcProvider, DirtyHandler);
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
