using Dalamud.Interface;
using ImGuiNET;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class EraserButton
{
    public static bool Draw(Vector2 size, string tooltipInactive, string tooltipActive)
    {
        bool keyComboNotPressed = !ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || !ImGui.IsKeyDown(ImGuiKey.LeftShift);

        ImGui.BeginDisabled(keyComboNotPressed);
        ImGui.PushFont(UiBuilder.IconFont);

        bool pressed = ImGui.Button($"{FontAwesomeIcon.Eraser.ToIconString()}##clearButton_{WindowHandler.InternalCounter}", size);

        ImGui.PopFont();
        ImGui.EndDisabled();

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.SetTooltip(keyComboNotPressed ? tooltipInactive : tooltipActive);
        }

        return pressed;
    }
}
