using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class ModeToggle
{
    public static void Draw(IPetWindow petWindow, IPetServices petServices)
    {
        Vector2       lastCursorPos = ImGui.GetCursorPos();
        float         height        = ImGui.GetContentRegionAvail().Y;
        float         cutHeight     = height * ModeToggleNode.BUTTON_WEAKENER;
        ImGuiStylePtr style         = ImGui.GetStyle();
        float         mappedHeight  = (height - cutHeight - style.FramePadding.Y) * 0.5f;
        
        lastCursorPos              += new Vector2(0, mappedHeight);
        
        ImGui.SetCursorPos(ImGui.GetCursorPos() + lastCursorPos);

        float         oldY          = ImGui.GetCursorPos().Y - (mappedHeight * 0.5f);
        
        if (petWindow.HasModeToggle)
        {
            ModeToggleNode.Draw(petWindow, PluginConstants.MinionModeToggle);
            ModeToggleNode.Draw(petWindow, PluginConstants.BattleModeToggle);
            ModeToggleNode.Draw(petWindow, PluginConstants.BeastMasterModeToggle);
        }
        else
        {
            ModeToggleNode.DrawInvis(petWindow);
        }
        
        ImGui.SameLine();
        
        if (petWindow.HasModeToggle)
        {
            ImGui.SetCursorPosY(oldY);
        }
        
        ImGui.Text(petServices.Version);
        
        ImGui.SetCursorPos(ImGui.GetCursorPos() - lastCursorPos);
    }
}