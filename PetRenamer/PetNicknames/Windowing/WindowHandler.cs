using Dalamud.Interface.Windowing;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
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
using Dalamud.Interface.Utility;
using Dalamud.Bindings.ImGui;

namespace PetRenamer.PetNicknames.Windowing;

internal class WindowHandler : IWindowHandler
{
    private static int   _internalCounter  = 0;

    private PetWindowMode _windowMode = PetWindowMode.Minion;

    private readonly DalamudServices        DalamudServices;
    private readonly Configuration          Configuration;
    private readonly IPetServices           PetServices;
    private readonly IPettableUserList      UserList;
    private readonly IPettableDatabase      Database;
    private readonly ILegacyDatabase        LegacyDatabase;
    private readonly IImageDatabase         ImageDatabase;
    private readonly IPettableDirtyListener DirtyListener;
    private readonly IDataParser            DataParser;
    private readonly IDataWriter            DataWriter;
    private readonly WindowSystem           WindowSystem;

    private bool isDirty = false;

    public WindowHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase pettableDatabase, ILegacyDatabase legacyDatabase, IImageDatabase imageDatabase, IPettableDirtyListener dirtyListener, IDataParser dataParser, IDataWriter dataWriter)
    {
        DalamudServices = dalamudServices;
        Configuration   = petServices.Configuration;
        PetServices     = petServices;
        UserList        = userList;
        Database        = pettableDatabase;
        LegacyDatabase  = legacyDatabase;
        ImageDatabase   = imageDatabase;
        DirtyListener   = dirtyListener;

        DataParser      = dataParser;
        DataWriter      = dataWriter;

        DirtyListener.RegisterOnClearEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyName(HandleDirty);


        WindowSystem = new WindowSystem(PluginConstants.pluginName);

        DalamudServices.DalamudPlugin.UiBuilder.Draw         += Draw;
        DalamudServices.DalamudPlugin.UiBuilder.OpenMainUi   += Open<PetRenameWindow>;
        DalamudServices.DalamudPlugin.UiBuilder.OpenConfigUi += Open<PetConfigWindow>;

        ComponentLibrary.Initialise(in dalamudServices);

        Register();
    }

    public static int InternalCounter
        => _internalCounter++;

    public static float FontScale 
        => (ImGui.GetFontSize() / 16.0f);

    public static float GlobalScale
        => ImGuiHelpers.GlobalScale * FontScale;

    public PetWindowMode PetWindowMode
    {
        get => _windowMode;
        set => SetWindowMode(value);
    }

    private void Register()
    {
        AddWindow(new PetRenameWindow(this, DalamudServices, PetServices, UserList, DirtyListener));
        AddWindow(new PetConfigWindow(this, DalamudServices, Configuration, PetServices.PluginWatcher));
        AddWindow(new PetListWindow(this, DalamudServices, PetServices, UserList, Database, LegacyDatabase, ImageDatabase, DataParser, DataWriter));
        AddWindow(new KofiWindow(this, DalamudServices, Configuration));
        AddWindow(new PetDevWindow(this, DalamudServices, Configuration, UserList, Database));
    }

    private void AddWindow(PetWindow window)
    {
        WindowSystem.AddWindow(window);

        _ = DalamudServices.Framework.Run(() =>
        {
            window.SetPetMode(_windowMode);
        });
    }

    public void Open<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            if (window is not T tWindow)
            {
                continue;
            }

            tWindow.Open();
        }
    }

    public void Close<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            if (window is not T tWindow)
            {
                continue;
            }

            tWindow.Close();
        }
    }

    public T? GetWindow<T>() where T : PetWindow
        => WindowSystem.Windows.OfType<T>().FirstOrDefault();
    
    public void Toggle<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            if (window is not T tWindow)
            {
                continue;
            }

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

    private void HandleDirty(INamesDatabase namesDatabase)
        => isDirty = true;

    private void HandleDirty(IPettableDatabaseEntry entry)
        => isDirty = true;

    private void Draw()
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

    private void HandleDirty()
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            _ = DalamudServices.Framework.Run(window.NotifyDirty);
        }
    }

    private void HandleModeChange()
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            if (!window.RequestsModeChange)
            {
                continue;
            }

            _ = DalamudServices.Framework.Run(() =>
            {
                SetWindowMode(window.NewMode);
            });

            window.DeclareModeChangedSeen();

            break;
        }
    }

    public void Dispose()
    {
        DalamudServices.DalamudPlugin.UiBuilder.Draw -= Draw;

        ClearAllWindows();

        ComponentLibrary.Dispose();
    }


    private void ClearAllWindows()
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            window?.Dispose();
        }

        WindowSystem?.RemoveAllWindows();
    }
}
