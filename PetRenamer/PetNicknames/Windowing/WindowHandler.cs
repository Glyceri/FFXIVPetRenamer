using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.ColourEditorWindow;
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
    readonly IDataParser DataParser;
    readonly IDataWriter DataWriter;
    readonly IColourProfileHandler ColourProfileHandler;

    readonly WindowSystem WindowSystem;

    public WindowHandler(in DalamudServices dalamudServices, in Configuration configuration, in IPetServices petServices, in IPettableUserList userList, in IPettableDatabase pettableDatabase, in ILegacyDatabase legacyDatabase, in IImageDatabase imageDatabase, in IPettableDirtyListener dirtyListener, in IDataParser dataParser, in IDataWriter dataWriter, in IColourProfileHandler colourProfileHandler)
    {
        DalamudServices = dalamudServices;
        Configuration = configuration;
        PetServices = petServices;
        UserList = userList;
        Database = pettableDatabase;
        LegacyDatabase = legacyDatabase;
        ImageDatabase = imageDatabase;
        DirtyListener = dirtyListener;

        DataParser = dataParser;
        DataWriter = dataWriter;

        ColourProfileHandler = colourProfileHandler;

        DirtyListener.RegisterOnClearEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyName(HandleDirty);

        Node.UseThreadedStyleComputation = true;
        DrawingLib.Setup(dalamudServices.PetNicknamesPlugin);

        WindowSystem = new WindowSystem(PluginConstants.pluginName);
        DalamudServices.PetNicknamesPlugin.UiBuilder.Draw += Draw;
        DalamudServices.PetNicknamesPlugin.UiBuilder.OpenMainUi += Open<PetRenameWindow>;
        DalamudServices.PetNicknamesPlugin.UiBuilder.OpenConfigUi += Open<PetConfigWindow>;

        Register();
    }

    void Register()
    {
        AddWindow(new KofiWindow(this, in DalamudServices, in Configuration));
        AddWindow(new PetRenameWindow(this, in DalamudServices, in Configuration,  PetServices, UserList));
        AddWindow(new PetListWindow(this, in DalamudServices, in Configuration, in PetServices, UserList, Database, LegacyDatabase, ImageDatabase, in DataParser, in DataWriter));
        AddWindow(new PetConfigWindow(this, in DalamudServices, in Configuration));
        AddWindow(new ColourEditorWindow(this, in DalamudServices, in Configuration, in ColourProfileHandler));
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

    public void SetKofiMode(bool mode)
    {
        foreach (PetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            window.HeaderBar.SetKofiButton(mode);
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
        if (PetServices.Configuration.petNicknamesUIScale <= 0)
        {
            Node.ScaleFactor = ImGuiHelpers.GlobalScale;
        }
        else
        {
            Node.ScaleFactor = PetServices.Configuration.petNicknamesUIScale;
        }

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
            DalamudServices.Framework.Run(window.OnDirty);
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
        DrawingLib.Dispose();
        DalamudServices.PetNicknamesPlugin.UiBuilder.Draw -= Draw;
        ClearAllWindows();
    }

    public void Rebuild()
    {
        ClearAllWindows();
        Register();
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
