using Dalamud.Utility;
using ImGuiNET;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class BasicLabel
{
    public static unsafe void Draw(string label, Vector2 size, string tooltip = "")
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        Vector4* colour = ImGui.GetStyleColorVec4(ImGuiCol.ButtonActive);

        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, *colour);
        ImGui.PushStyleColor(ImGuiCol.Button, *colour);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, *colour);

        ImGui.Button(label, size);

        if (!tooltip.IsNullOrWhitespace())
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(tooltip);
            }
        }

        ImGui.PopStyleColor(3);
    }
}
