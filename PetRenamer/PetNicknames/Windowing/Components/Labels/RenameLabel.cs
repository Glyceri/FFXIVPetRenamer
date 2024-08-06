using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class RenameLabel
{
    public static bool Draw(string label, bool activeSave, ref string value, Vector2 size, string tooltipLabel = "",float labelWidth = 140)
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        float actualWidth = labelWidth * ImGuiHelpers.GlobalScale;
        float height = size.Y;

        TextAligner.Align(TextAlignment.Left);
        BasicLabel.Draw(label, new Vector2(actualWidth, size.Y), tooltipLabel);
        TextAligner.PopAlignment();

        ImGui.SameLine();

        bool shouldActivate = false;

        ImGui.BeginDisabled(activeSave);
        ImGui.PushFont(UiBuilder.IconFont);

        shouldActivate |= ImGui.Button($"{FontAwesomeIcon.Save.ToIconString()}##saveButton_{WindowHandler.InternalCounter}", new Vector2(height, height));

        ImGui.EndDisabled();

        ImGui.SameLine();

        if (ImGui.Button($"{FontAwesomeIcon.Eraser.ToIconString()}##clearButton_{WindowHandler.InternalCounter}", new Vector2(height, height)))
        {
            value = string.Empty;
            shouldActivate |= true;
        }

        ImGui.PopFont();

        ImGui.SameLine();

        shouldActivate |= ImGui.InputTextMultiline($"##RenameBar_{WindowHandler.InternalCounter}", ref value, PluginConstants.ffxivNameSize, size - new Vector2(actualWidth + style.ItemSpacing.X * 3 + height * 2, 0), ImGuiInputTextFlags.CtrlEnterForNewLine | ImGuiInputTextFlags.EnterReturnsTrue);

        return shouldActivate;
    }
}
