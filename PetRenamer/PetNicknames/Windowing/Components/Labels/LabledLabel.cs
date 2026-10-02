using Dalamud.Utility;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class LabledLabel
{
    public static void Draw(string label, string value, Vector2 size, string tooltipLabel = "", string tooltipValue = "", float labelWidth = 140)
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        float actualWidth = labelWidth * ImGuiHelpers.GlobalScale;

        TextAligner.Align(TextAlignment.Left);
        BasicLabel.Draw(label + $"###PET_NICKNAMES_LABEL_{WindowHandler.InternalCounter}", new Vector2(actualWidth, size.Y), tooltipLabel);
        TextAligner.PopAlignment();

        ImGui.SameLine();

        TextAligner.Align(TextAlignment.Right);
        BasicLabel.Draw(value  + $"###PET_NICKNAMES__VALUE_LABEL_{WindowHandler.InternalCounter}", size - new Vector2(actualWidth + style.ItemSpacing.X, 0), tooltipValue);
        TextAligner.PopAlignment();
    }

    public static bool DrawButton(string label, string value, Vector2 size, string tooltipLabel = "", string tooltipValue = "", float labelWidth = 140)
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        float actualWidth = labelWidth * ImGuiHelpers.GlobalScale;

        TextAligner.Align(TextAlignment.Left);
        BasicLabel.Draw(label, new Vector2(actualWidth, size.Y), tooltipLabel);
        TextAligner.PopAlignment();

        ImGui.SameLine();

        TextAligner.Align(TextAlignment.Right);
        
        bool returner = ImGui.Button(value + $"###PET_NICKNAMES_LABEL_BUTTON_{WindowHandler.InternalCounter}", size - new Vector2(actualWidth + style.ItemSpacing.X, 0));
        
        if (!tooltipValue.IsNullOrWhitespace())
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(tooltipValue);
            }
        }
        
        TextAligner.PopAlignment();

        return returner;
    }
}
