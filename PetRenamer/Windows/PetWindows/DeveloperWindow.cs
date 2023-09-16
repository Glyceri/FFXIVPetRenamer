using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.PettableUserSystem.Enums;
using CSCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;
using CSGameObjectManager = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

//[PersistentPetWindow]
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
                if (ToggleButton()) currentTab = i;
            }
            else
            {
                if (ToggleButtonBad()) currentTab = i;

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
                        (ushort)((PlayerCharacter)target).HomeWorld.Id), UserDeclareType.Add);
            }

        if (Button("Add ALL Users EMPTY"))
        {
            AddUser(false, 0);
        }

        if (Button("Add ALL Users"))
        {
            AddUser(true, 1000);
        }

        if (Button("Add ALL Users Small"))
        {
            AddUser(true, 0);
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

    unsafe void AddUser(bool addStuff, int count)
    {
        for (int i = 2; i < 200; i += 2)
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

            for(int f = 0; f < count; f++)
            {
                ids.Add(f);
                names.Add("[TESTNAME]");
            }

            if (!addStuff)
            {
                ids.Clear();
                names.Clear();
            }

            PluginLink.PettableUserHandler.DeclareUser(
                new SerializableUserV3(
                    ids.ToArray(),
                    names.ToArray(),
                    gObj.Name.ToString(),
                    (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id), UserDeclareType.Add);
        }
    }

    int totalCount = 0;
    int playerCount = 0;

    void DrawUsers()
    {
        SetSize(baseSize);
        NewLabel("Totalcount: " + totalCount.ToString(), Styling.ListSmallNameField);
        SameLine();
        NewLabel("Active player count: " + playerCount.ToString(), Styling.ListSmallNameField);
        NewLine();
        totalCount = 0;
        playerCount = 0;
        PluginLink.PettableUserHandler.LoopThroughUsers(DrawUser);
    }

    void DrawUser(PettableUser user)
    {
        NewLabel(user.UserName, Styling.ListSmallNameField);
        SameLine();
        NewLabel(user.Homeworld.ToString(), Styling.ListIDField);
        SameLine();
        if (user.UserExists) { NewLabel("O", Styling.SmallButton); playerCount++; }
        else NewLabel("X", Styling.SmallButton);

        SameLine();
        NewLabel(user.SerializableUser.length.ToString(), Styling.ListIDField);
        totalCount += user.SerializableUser.length;
        if (user.HasBattlePet)
        {
            NewLabel(user.BattlePetID.ToString(), Styling.ListIDField);
            SameLine();
            NewLabel(user.BattlePetSkeletonID.ToString(), Styling.ListIDField);
            SameLine();
            NewLabel(user.BattlePetCustomName.ToString(), Styling.ListSmallNameField);
            SameLine();
            NewLabel(user.BaseBattlePetName.ToString(), Styling.ListSmallNameField);
        }

        if (user.HasCompanion)
        {
            NewLabel(user.CompanionID.ToString(), Styling.ListIDField);
            SameLine();
            NewLabel(user.CompanionID.ToString(), Styling.ListIDField);
            SameLine();
            NewLabel(user.CustomCompanionName.ToString(), Styling.ListSmallNameField);
            SameLine();
            NewLabel(user.CompanionBaseName.ToString(), Styling.ListSmallNameField);
        }

        NewLine();
    }
}