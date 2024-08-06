using ImGuiNET;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class CenteredLabel
{
    public static unsafe void Draw(string label, Vector2 size, string tooltip = "")
    {
        Vector2 contentSize = ImGui.GetContentRegionAvail();
        Vector2 halfSize = contentSize * 0.5f;
        Vector2 halfSizeSize = size * 0.5f;
        Vector2 adder = halfSize - halfSizeSize;

        ImGui.SetCursorPos(ImGui.GetCursorPos() + adder);

        BasicLabel.Draw(label, size, tooltip);
    }
}
