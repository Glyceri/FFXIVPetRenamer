using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Theming;

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
            SetTheme();
        }
    }

    internal static void SetTheme()
    {
        if (PluginLink.Configuration.newUseCustomTheme)
        {
            if (_petMode == PetMode.Normal) ThemeHandler.SetTheme(PluginLink.Configuration.CustomBaseTheme);
            else if (_petMode == PetMode.BattlePet) ThemeHandler.SetTheme(PluginLink.Configuration.CustomGreenTheme);
            else if (_petMode == PetMode.ShareMode) ThemeHandler.SetTheme(PluginLink.Configuration.CustomRedTheme);
        }
        else
        {
            if (_petMode == PetMode.Normal) ThemeHandler.SetTheme(ThemeHandler.baseTheme);
            else if (_petMode == PetMode.BattlePet) ThemeHandler.SetTheme(ThemeHandler.greenTheme);
            else if (_petMode == PetMode.ShareMode) ThemeHandler.SetTheme(ThemeHandler.redTheme);
        }
    }

    internal void SetPetMode(PetMode mode) 
    {
        petMode = mode;
        TickPetModeChanged();
    }
    internal virtual void OnPetModeChange(PetMode mode) { }
    protected PetWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    public sealed override unsafe void Draw()
    {
        base.Draw();
        if (drawToggle) DrawModeToggle();
        TickPetModeChanged();
        OnDraw();
        if (petMode == PetMode.Normal) OnDrawNormal();
        else if (petMode == PetMode.BattlePet) OnDrawBattlePet();
        else OnDrawSharing();
        OnLateDraw();

        _PopAllStyleColours();
    }

    void TickPetModeChanged()
    {
        if (_lastMode != petMode)
        {
            OnPetModeChange(petMode);
            _lastMode = petMode;
        }
    }

    public sealed override void PostDraw()
    {
        PostDrawHelper();
        _PopAllStyleColours();
    }
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