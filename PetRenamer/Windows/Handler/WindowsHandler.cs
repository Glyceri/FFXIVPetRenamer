using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PetRenamer.Windows.Handler;

internal class WindowsHandler : RegistryBase<PetWindow, PersistentPetWindowAttribute>
{
    WindowSystem windowSystem = new WindowSystem("Pet Nicknames");
    public WindowSystem WindowSystem { get => windowSystem; }

    List<PetWindow> petWindows => elements;
    List<TemporaryPetWindow> temporaryPetWindows = new List<TemporaryPetWindow>();

    public WindowsHandler() : base()
    {
        PluginHandlers.PluginInterface.UiBuilder.Draw += Draw;
    }
    ~WindowsHandler()
    {
        PluginHandlers.PluginInterface.UiBuilder.Draw -= Draw;
        RemoveAllWindows();
    }

    public PetWindow GetWindow(Type windowType) => GetElement(windowType);
    public T GetWindow<T>() where T : PetWindow => (T)GetWindow(typeof(T));

    protected override void OnElementCreation(PetWindow element)
    {
        windowSystem.AddWindow(element);

        Type t = element.GetType();

        if (t.GetCustomAttribute<ModeTogglePetWindowAttribute>() != null)
            t.GetField("drawToggle", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(element, true);

        if (t.GetCustomAttribute<ConfigPetWindowAttribute>() != null)
            PluginHandlers.PluginInterface.UiBuilder.OpenConfigUi += () => PluginLink.WindowHandler.ToggleWindow(element.GetType());
    }



    public T AddTemporaryWindow<T>(string message, Action<object> callback, Window blackenedWindow = null!) where T : TemporaryPetWindow
    {
        TemporaryPetWindow petWindow = (Activator.CreateInstance(typeof(T), new object[3] { message, callback, blackenedWindow }) as TemporaryPetWindow)!;
        temporaryPetWindows.Add(petWindow);
        windowSystem.AddWindow(petWindow);
        return (T)petWindow;
    }

    public void ToggleWindow<T>() where T : PetWindow => GetWindow<T>().IsOpen = !GetWindow<T>().IsOpen;
    public void CloseWindow<T>() where T : PetWindow => GetWindow<T>().IsOpen = false;
    public void OpenWindow<T>() where T : PetWindow => GetWindow<T>().IsOpen = true;

    public void ToggleWindow(Type windowType) => GetWindow(windowType).IsOpen = !GetWindow(windowType).IsOpen;
    public void CloseWindow(Type windowType) => GetWindow(windowType).IsOpen = false;
    public void OpenWindow(Type windowType) => GetWindow(windowType).IsOpen = true;

    public void RemoveAllWindows()
    {
        windowSystem.RemoveAllWindows();

        ClearAllElements();
    }

    public void CloseAllWindows()
    {
        foreach (PetWindow window in petWindows)
            window.IsOpen = false;
    }

    public void Draw()
    {
        for (int i = temporaryPetWindows.Count - 1; i >= 0; i--)
            if (temporaryPetWindows[i].closed)
            {
                windowSystem.RemoveWindow(temporaryPetWindows[i]);
                temporaryPetWindows.RemoveAt(i);
            }

        windowSystem.Draw();
    }

    bool initialized = false;

    internal void Initialize()
    {
        if (initialized) return;
        initialized = true;

        foreach (PetWindow element in elements)
            if(element is InitializablePetWindow initWindow)
                initWindow.OnInitialized();
    }
}
