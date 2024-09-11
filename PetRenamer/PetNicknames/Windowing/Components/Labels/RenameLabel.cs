using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.TranslatorSystem;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class RenameLabel
{
    public static bool Draw(string label, bool activeSave, ref string value, Vector2 size, string tooltipLabel = "", float labelWidth = 140)
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

        ImGui.PopFont();

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.SetTooltip(Translator.GetLine("PetRenameNode.Save"));
        }

        ImGui.EndDisabled();

        ImGui.SameLine();

        bool keyComboNotPressed = !ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || !ImGui.IsKeyDown(ImGuiKey.LeftShift);

        ImGui.BeginDisabled(keyComboNotPressed);

        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button($"{FontAwesomeIcon.Eraser.ToIconString()}##clearButton_{WindowHandler.InternalCounter}", new Vector2(height, height)))
        {
            value = string.Empty;
            shouldActivate |= true;
        }
        ImGui.PopFont();
        ImGui.EndDisabled();

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            if (keyComboNotPressed)
            {
                ImGui.SetTooltip(Translator.GetLine("ClearButton.Label"));
            }
            else
            {
                ImGui.SetTooltip(Translator.GetLine("PetRenameNode.Clear"));
            }
        }

        ImGui.SameLine();

        bool valueNullOrWhitespace = value.IsNullOrWhitespace();

        if (valueNullOrWhitespace)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
            ImGui.PushStyleColor(ImGuiCol.Border, 0xFF404040);
        }

        shouldActivate |= ImGui.InputTextMultiline($"##RenameBar_{WindowHandler.InternalCounter}", ref value, PluginConstants.ffxivNameSize, size - new Vector2(actualWidth + style.ItemSpacing.X * 3 + height * 2, 0), ImGuiInputTextFlags.CtrlEnterForNewLine | ImGuiInputTextFlags.EnterReturnsTrue);

        if (valueNullOrWhitespace)
        {
            ImGui.PopStyleColor(1);
            ImGui.PopStyleVar(1);
        }

        return shouldActivate;
    }
}
