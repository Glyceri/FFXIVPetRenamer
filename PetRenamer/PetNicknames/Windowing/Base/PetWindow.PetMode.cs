using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Enums;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Base;

internal partial class PetWindow
{
    public bool RequestsModeChange { get; private set; }
    public PetWindowMode NewMode { get; private set; }

    ModeToggleNode? activeModeToggleNode = null;

    void PetModeConstructor()
    {
        activeModeToggleNode = new ModeToggleNode(in DalamudServices);
        activeModeToggleNode.OnModeChange += _InternalSetPetWindowMode;
        PrepependNode(TopLeftAnchor, activeModeToggleNode);
    }

    public void DeclareModeChangedSeen()
    {
        RequestsModeChange = false;
    }

    void _InternalSetPetWindowMode(PetWindowMode mode)
    {
        RequestsModeChange = true;
        NewMode = mode;
    }

    public void SetPetMode(PetWindowMode mode)
    {
        activeModeToggleNode?.SetActivePetMode(mode);
        CurrentMode = mode;

        if (HasModeToggle)
        {
            TitlebarNode.Style.BackgroundGradient = GradientColor.Horizontal(new(mode == PetWindowMode.Minion ? "Titlebar.Minion" : "Titlebar.BattlePet"), new("Window.Background"));
        }

        OnPetModeChanged(mode);
    }

    protected virtual void OnPetModeChanged(PetWindowMode mode) { }
}
