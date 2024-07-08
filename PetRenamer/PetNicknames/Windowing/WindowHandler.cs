﻿using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing;

internal class WindowHandler : IWindowHandler
{
    PetWindowMode _windowMode = PetWindowMode.Minion;
    public PetWindowMode PetWindowMode { get => _windowMode; set => SetWindowMode(value); }

    readonly DalamudServices DalamudServices;
    readonly IPetServices PetServices;
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;

    WindowSystem WindowSystem;

    public WindowHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList pettableUserList, IPettableDatabase pettableDatabase)
    {
        DrawingLib.Setup(dalamudServices.PetNicknamesPlugin);
        WindowStyles.RegisterDefaultColors();

        DalamudServices = dalamudServices;
        PetServices = petServices;
        UserList = pettableUserList;
        Database = pettableDatabase;

        WindowSystem = new WindowSystem(PluginConstants.pluginName);

        DalamudServices.PetNicknamesPlugin.UiBuilder.Draw += Draw;
    }

    public void AddWindow(PetWindow window)
    {        
        WindowSystem.AddWindow(window);
        window.SetPetMode(_windowMode);
    }

    public void RemoveWindow(PetWindow window)
    {
        WindowSystem.RemoveWindow(window);
    }

    public void Open<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows)
            if (window is T tWindow)
                tWindow.Open();
    }

    public void Close<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows)
            if (window is T tWindow)
                tWindow.Close();
    }

    public T? GetWindow<T>() where T : PetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows)
            if (window is T tWindow)
                return tWindow;
        return null;
    }

    public void Toggle<T>() where T : IPetWindow
    {
        foreach (IPetWindow window in WindowSystem.Windows)
            if (window is T tWindow)
                tWindow.Toggle();
    }

    public void SetWindowMode(PetWindowMode mode)
    {
        _windowMode = mode;
        foreach (IPetWindow window in WindowSystem.Windows)
        {
            window.SetPetMode(mode);
        }
    }

    void Draw()
    {
        Node.ScaleFactor = ImGuiHelpers.GlobalScale;
        WindowSystem.Draw();

        foreach (IPetWindow window in WindowSystem.Windows)
        {
            if (!window.RequestsModeChange) continue;
            SetWindowMode(window.NewMode);
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
}