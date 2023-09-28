using ImGuiNET;
using PetRenamer.Logging;
using PetRenamer.Theming;
using System;

namespace PetRenamer.Windows;

public abstract class PetWindow : PetWindowHelpers
{
    internal PetMode _lastMode = PetMode.ShareMode;
    internal static PetMode _petMode = PetMode.Normal;
    internal static PetMode petMode
    {
        get => _petMode;
        set
        {
            _petMode = value;
            if (petMode == PetMode.Normal) ThemeHandler.SetTheme(ThemeHandler.baseTheme);
            else if (petMode == PetMode.BattlePet) ThemeHandler.SetTheme(ThemeHandler.greenTheme);
            else if (petMode == PetMode.ShareMode) ThemeHandler.SetTheme(ThemeHandler.redTheme);
        }
    }

    internal static void SetPetMode(PetMode mode) => petMode = mode;
    internal virtual void OnPetModeChange(PetMode mode) { }
    protected PetWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    public sealed override unsafe void Draw()
    {
        if (drawToggle) DrawModeToggle();

        if (_lastMode != petMode)
        {
            _lastMode = petMode;
            OnPetModeChange(petMode);
        }
        try
        {
            OnDraw();
        
        if (petMode == PetMode.Normal) OnDrawNormal();
        else if (petMode == PetMode.BattlePet) OnDrawBattlePet();
        else OnDrawSharing();
        OnLateDraw();

        }
        catch (NullReferenceException nExc)
        {
            PetLog.LogError(nExc, nExc.TargetSite?.Name.ToString() ?? string.Empty);
        }
        _PopAllStyleColours();
    }

    public sealed override void PostDraw() => _PopAllStyleColours();
    public sealed override void OnOpen() => OnWindowOpen();
    public sealed override void OnClose() => OnWindowClose();

    public unsafe virtual void OnWindowOpen() { }
    public unsafe virtual void OnWindowClose() { }
    public unsafe virtual void OnDraw() { }
    public unsafe virtual void OnDrawNormal() { }
    public unsafe virtual void OnDrawBattlePet() { }
    public unsafe virtual void OnDrawSharing() { }
    public unsafe virtual void OnLateDraw() { }
}

internal enum PetMode
{
    Normal,
    BattlePet,
    ShareMode,
    COUNT
}