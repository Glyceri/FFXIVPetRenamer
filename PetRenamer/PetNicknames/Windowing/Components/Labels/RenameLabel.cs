using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.TranslatorSystem;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class RenameLabel
{
    public static bool Draw(string label, bool activeSave, ref string value, ref Vector3? edgeColour, ref Vector3? textColour, Vector2 size, string tooltipLabel = "", float labelWidth = 140)
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

        if (EraserButton.Draw(new Vector2(height, height), Translator.GetLine("ClearButton.Label"), Translator.GetLine("PetRenameNode.Clear")))
        {
            value = string.Empty;
            shouldActivate |= true;
        }
        ImGui.SameLine();

        bool valueNullOrWhitespace = value.IsNullOrWhitespace();

        if (valueNullOrWhitespace)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
            ImGui.PushStyleColor(ImGuiCol.Border, 0xFF404040);
        }

        shouldActivate |= ImGui.InputTextMultiline($"##RenameBar_{WindowHandler.InternalCounter}", ref value, PluginConstants.ffxivNameSize, size - new Vector2(actualWidth + style.ItemSpacing.X * 5 + height * 4, 0), ImGuiInputTextFlags.CtrlEnterForNewLine | ImGuiInputTextFlags.EnterReturnsTrue);

        if (valueNullOrWhitespace)
        {
            ImGui.PopStyleColor(1);
            ImGui.PopStyleVar(1);
        }

        ImGui.SameLine();

        shouldActivate |= ColourPicker.Draw($"##EdgeColourPicker_{WindowHandler.InternalCounter}", "Edge Colour", ref edgeColour, new Vector2(height, height));

        ImGui.SameLine();

        shouldActivate |= ColourPicker.Draw($"##TextColourPicker_{WindowHandler.InternalCounter}", "Text Colour", ref textColour, new Vector2(height, height));

        return shouldActivate;
    }
}
