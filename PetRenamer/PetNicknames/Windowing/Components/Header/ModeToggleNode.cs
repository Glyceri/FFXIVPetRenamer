using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class ModeToggleNode
{
    public static Vector2 ButtonSize
        => new Vector2(50, 15) * WindowHandler.GlobalScale;
    
    public static void Draw(IPetWindow window, SkeletonType windowSkeletonType, SkeletonType drawForType, Vector4 hoverColour, Vector4 idleColour, Vector4 clickColour)
    { 
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hoverColour);
        ImGui.PushStyleColor(ImGuiCol.Button,        idleColour);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive,  clickColour);
        
        ImGui.BeginDisabled(windowSkeletonType == drawForType);
        
        bool clicked = ImGui.Button($"##ModeToggle_{WindowHandler.InternalCounter}", ButtonSize);
        
        ImGui.EndDisabled();
        
        ImGui.PopStyleColor(3);
        
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
        {
            ImGui.SetTooltip(CreateStringFromMode(drawForType));
        }
        
        ImGui.SameLine();
        
        if (!clicked)
        {
            return;
        }
        
        window.SetPetMode(drawForType);
    }
    
    public static string CreateStringFromMode(SkeletonType mode)
    {
        string petModeLine = Translator.GetLine("PetMode");
        string speciesLine = Translator.GetLine($"PetRenameNode.Species{(int)mode}");
        
        return string.Format(petModeLine, speciesLine);
    }
}
