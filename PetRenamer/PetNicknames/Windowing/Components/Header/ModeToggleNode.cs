using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class ModeToggleNode
{
    public static bool Draw(Vector2 size) 
        => ImGui.Button($"##ModeToggle_{WindowHandler.InternalCounter}", size);
}
