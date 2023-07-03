using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Windows;

public class ConfigWindow : PetWindow
{

    public ConfigWindow() : base(
        "Global petname Settings",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(232, 150);
        SizeCondition = ImGuiCond.Always;
    }

    public override void Draw()
    {
        if (ImGui.Checkbox("Display Custom Names", ref PluginLink.Configuration.displayCustomNames))
        {
            Globals.RedrawPet = true;
            PluginLink.Configuration.Save();
        }
        
        /*if(ImGui.Button("Clear All Nicknames"))
            new ConfirmPopup("Are you sure you want to clear all Nicknames?", 
                (outcome) => { if (outcome) { PluginLink.Configuration.ClearNicknames(); } }
                , this);*/

        if (ImGui.Button("Credits"))
            PluginLink.WindowHandler.GetWindow<CreditsWindow>().IsOpen = true;
    }
}
