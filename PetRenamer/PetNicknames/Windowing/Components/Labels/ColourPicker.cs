using Dalamud.Interface;
using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.TranslatorSystem;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Labels;

internal static class ColourPicker
{
    static Vector3? colourHolder = Vector3.One;

    public static bool Draw(string id, string tooltip, ref Vector3? colour, Vector2 size)
    {
        bool edited = false;

        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
        ImGui.PushStyleColor(ImGuiCol.Border, 0xFF404040);

        bool shouldOpen = ImGui.ColorButton(id, new Vector4(colour ?? Vector3.One, 1), ImGuiColorEditFlags.NoTooltip, size);

        ImGui.PopStyleColor(1);
        ImGui.PopStyleVar(1);

        if (colour == null)
        {
            ImDrawListPtr dl = ImGui.GetWindowDrawList();
            
            dl.AddLine(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), 0xFF0000FF, 3f * WindowHandler.GlobalScale);
            dl.AddLine(ImGui.GetItemRectMin() + new Vector2(ImGui.GetItemRectSize().X, 0), ImGui.GetItemRectMin() + new Vector2(0, ImGui.GetItemRectSize().Y), 0xFF0000FF, 3f * WindowHandler.GlobalScale);
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
            
            ImGui.OpenPopup(id);
        }

        if (ImGui.BeginPopup(id))
        {
            Vector3 temporaryColour = colourHolder ?? Vector3.One;

            if (EraserButton.Draw(size, Translator.GetLine("ClearButton.ColourLabel"), Translator.GetLine("ColourPicker.ClearColour")))
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
