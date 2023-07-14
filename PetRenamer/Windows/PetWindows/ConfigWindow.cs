using System.Numerics;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[ConfigPetWindow]
[PersistentPetWindow]
public class ConfigWindow : PetWindow
{
    public ConfigWindow() : base(
        "Global minionname Settings",
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
            PluginLink.Configuration.Save();
        }

        if(ImGui.Button("Clear All Nicknames")) 
            PluginLink.WindowHandler.AddTemporaryWindow<ConfirmPopup>(
                "Are you sure you want to clear all Nicknames?",
                (outcome) => { if ((bool)outcome) { PluginLink.Configuration.ClearNicknames(); } }
                , this);
        

        if (ImGui.Button("Credits"))
            PluginLink.WindowHandler.OpenWindow<CreditsWindow>();
    }
}
