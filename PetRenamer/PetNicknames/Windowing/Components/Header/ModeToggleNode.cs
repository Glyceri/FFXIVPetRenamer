using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class ModeToggleNode
{
    public const float BUTTON_WEAKENER = 0.75f;
    
    private static Vector2 ButtonSize
        => new Vector2(50 * WindowHandler.GlobalScale, ImGui.GetContentRegionAvail().Y * BUTTON_WEAKENER);
    
    public static void Draw(IPetWindow window, ModeToggleRegistration modeToggleRegistration)
    { 
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, modeToggleRegistration.HoverColour);
        ImGui.PushStyleColor(ImGuiCol.Button,        modeToggleRegistration.IdleColour);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive,  modeToggleRegistration.ClickColour);
        
        ImGui.BeginDisabled(window.PetMode == modeToggleRegistration.PetMode);
        
        bool clicked = ImGui.Button($"##ModeToggle_{WindowHandler.InternalCounter}", ButtonSize);
        
        ImGui.EndDisabled();
        
        ImGui.PopStyleColor(3);
        
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.SetTooltip(CreateStringFromMode(modeToggleRegistration.PetMode));
        }
        
        ImGui.SameLine(0, 0);
        
        if (!clicked)
        {
            return;
        }
        
        window.SetPetMode(modeToggleRegistration.PetMode);
    }
    
    public static void DrawInvis(IPetWindow window)
    {
        Vector2 newSize = new Vector2(0.0001f, ButtonSize.Y);
        
        _ = ImGui.InvisibleButton($"###{window.ToString()}_INVIS_BUTTON_{WindowHandler.InternalCounter}", newSize);
    }
    
    public static string CreateStringFromMode(SkeletonType mode)
    {
        string petModeLine = Translator.GetLine("PetMode");
        string speciesLine = Translator.GetLine($"PetRenameNode.Species{(int)mode}");
        
        return string.Format(petModeLine, speciesLine);
    }
}
