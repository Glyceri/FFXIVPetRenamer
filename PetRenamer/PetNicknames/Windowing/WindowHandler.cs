using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.EmptyWindow;
using PetRenamer.PetNicknames.Windowing.Windows.PetConfigWindow;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;
using PetRenamer.PetNicknames.Windowing.Windows.PetShareWindow;
using PetRenamer.PetNicknames.Windowing.Windows.TempWindow;
using System.Linq;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing;

// Where normal Pet nicknames code is.... f i n e
// UI code is such a focking mess, I don't want to even look at it anymore

internal class WindowHandler : IWindowHandler
{
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

    readonly WindowSystem WindowSystem;

    public WindowHandler(in DalamudServices dalamudServices, in Configuration configuration, in IPetServices petServices, in IPettableUserList userList, in IPettableDatabase pettableDatabase, in ILegacyDatabase legacyDatabase, in IImageDatabase imageDatabase, in IPettableDirtyListener dirtyListener)
    {
        DalamudServices = dalamudServices;
        Configuration = configuration;
        PetServices = petServices;
        UserList = userList;
        Database = pettableDatabase;
        LegacyDatabase = legacyDatabase;
        ImageDatabase = imageDatabase;
        DirtyListener = dirtyListener;

        DirtyListener.RegisterOnClearEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyName(HandleDirty);

        Node.UseThreadedStyleComputation = true;
        DrawingLib.Setup(dalamudServices.PetNicknamesPlugin);
        WindowStyles.RegisterDefaultColors();

        WindowSystem = new WindowSystem(PluginConstants.pluginName);
        DalamudServices.PetNicknamesPlugin.UiBuilder.Draw += Draw;
        DalamudServices.PetNicknamesPlugin.UiBuilder.OpenMainUi += Open<PetRenameWindow>;
        DalamudServices.PetNicknamesPlugin.UiBuilder.OpenConfigUi += Open<PetConfigWindow>;

        _Register();
    }

    void _Register()
    {
        AddWindow(new PetRenameWindow(this, in DalamudServices, in Configuration,  PetServices, UserList));
        AddWindow(new PetListWindow(this, in DalamudServices, in Configuration, in PetServices, UserList, Database, LegacyDatabase, ImageDatabase));
        AddWindow(new EmptyWindow(this, in DalamudServices, in Configuration, in PetServices, UserList, Database, LegacyDatabase, ImageDatabase));
        AddWindow(new PetSharingWindow(this, in DalamudServices, in Configuration, ImageDatabase));
        AddWindow(new PetConfigWindow(this, in DalamudServices, in Configuration));
    }

    void AddWindow(PetWindow window)
    {
        WindowSystem.AddWindow(window);
        DalamudServices.Framework.Run(() => window.SetPetMode(_windowMode));
    }

    public void Open<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows)
        {
            if (window is not T tWindow) continue;
            tWindow.Open();
        }
    }

    public void Close<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows)
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
        foreach (IPetWindow window in WindowSystem.Windows)
        {
            if (window is not T tWindow) continue;
            tWindow.Toggle();
        }
    }

    public void SetWindowMode(PetWindowMode mode)
    {
        _windowMode = mode;
        foreach (IPetWindow window in WindowSystem.Windows)
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
        Node.ScaleFactor = ImGuiHelpers.GlobalScale;


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
        foreach (IPetWindow window in WindowSystem.Windows)
        {
            DalamudServices.Framework.Run(window.OnDirty);
        }
    }

    void HandleModeChange()
    {
        foreach (IPetWindow window in WindowSystem.Windows)
        {
            if (!window.RequestsModeChange) continue;
            DalamudServices.Framework.Run(() => SetWindowMode(window.NewMode));
            window.DeclareModeChangedSeen();
            break;
        }
    }

    public void Dispose()
    {
        DrawingLib.Dispose();
        DalamudServices.PetNicknamesPlugin.UiBuilder.Draw -= Draw;
        WindowSystem?.RemoveAllWindows();
    }

    public void Rebuild()
    {
        WindowSystem?.RemoveAllWindows();
        _Register();
    }
}
