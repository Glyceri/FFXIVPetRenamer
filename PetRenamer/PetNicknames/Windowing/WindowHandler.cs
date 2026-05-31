using Dalamud.Interface.Windowing;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Components;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System.Linq;
using Dalamud.Interface.Utility;
using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.IPC.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing;

internal class WindowHandler : IWindowHandler
{
    private static int _internalCounter;
    
    private readonly DalamudServices    DalamudServices;
    private readonly IPetServices       PetServices;
    private readonly IPettableDatabase  Database;
    private readonly ILegacyDatabase    LegacyDatabase;
    private readonly IImageDatabase     ImageDatabase;
    private readonly IDataParser        DataParser;
    private readonly IDataWriter        DataWriter;
    private readonly WindowSystem       WindowSystem;
    private readonly ISharingDictionary SharingDictionary;
    private readonly IPronounHook       PronounHook;

    private bool isDirty;
    
    private IPetWindow? lastFocussedWindow = null;
    
    public WindowHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableDatabase pettableDatabase, ILegacyDatabase legacyDatabase, IImageDatabase imageDatabase, IDataParser dataParser, IDataWriter dataWriter, ISharingDictionary sharingDictionary, IPronounHook pronounHook)
    {
        DalamudServices       = dalamudServices;
        PetServices           = petServices;
        Database              = pettableDatabase;
        LegacyDatabase        = legacyDatabase;
        ImageDatabase         = imageDatabase;
        DataParser            = dataParser;
        DataWriter            = dataWriter;
        SharingDictionary     = sharingDictionary;
        PronounHook           = pronounHook;

        PetServices.DirtyListener.RegisterOnClearEntry(HandleDirty);
        PetServices.DirtyListener.RegisterOnDirtyEntry(HandleDirty);
        PetServices.DirtyListener.RegisterOnDirtyName(HandleDirty);
        
        WindowSystem = new WindowSystem(PluginConstants.pluginName);

        DalamudServices.DalamudPlugin.UiBuilder.Draw         += Draw;
        DalamudServices.DalamudPlugin.UiBuilder.OpenMainUi   += Open<PetRenameWindow>;
        DalamudServices.DalamudPlugin.UiBuilder.OpenConfigUi += Open<PetConfigWindow>;

        ComponentLibrary.Initialise(in dalamudServices);

        Register();
    }
    
    public IPetWindow? FocussedWindow 
        { get; private set; } = null;

    public static int InternalCounter
        => _internalCounter++;

    // The 16 is because this plugin was made for exlusively dalamud font size 12 (which is font scale 16 in ImGUI).
    // Scaling the whole UI thingy around it seems to work perfectly fine
    public static float FontScale 
        => (ImGui.GetFontSize() / 16.0f);

    public static float GlobalScale
        => ImGuiHelpers.GlobalScale * FontScale;

    public static float BarHeight
        => 30 * GlobalScale;

    public static Vector2 StretchingBar
        => new Vector2(ImGui.GetContentRegionAvail().X, BarHeight);

    private void Register()
    {
        AddWindow(new PetRenameWindow(this, DalamudServices, PetServices));
        AddWindow(new PetConfigWindow(this, DalamudServices, PetServices));
        AddWindow(new PetListWindow(this, DalamudServices, PetServices, Database, LegacyDatabase, ImageDatabase, DataParser, DataWriter));
        AddWindow(new KofiWindow(this, DalamudServices, PetServices));
        AddWindow(new PetDevWindow(this, DalamudServices, PetServices, Database, SharingDictionary, PronounHook));
        AddWindow(new PetModeWindow(this, DalamudServices, PetServices));
    }

    private void AddWindow(PetWindow window)
    {
        WindowSystem.AddWindow(window);
    }

    public void Open<T>() 
        where T : IPetWindow
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

    public void Close<T>() 
        where T : IPetWindow
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

    public T? GetWindow<T>() 
        where T : PetWindow
        => WindowSystem.Windows.OfType<T>().FirstOrDefault();
    
    public void Toggle<T>() 
        where T : IPetWindow
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

    private void HandleDirty(INamesDatabase namesDatabase)
        => isDirty = true;

    private void HandleDirty(IPettableDatabaseEntry entry)
        => isDirty = true;

    private void Draw()
    {
        _internalCounter = 0;

        WindowSystem.Draw();
        
        HandleFocussedWindow();
        
        if (!isDirty)
        {
            return;
        }
        
        isDirty = false;
        
        HandleDirty();
    }
    
    private void HandleFocussedWindow()
    {
        FocussedWindow = null;
        
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            if (!window.HasFocus || !window.HasModeToggle)
            {
                continue;
            }
            
            FocussedWindow = window;
            
            break;
        }
        
        if (lastFocussedWindow == FocussedWindow)
        {
            return;
        }
        
        lastFocussedWindow = FocussedWindow;
        
        if (FocussedWindow != null)
        {
            Open<PetModeWindow>();
        }
        else
        {
            Close<PetModeWindow>();
        }
    }
    
    private void HandleDirty()
    {
        foreach (IPetWindow window in WindowSystem.Windows.Cast<PetWindow>())
        {
            _ = DalamudServices.Framework.Run(window.NotifyDirty);
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
