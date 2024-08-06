using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class KofiWindow : PetWindow
{
    protected override Vector2 MinSize { get; }
    protected override Vector2 MaxSize { get; }
    protected override Vector2 DefaultSize { get; }
    protected override bool HasModeToggle { get; }

    public KofiWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "Kofi-Window", ImGuiWindowFlags.None)
    {

    }
}
