using Dalamud.Interface.Utility;
using ImGuiNET;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class LabledLabel
{
    public static void Draw(string label, string value, Vector2 size, string tooltipLabel = "", string tooltipValue = "", float labelWidth = 140)
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        float actualWidth = labelWidth * ImGuiHelpers.GlobalScale;

        TextAligner.Align(TextAlignment.Left);
        BasicLabel.Draw(label, new Vector2(actualWidth, size.Y), tooltipLabel);
        TextAligner.PopAlignment();

        ImGui.SameLine();

        TextAligner.Align(TextAlignment.Right);
        BasicLabel.Draw(value, size - new Vector2(actualWidth + style.ItemSpacing.X, 0), tooltipValue);
        TextAligner.PopAlignment();
    }
}
