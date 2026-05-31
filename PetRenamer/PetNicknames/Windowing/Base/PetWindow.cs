using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Components.Header;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Base;

internal abstract class PetWindow : Window, IPetWindow
{
    private static readonly Vector2 windowPadding    = new Vector2(8, 8);
    private static readonly Vector2 framePadding     = new Vector2(4, 3);
    private static readonly Vector2 itemInnerSpacing = new Vector2(4, 4);
    private static readonly Vector2 itemSpacing      = new Vector2(4, 4);

    public SkeletonType PetMode { get; private set; }
        = SkeletonType.Minion;

    protected abstract Vector2 MinSize     { get; }
    protected abstract Vector2 MaxSize     { get; }
    protected abstract Vector2 DefaultSize { get; }

    public abstract bool ShowQuickButtons { get; }
    public abstract bool HasModeToggle    { get; }

    protected readonly DalamudServices DalamudServices;
    protected readonly WindowHandler   WindowHandler;
    protected readonly IPetServices    PetServices;
    
    private float lastGlobalScale = 0;

    protected PetWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, string name, ImGuiWindowFlags windowFlags = ImGuiWindowFlags.None) : base(name, windowFlags, true)
    {
        WindowHandler   = windowHandler;
        DalamudServices = dalamudServices;
        PetServices     = petServices;
        
        PetServices.DirtyListener.RegisterOnDirtyConfig(OnDirtyConfig);
        
        SizeCondition   = ImGuiCond.FirstUseEver;
        Size            = DefaultSize;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = MinSize,
            MaximumSize = MaxSize,
        };
        
        SetupTitlebar();
    }
    
    private void OnDirtyConfig(Configuration _)
        => SetupTitlebar();
    
    private void SetupTitlebar()
    {
        TitleBarButtons.Clear();
    
        if (!ShowQuickButtons)
        {
            return;
        }
        
        TitleBarButtons = HeaderBar.HandleHeaderButtons(WindowHandler, PetServices);
    }

    public void Close() 
        => IsOpen = false;

    public void Open()
        => IsOpen = true;
    
    public void SetPetMode(SkeletonType mode)
    {
        if (PetMode == mode)
        {
            return;
        }
        
        PetMode = mode;
        
        OnModeChange();
    }
    
    public Vector2 CurrentPosition 
        { get; private set; } = Vector2.Zero;
    
    public sealed override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    windowPadding * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,     framePadding * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing,      itemSpacing * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, itemInnerSpacing * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowTitleAlign, new Vector2(0, 0.5f));

        float currentGlobalScale = WindowHandler.FontScale;

        if (lastGlobalScale != currentGlobalScale)
        {
            lastGlobalScale = currentGlobalScale;

            SizeCondition   = ImGuiCond.FirstUseEver;
            Size            = DefaultSize * currentGlobalScale;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = MinSize * currentGlobalScale,
                MaximumSize = MaxSize * currentGlobalScale,
            };
        }

        OnEarlyDraw();
    }

    public sealed override void PostDraw()
    {
        OnLateDraw();

        ImGui.PopStyleVar(5);
    }

    public sealed override void Draw()
    {
        CurrentPosition = ImGui.GetWindowPos();
        
        OnDraw();
        
        if (WindowHandler.FocussedWindow != this)
        {
            return;
        }

        ImGui.BeginPopup("RenameWindow", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration);
    }

    public void NotifyDirty() 
        => OnDirty();

    protected virtual void OnEarlyDraw()  { }
    protected virtual void OnDraw()       { }
    protected virtual void OnLateDraw()   { }
    protected virtual void OnDirty()      { }
    protected virtual void OnModeChange() { }
    protected virtual void OnDispose()    { }

    public bool HasFocus
        => IsFocused && IsOpen;
    
    protected string SpeciesLine
        => Translator.GetLine($"PetRenameNode.Species{(int)PetMode}");

    public void Dispose() 
        => OnDispose();
}
