using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetListWindow : PetWindow
{
    protected override Vector2 MinSize { get; }
    protected override Vector2 MaxSize { get; }
    protected override Vector2 DefaultSize { get; }
    protected override bool HasModeToggle { get; }
    protected override bool HasExtraButtons { get; }

    public PetListWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "Pet List Window", ImGuiWindowFlags.None)
    {

    }
}
