using System.Numerics;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[ConfigPetWindow]
[PersistentPetWindow]
[ModeTogglePetWindow]
public class ConfigWindow : PetWindow
{
    Vector2 baseSize = new Vector2(300, 435);
    bool unsupportedMode = false;

    public ConfigWindow() : base(
        "Pet Nicknames Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse)
    {
        Size = baseSize;
        SizeCondition = ImGuiCond.Always;
    }

    bool lastDownloadAll = false;

    public override void OnDraw()
    {
        if (AnyIllegalsGoingOn())
        {
            Size = baseSize + new Vector2(0, 50);
        }
        if (unsupportedMode)
        {
            OverrideLabel("Only INSTALLED plugins will show up!", new Vector2(286, 24));
            BeginListBox("##SharingConfig", new Vector2(286, 102));
            TextColoured(new Vector4(1, 0.6f, 0.6f, 1), "Do NOT send feedback or issues for these\nsettings on discord or via the official \n[Send Feedback] button!\nONLY Github Issues are ALLOWED!");
            if (Checkbox("Understood!", ref PluginLink.Configuration.understoodWarningThirdPartySettings))
                PluginLink.Configuration.Save();
            ImGui.EndListBox();
            if (Button("Back to Normal Settings", new Vector2(286, 24)))
                unsupportedMode = false;

            NewLine();
        }
        else
        {
            if (Checkbox("Display Custom Names", ref PluginLink.Configuration.displayCustomNames) ||
                Checkbox("Use Custom Theme", ref PluginLink.Configuration.useCustomTheme))
                PluginLink.Configuration.Save();

            if(Checkbox("Allow Automatic Profile Pictures", ref PluginLink.Configuration.downloadProfilePictures))
                PluginLink.Configuration.Save();
            SetTooltipHovered("This will automatically download profile pictures from Lodestone");

            if (lastDownloadAll != PluginLink.Configuration.downloadProfilePictures)
            {
                lastDownloadAll = PluginLink.Configuration.downloadProfilePictures;
                if (PluginLink.Configuration.downloadProfilePictures)
                    for(int i = 0; i < PluginLink.PettableUserHandler.Users.Count; i++)
                        PluginLink.PettableUserHandler.OnDeclare(PluginLink.PettableUserHandler.Users[i], Core.PettableUserSystem.Enums.UserDeclareType.Add, true);
            }

            NewLine();
        }
    }

    public override void OnDrawNormal()
    {
        if (unsupportedMode)
        {
            if (!PluginLink.Configuration.understoodWarningThirdPartySettings) return;
            BeginListBox("##MinionConfig", new Vector2(286, 194));
            OverrideLabel("Minion Specific Settings", new Vector2(278, 25));
            if (Checkbox("Redraw On Change [Penumbra]", ref PluginLink.Configuration.redrawMinionOnSpawn))
                PluginLink.Configuration.Save();
            SetTooltipHovered("Redraws your Minion upon changing or spawning\nThis fixes bugs like names lingering on, or appearing too late.");
            ImGui.EndListBox();
            return;
        }
        
        BeginListBox("##MinionConfig", new Vector2(286, 194));
        OverrideLabel("Minion Specific Settings", new Vector2(278, 25));
        if (Checkbox("Allow Context Menus", ref PluginLink.Configuration.useContextMenuOnMinions) ||
            Checkbox("Allow Tooltips", ref PluginLink.Configuration.allowTooltipsOnMinions) ||
            Checkbox("Replace Emotes", ref PluginLink.Configuration.replaceEmotesOnMinions) ||
            Checkbox("Show names in Minion Notebook", ref PluginLink.Configuration.showNamesInMinionBook))
            PluginLink.Configuration.Save();

        ImGui.EndListBox();
    }

    public override void OnDrawBattlePet()
    {
        if (unsupportedMode )
        {
            if (!PluginLink.Configuration.understoodWarningThirdPartySettings) return;
            if (PenumbraIPCProvider.PenumbraEnabled())
            {
                BeginListBox("##BattleConfig", new Vector2(286, 194));
                OverrideLabel("Battle Pet Specific Settings", new Vector2(278, 25));
                if (Checkbox("Redraw On Change [Penumbra]", ref PluginLink.Configuration.redrawBattlePetOnSpawn))
                    PluginLink.Configuration.Save();
                SetTooltipHovered("Redraws the Battle Pet upon changing or spawning\nThis fixes bugs like names lingering on, or appearing too late.");
                ImGui.EndListBox();
            }
            return;
        }
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

    public bool AnyIllegalsGoingOn()
    {
        // Comment out this line to unsupport all the Third Party support
        if (PenumbraIPCProvider.PenumbraEnabled()) return true;

        unsupportedMode = false;
        return false;
    }

    public override void OnDrawSharing()
    {
        if (unsupportedMode)
        {
            if (!PluginLink.Configuration.understoodWarningThirdPartySettings) return;
            BeginListBox("##SharingConfig2", new Vector2(286, 194));
            OverrideLabel("Sharing Mode Specific Settings", new Vector2(278, 25));

            ImGui.EndListBox();

            return;
        }

        BeginListBox("##SharingConfig3", new Vector2(286, 194));
        OverrideLabel("Sharing Mode Specific Settings", new Vector2(278, 25));

        if (Checkbox("Always open Advanced Mode [Export]", ref PluginLink.Configuration.alwaysOpenAdvancedMode))
            PluginLink.Configuration.Save();

        ImGui.EndListBox();
    }


    public override void OnWindowOpen() =>  unsupportedMode = false;
    public override void OnWindowClose() => unsupportedMode = false;
    

    public override void OnLateDraw()
    {
        NewLine();

        if (!unsupportedMode)
        {
            if (Button("Clear All Nicknames", new Vector2(286, 24)))
                PluginLink.WindowHandler.AddTemporaryWindow<ConfirmPopup>(
                    "Are you sure you want to clear all Nicknames\nfor every user?",
                    (outcome) => { if ((bool)outcome) { PluginLink.Configuration.ClearAllNicknames(); } }
                    , this);

            if (Button("Credits", new Vector2(286, 24)))
                PluginLink.WindowHandler.OpenWindow<CreditsWindow>();

            if (AnyIllegalsGoingOn())
            {
                NewLine();
                if (XButton("Other Plugin Settings", new Vector2(286, 24)))
                    unsupportedMode = true;
            }
        }
    }
}
