using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.TranslatorSystem;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class ColourPicker
{
    static Vector3? colourHolder = Vector3.One;
    static bool popupIsOpen = false;

    public static bool Draw(string ID, string tooltip, ref Vector3? colour, Vector2 size)
    {
        bool edited = false;

        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
        ImGui.PushStyleColor(ImGuiCol.Border, 0xFF404040);

        bool shouldOpen = ImGui.ColorButton(ID, new Vector4(colour ?? Vector3.One, 1), ImGuiColorEditFlags.NoTooltip, size);

        ImGui.PopStyleColor(1);
        ImGui.PopStyleVar(1);

        if (colour == null)
        {
            var dl = ImGui.GetWindowDrawList();
            dl.AddLine(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), 0xFF0000FF, 3f * ImGuiHelpers.GlobalScale);
            dl.AddLine(ImGui.GetItemRectMin() + new Vector2(ImGui.GetItemRectSize().X, 0), ImGui.GetItemRectMin() + new Vector2(0, ImGui.GetItemRectSize().Y), 0xFF0000FF, 3f * ImGuiHelpers.GlobalScale);
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(tooltip);
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        if (shouldOpen)
        {
            colourHolder = colour;
            ImGui.CloseCurrentPopup();
            ImGui.OpenPopup(ID, ImGuiPopupFlags.MouseButtonLeft);
        }

        if (ImGui.BeginPopup(ID))
        {
            Vector3 temporaryColour = colourHolder ?? Vector3.One;

            if (EraserButton.Draw(size, Translator.GetLine("ClearButton.ColourLabel"), "Clear Colour"))
            {
                colour = null;
                edited = true;
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();

            ImGui.BeginDisabled(colourHolder == colour);

            ImGui.PushFont(UiBuilder.IconFont);

            if (ImGui.Button($"{FontAwesomeIcon.Save.ToIconString()}##saveButton_{WindowHandler.InternalCounter}", size))
            {
                edited = true;
                colour = colourHolder;
                ImGui.CloseCurrentPopup();
            }

            ImGui.PopFont();

            ImGui.EndDisabled();

            bool colourPicked = ImGui.ColorPicker3($"##ColorPick", ref temporaryColour, ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoSmallPreview);

            if (colourPicked)
            {
                colourHolder = temporaryColour;
            }

            ImGui.EndPopup();
        }

        return edited;
    }
}
