using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class DeveloperWindow : PetWindow
{
    public DeveloperWindow() : base("Dev Window Pet Renamer")
    {
        IsOpen = true;
        Size = new Vector2(700, 500);
    }

    int currentTab = 0;

    int maxTabs = 5;

    public override void OnDraw()
    {
        for(int i = 0; i < maxTabs; i++)
        {
            if (currentTab == i)
            {
                if (ToggleButton(i))
                {
                    currentTab = i;
                }
            }else
            {
                if (ToggleButtonBad(i))
                {
                    currentTab = i;
                }
            }
            SameLineNoMargin();
        }
        ImGui.NewLine();
        ImGui.NewLine();

        if (currentTab == maxTabs)
            currentTab = 0;
        if (currentTab < 0)
            currentTab = maxTabs - 1;

        if (currentTab == 0)
            PluginLink.PettableUserHandler.LoopThroughUsers(DrawUser);
        else if (currentTab == 1) DrawHelpField();
    }

    void DrawHelpField()
    {

    }

    void DrawUser(PettableUser user)
    {
        Label(user.UserName, Styling.ListSmallNameField);
        ImGui.SameLine();
        Label(user.Homeworld.ToString(), Styling.ListIDField);
        ImGui.SameLine();
        if (user.UserExists) Label("O", Styling.SmallButton);
        else Label("X", Styling.SmallButton);

        if (user.HasBattlePet)
        {
            Label(user.BattlePetID.ToString(), Styling.ListIDField);
            ImGui.SameLine();
            Label(user.BattlePetSkeletonID.ToString(), Styling.ListIDField);
            ImGui.SameLine();
            Label(user.BattlePetCustomName.ToString(), Styling.ListSmallNameField);
            ImGui.SameLine();
            Label(user.BaseBattelPetName.ToString(), Styling.ListSmallNameField);
        }

        if (user.HasCompanion)
        {
            Label(user.CompanionID.ToString(), Styling.ListIDField); 
            ImGui.SameLine();
            Label(user.CompanionID.ToString(), Styling.ListIDField);
            ImGui.SameLine();
            Label(user.CustomCompanionName.ToString(), Styling.ListSmallNameField);
            ImGui.SameLine();
            Label(user.CompanionBaseName.ToString(), Styling.ListSmallNameField);
        }

        ImGui.NewLine();
    }
}