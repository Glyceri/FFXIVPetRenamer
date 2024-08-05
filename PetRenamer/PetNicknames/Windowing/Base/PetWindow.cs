using Dalamud.Interface.Windowing;
using ImGuiNET;
using PetNicknames.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Base;

internal abstract partial class PetWindow : Window, IPetWindow, IPetMode
{
    public PetWindowMode CurrentMode { get; private set; }

    protected abstract Vector2 MinSize { get; }
    protected abstract Vector2 MaxSize { get; }
    protected abstract Vector2 DefaultSize { get; }

    protected abstract bool HasModeToggle { get; }
    protected abstract bool HasExtraButtons { get; }

    protected readonly DalamudServices DalamudServices;
    protected readonly WindowHandler WindowHandler;
    protected readonly Configuration Configuration;

    public bool RequestsModeChange { get; private set; }
    public PetWindowMode NewMode { get; private set; } = PetWindowMode.Minion;

    protected PetWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, string name, ImGuiWindowFlags windowFlags = ImGuiWindowFlags.None) : base(name, windowFlags, true)
    {
        WindowHandler = windowHandler;
        DalamudServices = dalamudServices;
        Configuration = configuration;

        SizeCondition = ImGuiCond.FirstUseEver;
        Size = DefaultSize;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = MinSize,
            MaximumSize = MaxSize,
        };
      
        //if (HasModeToggle) PetModeConstructor();
    }

    public void Close() => IsOpen = false;
    public void Open() => IsOpen = true;
    public void DeclareModeChangedSeen() => RequestsModeChange = false;

    public void SetPetMode(PetWindowMode mode)
    {
        if (CurrentMode != mode)
        {
            CurrentMode = mode;
            OnModeChange();
        }
    }

    const float IndentSpacing = 21f;
    const float ChildBorderSize = 1;
    readonly Vector2 cellPadding = new(4, 2);
    readonly Vector2 framePadding = new(4, 3);
    readonly Vector2 itemInnerSpacing = new(4, 4);
    readonly Vector2 itemSpacing = new(8, 4);
    readonly Vector2 windowPadding = new(8, 8);

    public sealed override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, windowPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, framePadding);
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, cellPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, itemSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, itemInnerSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, IndentSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, ChildBorderSize);

        OnEarlyDraw();
    }

    public sealed override void PostDraw()
    {
        OnLateDraw();
        ImGui.PopStyleVar(7);
    }

    public sealed override void Draw() => OnDraw();
    public void NotifyDirty() => OnDirty();

    protected virtual void OnEarlyDraw() { }
    protected virtual void OnDraw() { }
    protected virtual void OnLateDraw() { }
    protected virtual void OnDirty() { }
    protected virtual void OnModeChange() { }
    protected virtual void OnDispose() { }

    protected void RequestPetModeChange(PetWindowMode newMode)
    {
        RequestsModeChange = true;
        NewMode = newMode;
    }

    public void Dispose() => OnDispose();
}
