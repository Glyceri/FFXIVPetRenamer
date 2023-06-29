using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace PetRenamer.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    private PetRenamerPlugin plugin;

    public ConfigWindow(PetRenamerPlugin plugin) : base(
        "Global petname Settings",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;   
        this.Size = new Vector2(232, 75);
        this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Checkbox("Display Custom Names", ref Configuration.displayCustomNames);
        if(ImGui.Button("Clear All Names"))
        {
            new ConfirmPopup("Are you sure you want to clear all Nicknames?", (outcome) => { if (outcome) { Configuration.ClearNicknames(); } }, plugin, this);
        }
    }
}
