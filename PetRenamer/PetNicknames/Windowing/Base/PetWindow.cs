using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using PetNicknames.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Components.Header;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Base;

internal abstract partial class PetWindow : Window, IPetWindow, IPetMode
{
    private static readonly Vector2 windowPadding    = new Vector2(8, 8);
    private static readonly Vector2 framePadding     = new Vector2(4, 3);
    private static readonly Vector2 itemInnerSpacing = new Vector2(4, 4);
    private static readonly Vector2 itemSpacing      = new Vector2(4, 4);

    public PetWindowMode CurrentMode { get; private set; }

    protected abstract Vector2 MinSize     { get; }
    protected abstract Vector2 MaxSize     { get; }
    protected abstract Vector2 DefaultSize { get; }

    protected abstract bool HasModeToggle  { get; }

    protected readonly DalamudServices DalamudServices;
    protected readonly WindowHandler   WindowHandler;
    protected readonly Configuration   Configuration;

    public bool RequestsModeChange { get; set; }

    public PetWindowMode NewMode { get; set; } 
        = PetWindowMode.Minion;

    private float lastGlobalScale = 0;

    protected PetWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, string name, ImGuiWindowFlags windowFlags = ImGuiWindowFlags.None) : base(name, windowFlags, true)
    {
        WindowHandler   = windowHandler;
        DalamudServices = dalamudServices;
        Configuration   = configuration;

        SizeCondition   = ImGuiCond.FirstUseEver;
        Size            = DefaultSize;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = MinSize,
            MaximumSize = MaxSize,
        };
    }

    public void Close() 
        => IsOpen = false;

    public void Open()
        => IsOpen = true;

    public void DeclareModeChangedSeen() 
        => RequestsModeChange = false;

    public void SetPetMode(PetWindowMode mode)
    {
        if (CurrentMode != mode)
        {
            CurrentMode = mode;
            OnModeChange();
        }
    }

    public sealed override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,    windowPadding * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,     framePadding * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing,      itemSpacing * WindowHandler.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, itemInnerSpacing * WindowHandler.GlobalScale);

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

        ImGui.PopStyleVar(4);
    }

    public sealed override void Draw()
    {
        if (HasModeToggle)
        {
            HeaderBar.Draw(DalamudServices, WindowHandler, Configuration, this);
        }

        OnDraw();
    }

    public void NotifyDirty() 
        => OnDirty();

    protected virtual void OnEarlyDraw()  { }
    protected virtual void OnDraw()       { }
    protected virtual void OnLateDraw()   { }
    protected virtual void OnDirty()      { }
    protected virtual void OnModeChange() { }
    protected virtual void OnDispose()    { }

    protected void RequestPetModeChange(PetWindowMode newMode)
    {
        RequestsModeChange = true;
        NewMode            = newMode;
    }

    public void Dispose() 
        => OnDispose();
}
