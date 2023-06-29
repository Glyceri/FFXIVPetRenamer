using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PetRenamer.Core;

namespace PetRenamer.Windows;

public class ConfigWindow : Window
{
    private Configuration configuration;
    private PetRenamerPlugin plugin;

    public ConfigWindow(PetRenamerPlugin plugin) : base(
        "Global petname Settings",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;   
        Size = new Vector2(232, 150);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
    }

    public override void Draw()
    {
        if (ImGui.Checkbox("Display Custom Names", ref configuration.displayCustomNames))
        {
            Globals.RedrawPet = true;
            configuration.Save();
        }
        
        if(ImGui.Button("Clear All Nicknames"))
            new ConfirmPopup("Are you sure you want to clear all Nicknames?", 
                (outcome) => { if (outcome) { configuration.ClearNicknames(); } }
                , plugin, this);

        if (ImGui.Button("Credits"))
            plugin.CreditsWindow.IsOpen = true;
    }
}
