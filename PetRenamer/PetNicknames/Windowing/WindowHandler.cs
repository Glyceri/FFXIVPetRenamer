using Dalamud.Interface.Windowing;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Components;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System.Linq;

namespace PetRenamer.PetNicknames.Windowing;

internal class WindowHandler : IWindowHandler
{
    static int _internalCounter = 0;
    public static int InternalCounter { get  => _internalCounter++; }

    PetWindowMode _windowMode = PetWindowMode.Minion;
    public PetWindowMode PetWindowMode { get => _windowMode; set => SetWindowMode(value); }

    readonly DalamudServices DalamudServices;
    readonly Configuration Configuration;
    readonly IPetServices PetServices;
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly ILegacyDatabase LegacyDatabase;
    readonly IImageDatabase ImageDatabase;
    readonly IPettableDirtyListener DirtyListener;
    readonly IDataParser DataParser;
    readonly IDataWriter DataWriter;
    readonly IMapHook MapHook;

    readonly WindowSystem WindowSystem;

    public WindowHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase pettableDatabase, ILegacyDatabase legacyDatabase, IImageDatabase imageDatabase, IPettableDirtyListener dirtyListener, IDataParser dataParser, IDataWriter dataWriter, IMapHook mapHook)
    {
        DalamudServices = dalamudServices;
        Configuration = petServices.Configuration;
        PetServices = petServices;
        UserList = userList;
        Database = pettableDatabase;
        LegacyDatabase = legacyDatabase;
        ImageDatabase = imageDatabase;
        DirtyListener = dirtyListener;
        MapHook = mapHook;

        DataParser = dataParser;
        DataWriter = dataWriter;

        DirtyListener.RegisterOnClearEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyName(HandleDirty);


        WindowSystem = new WindowSystem(PluginConstants.pluginName);
        DalamudServices.PetNicknamesPlugin.UiBuilder.Draw += Draw;
        DalamudServices.PetNicknamesPlugin.UiBuilder.OpenMainUi += Open<PetRenameWindow>;
        DalamudServices.PetNicknamesPlugin.UiBuilder.OpenConfigUi += Open<PetConfigWindow>;

        ComponentLibrary.Initialise(in dalamudServices);

        Register();
    }

    void Register()
    {
        AddWindow(new PetRenameWindow(this, DalamudServices, PetServices, UserList, DirtyListener));
        AddWindow(new PetConfigWindow(this, DalamudServices, Configuration));
        AddWindow(new PetListWindow(this, DalamudServices, PetServices, UserList, Database, LegacyDatabase, ImageDatabase, DataParser, DataWriter));
        AddWindow(new KofiWindow(this, DalamudServices, Configuration));
        AddWindow(new PetDevWindow(this, DalamudServices, Configuration, UserList, MapHook));
    }

    void AddWindow(PetWindow window)
    {
        WindowSystem.AddWindow(window);
        DalamudServices.Framework.Run(() => window.SetPetMode(_windowMode));
    }

    public void Open<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            if (window is not T tWindow) continue;
            tWindow.Open();
        }
    }

    public void Close<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            if (window is not T tWindow) continue;
            tWindow.Close();
        }
    }

    public T? GetWindow<T>() where T : PetWindow
    {
       return WindowSystem.Windows.OfType<T>().FirstOrDefault();
    }

    public void Toggle<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            if (window is not T tWindow) continue;
            tWindow.Toggle();
        }
    }

    public void SetWindowMode(PetWindowMode mode)
    {
        _windowMode = mode;
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            window.SetPetMode(mode);
        }
    }

    bool isDirty = false;

    void HandleDirty(INamesDatabase namesDatabase)
    {
        isDirty = true;
    }

    void HandleDirty(IPettableDatabaseEntry entry)
    {
        isDirty = true;
    }

    void Draw()
    {
        _internalCounter = 0;
        WindowSystem.Draw();

        if (isDirty)
        {
            HandleDirty();
            isDirty = false;
        }

        HandleModeChange();
    }

    void HandleDirty()
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            DalamudServices.Framework.Run(window.NotifyDirty);
        }
    }

    void HandleModeChange()
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            if (!window.RequestsModeChange) continue;
            DalamudServices.Framework.Run(() => SetWindowMode(window.NewMode));
            window.DeclareModeChangedSeen();
            break;
        }
    }

    public void Dispose()
    {
        DalamudServices.PetNicknamesPlugin.UiBuilder.Draw -= Draw;
        ClearAllWindows();

        ComponentLibrary.Dispose();
    }


    void ClearAllWindows()
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            window?.Dispose();
        }

        WindowSystem?.RemoveAllWindows();
    }
}
