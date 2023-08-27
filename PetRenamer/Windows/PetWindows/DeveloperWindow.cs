using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.PettableUserSystem.Enums;
using CSGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;
using CSCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;
using CSGameObjectManager = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class DeveloperWindow : PetWindow
{
    readonly Vector2 baseSize = new Vector2(700, 500);

    Vector2 minSize => GetMinSize();

    public DeveloperWindow() : base("Dev Window Pet Renamer")
    {
        IsOpen = true;
        Size = baseSize;
    }

    int currentTab = 0;

    int maxTabs = 5;

    public override void OnDraw()
    {
        for (int i = 0; i < maxTabs; i++)
        {
            if (currentTab == i)
            {
                if (ToggleButton(i + 100))
                {
                    currentTab = i;
                }
            }
            else
            {
                if (ToggleButtonBad(i + 100))
                {
                    currentTab = i;
                }
            }
            SameLineNoMargin();
        }
        ImGui.NewLine();

        if (currentTab == maxTabs)
            currentTab = 0;
        if (currentTab < 0)
            currentTab = maxTabs - 1;

        if (currentTab == 0) DrawUsers();

        else if (currentTab == 1) DrawHelpField();
        else if (currentTab == 2) ConfigWindow();
        else if (currentTab == 3) PetNameWindow();
    }

    Vector2 GetMinSize()
    {
        Vector2 size = Styling.ToggleButton;
        size.X *= maxTabs;
        size.X += 10;
        return size;
    }

    void SetSize(Vector2? ss)
    {
        if (ss == null) return;
        Vector2 sss = ss.Value;
        float x = sss.X;
        if(x < minSize.X)
            x = minSize.X;
        sss.X = x;
        Size = sss; 
    }

    void PetNameWindow()
    {
        SetSize(PluginLink.WindowHandler.GetWindow<PetRenameWindow>()?.Size);
        PluginLink.WindowHandler.GetWindow<PetRenameWindow>()?.Draw();
    }

    void ConfigWindow()
    {
        SetSize(PluginLink.WindowHandler.GetWindow<ConfigWindow>()?.Size);
        PluginLink.WindowHandler.GetWindow<ConfigWindow>()?.Draw();
    }

    unsafe void DrawHelpField()
    {
        SetSize(baseSize);
        GameObject? target = PluginHandlers.TargetManager.Target!;
        if (target != null)
            if (Button("Add Target"))
            {
                int minionTarget = target.ObjectIndex + 1;
                CSCompanion* companion = (CSCompanion*)CSGameObjectManager.GetGameObjectByIndex(minionTarget);
                int minID = -1;
                string minName = string.Empty;
                if(companion != null)
                {
                    minName = "[TESTNAME]";
                    minID = companion->Character.CharacterData.ModelSkeletonId;
                }
                PluginLink.PettableUserHandler.DeclareUser(
                    new SerializableUserV3(
                        new int[2] { -2, minID },
                        new string[2] { "[TESTNAME]", minName },
                        target.Name.ToString(),
                        (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id), UserDeclareType.Add);
            }

        if (Button("Add ALL Users"))
        {
            for(int i = 2; i < 200; i += 2)
            {
                GameObject? gObj = PluginHandlers.ObjectTable[i];
                if (gObj == null) continue;
                int minionTarget = i + 1;
                CSCompanion* companion = (CSCompanion*)CSGameObjectManager.GetGameObjectByIndex(minionTarget);
                int minID = -1;
                string minName = string.Empty;
                if (companion != null)
                {
                    minName = "[TESTNAME]";
                    minID = companion->Character.CharacterData.ModelSkeletonId;
                }
                List<int> ids = new List<int>() { -2, minID };
                List<string> names = new List<string>() { "[TESTNAME]", minName };

                for(int f = 0; f < 1000; f++)
                {
                    ids.Add(f);
                    names.Add("[TESTNAME]");
                }

                PluginLink.PettableUserHandler.DeclareUser(
                    new SerializableUserV3(
                        ids.ToArray(),
                        names.ToArray(),
                        gObj.Name.ToString(),
                        (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id), UserDeclareType.Add);
            }
        }

        if (Button("Remove all custom users"))
        {
            PluginLink.PettableUserHandler.BackwardsSAFELoopThroughUser((user) => 
            {
                user.SerializableUser.LoopThroughBreakable(nickname =>
                {
                    if (nickname.Item2 == "[TESTNAME]")
                    {
                        PluginLink.PettableUserHandler.DeclareUser(user.SerializableUser, UserDeclareType.Remove);
                        return true;
                    }
                    return false;
                });
            });
        }
    }

    int totalCount = 0;
    int playerCount = 0;

    void DrawUsers()
    {
        SetSize(baseSize);
        NewLabel("Totalcount: " + totalCount.ToString(), Styling.ListSmallNameField);
        ImGui.SameLine();
        NewLabel("Active player count: " + playerCount.ToString(), Styling.ListSmallNameField);
        ImGui.NewLine();
        totalCount = 0;
        playerCount = 0;
        PluginLink.PettableUserHandler.LoopThroughUsers(DrawUser);
    }

    void DrawUser(PettableUser user)
    {
        NewLabel(user.UserName, Styling.ListSmallNameField);
        ImGui.SameLine();
        NewLabel(user.Homeworld.ToString(), Styling.ListIDField);
        ImGui.SameLine();
        if (user.UserExists) { NewLabel("O", Styling.SmallButton); playerCount++; }
        else NewLabel("X", Styling.SmallButton);

        ImGui.SameLine();
        NewLabel(user.SerializableUser.length.ToString(), Styling.ListIDField);
        totalCount += user.SerializableUser.length;
        if (user.HasBattlePet)
        {
            NewLabel(user.BattlePetID.ToString(), Styling.ListIDField);
            ImGui.SameLine();
            NewLabel(user.BattlePetSkeletonID.ToString(), Styling.ListIDField);
            ImGui.SameLine();
            NewLabel(user.BattlePetCustomName.ToString(), Styling.ListSmallNameField);
            ImGui.SameLine();
            NewLabel(user.BaseBattelPetName.ToString(), Styling.ListSmallNameField);
        }

        if (user.HasCompanion)
        {
            NewLabel(user.CompanionID.ToString(), Styling.ListIDField);
            ImGui.SameLine();
            NewLabel(user.CompanionID.ToString(), Styling.ListIDField);
            ImGui.SameLine();
            NewLabel(user.CustomCompanionName.ToString(), Styling.ListSmallNameField);
            ImGui.SameLine();
            NewLabel(user.CompanionBaseName.ToString(), Styling.ListSmallNameField);
        }

        ImGui.NewLine();
    }
}