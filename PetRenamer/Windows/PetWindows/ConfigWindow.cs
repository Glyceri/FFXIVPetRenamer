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
        Size = new Vector2(532, 275);
        SizeCondition = ImGuiCond.Always;
    }

    public override void OnDraw()
    {
        if (Checkbox("Display Custom Names", ref PluginLink.Configuration.displayCustomNames) || Checkbox("Use Custom Theme", ref PluginLink.Configuration.useCustomTheme) || Checkbox("Use New Hook Naming System", ref PluginLink.Configuration.useNewNameSystem))
            PluginLink.Configuration.Save();

        ImGui.TextColored(StylingColours.errorText, 
            "Only disable Hook Naming if you understand the following sentence:\n" +
            "Without hook naming the name gets applied to the pointer, causing\n" +
            "potentially A LOT of issues. It does update the name on focus and \nparty list however.\n" +
            "(Soon to be removed entirely)");

        ImGui.NewLine();

        if (Button("Clear All Nicknames")) 
            PluginLink.WindowHandler.AddTemporaryWindow<ConfirmPopup>(
                "Are you sure you want to clear all Nicknames\nfor every user?",
                (outcome) => { if ((bool)outcome) { PluginLink.Configuration.ClearNicknamesForAllUsersV2(); } }
                , this);

        if (Button("Credits"))
            PluginLink.WindowHandler.OpenWindow<CreditsWindow>();
    }
}
