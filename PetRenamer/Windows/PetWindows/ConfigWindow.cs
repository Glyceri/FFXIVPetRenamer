using System.Numerics;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[ConfigPetWindow]
[PersistentPetWindow]
[ModeTogglePetWindow]
public class ConfigWindow : PetWindow
{
    public ConfigWindow() : base(
        "Pet Nicknames Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse)
    {
        Size = new Vector2(300, 408);
        SizeCondition = ImGuiCond.Always;
    }

    public override void OnDraw()
    {
        if (Checkbox("Display Custom Names", ref PluginLink.Configuration.displayCustomNames) ||
            Checkbox("Use Custom Theme", ref PluginLink.Configuration.useCustomTheme))
            PluginLink.Configuration.Save();

        NewLine();
    }

    public override void OnDrawNormal()
    {
        BeginListBox("##MinionConfig", new Vector2(286, 194));
        OverrideLabel("Minion Specific Settings", new Vector2(278, 25));
        if (Checkbox("Allow Context Menus", ref PluginLink.Configuration.useContextMenuOnMinions) ||
            Checkbox("Allow Tooltips", ref PluginLink.Configuration.allowTooltipsOnMinions) ||
            Checkbox("Replace Emotes", ref PluginLink.Configuration.replaceEmotesOnMinions))
            PluginLink.Configuration.Save();

        ImGui.EndListBox();
    }

    public override void OnDrawBattlePet()
    {
        BeginListBox("##BattleConfig", new Vector2(286, 194));
        OverrideLabel("Battle Pet Specific Settings", new Vector2(278, 25));

        if (Checkbox("Allow Context Menus", ref PluginLink.Configuration.useContextMenuOnBattlePets) ||
            Checkbox("Allow Tooltips", ref PluginLink.Configuration.allowTooltipsBattlePets) ||
            Checkbox("Replace Emotes", ref PluginLink.Configuration.replaceEmotesBattlePets) ||
            Checkbox("Battle Chat", ref PluginLink.Configuration.useCustomPetNamesInBattleChat) ||
            Checkbox("Replace Flyout Text", ref PluginLink.Configuration.useCustomFlyoutPet) ||
            Checkbox("Allow Cast Bar", ref PluginLink.Configuration.allowCastBarPet))
            PluginLink.Configuration.Save();

        ImGui.EndListBox();
    }

    public override void OnDrawSharing()
    {
        BeginListBox("##SharingConfig", new Vector2(286, 194));
        OverrideLabel("Sharing Mode Specific Settings", new Vector2(278, 25));

        ImGui.EndListBox();
    }

    public override void OnLateDraw()
    {
        NewLine();
        if (Button("Clear All Nicknames"))
            PluginLink.WindowHandler.AddTemporaryWindow<ConfirmPopup>(
                "Are you sure you want to clear all Nicknames\nfor every user?",
                (outcome) => { if ((bool)outcome) { PluginLink.Configuration.ClearAllNicknames(); } }
                , this);

        if (Button("Credits"))
            PluginLink.WindowHandler.OpenWindow<CreditsWindow>();
    }
}
