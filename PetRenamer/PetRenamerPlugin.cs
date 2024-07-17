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
using PetRenamer.PetNicknames.ContextMenus;
using PetRenamer.PetNicknames.Serialization;
using System;
using System.Reflection;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    // Quite frankly this can be static
    public static readonly Version PuginVersion = Assembly.GetAssembly(typeof(PetRenamerPlugin))?.GetName()?.Version ?? new Version("vERROR");

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
        _DalamudServices = DalamudServices.Create(ref dalamud)!;
        _PetServices = new PetServices(_DalamudServices);

        Translator.Initialise(_DalamudServices);

        LodestoneNetworkerInterface = LodestoneNetworker = new LodestoneNetworker();

        DirtyHandler = new PettableDirtyHandler(_DalamudServices);

        PettableUserList = new PettableUserList();
        PettableDatabase = new PettableDatabase(in _PetServices, DirtyHandler);
        LegacyDatabase = new LegacyPettableDatabase(in _PetServices, DirtyHandler);

        ImageDatabase = new ImageDatabase(in _DalamudServices, in _PetServices, in LodestoneNetworkerInterface);

        DataWriter = new DataWriter(in PettableUserList);
        DataParser = new DataParser(in _DalamudServices, in PettableUserList, in PettableDatabase, in LegacyDatabase);

        IpcProvider = new IpcProvider(in _PetServices, _DalamudServices.PetNicknamesPlugin, in DataParser, in DataWriter);

        UpdateHandler = new UpdateHandler(in _DalamudServices, in PettableUserList, LegacyDatabase, in PettableDatabase, in _PetServices, in LodestoneNetworker, in ImageDatabase, DirtyHandler);
        HookHandler = new HookHandler(in _DalamudServices, in _PetServices, in PettableUserList, DirtyHandler);
        ChatHandler = new ChatHandler(in _DalamudServices, in _PetServices, in PettableUserList);
        CommandHandler = new CommandHandler(_DalamudServices, _PetServices, PettableUserList);
        WindowHandler = new WindowHandler(in _DalamudServices, _PetServices.Configuration, in _PetServices, in PettableUserList, in PettableDatabase, in LegacyDatabase, in ImageDatabase, DirtyHandler);
        ContextMenuHandler = new ContextMenuHandler(in _DalamudServices, in _PetServices, in PettableUserList, in WindowHandler, HookHandler.ActionTooltipHook);

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

        _DalamudServices.CommandManager.AddHandler("/petrestart", new Dalamud.Game.Command.CommandInfo((s, ss) => WindowHandler.Rebuild())
        {
            HelpMessage = "Temporary",
            ShowInHelp = true
        });

        _PetServices.Configuration.Initialise(_DalamudServices.PetNicknamesPlugin, PettableDatabase, LegacyDatabase);
        IpcProvider.Prepare();

        SaveHandler = new SaveHandler(_DalamudServices, _PetServices.Configuration, in PettableUserList, IpcProvider, DirtyHandler);
    }

    public void Dispose()
    {
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
