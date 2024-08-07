﻿using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;
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
        PrepependNode(TitlebarNode, activeModeToggleNode);
    }

    public void DeclareModeChangedSeen()
    {
        RequestsModeChange = false;
    }

    protected void _InternalSetPetWindowMode(PetWindowMode mode)
    {
        RequestsModeChange = true;
        NewMode = mode;
    }

    public void SetPetMode(PetWindowMode mode)
    {
        CurrentMode = mode;
        activeModeToggleNode?.SetActivePetMode(mode);

        if (HasModeToggle)
        {
            TitlebarNode.Style.BackgroundGradient = GradientColor.Horizontal(new(mode == PetWindowMode.Minion ? "Titlebar.Minion" : "Titlebar.BattlePet"), new("Window.Background"));
        }

        OnPetModeChanged(mode);
    }

    protected virtual void OnPetModeChanged(PetWindowMode mode) { }
}
