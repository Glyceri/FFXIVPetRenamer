using System.Numerics;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[ConfigPetWindow]
[PersistentPetWindow]
public class ConfigWindow : PetWindow
{
    public ConfigWindow() : base(
        "Global minionname Settings",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoFocusOnAppearing)
    {
        Size = new Vector2(232, 195);
        SizeCondition = ImGuiCond.Always;
    }

    public override void OnDraw()
    {
        if (Checkbox("Display Custom Names", ref PluginLink.Configuration.displayCustomNames) || Checkbox("Use Custom Theme", ref PluginLink.Configuration.useCustomTheme) || Checkbox("Allow Context Menus", ref PluginLink.Configuration.useContextMenus) || Checkbox("Use Party List (FLICKERS)", ref PluginLink.Configuration.usePartyList))
            PluginLink.Configuration.Save();


        if (Button("Clear All Nicknames")) 
            PluginLink.WindowHandler.AddTemporaryWindow<ConfirmPopup>(
                "Are you sure you want to clear all Nicknames\nfor every user?",
                (outcome) => { if ((bool)outcome) { PluginLink.Configuration.ClearNicknamesForAllUsersV2(); } }
                , this);

        if (Button("Credits"))
            PluginLink.WindowHandler.OpenWindow<CreditsWindow>();
    }
}
