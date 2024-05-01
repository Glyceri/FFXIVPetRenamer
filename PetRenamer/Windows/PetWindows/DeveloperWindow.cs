using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Internal;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;
using CSCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;
using CSGameObjectManager = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectManager;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class DeveloperWindow : PetWindow
{

    public DeveloperWindow() : base("Dev Window Pet Renamer")
    {
        if(PluginLink.Configuration.debugMode && PluginLink.Configuration.autoOpenDebug) IsOpen = true;
    }

    int currentTab = 0;

    readonly int maxTabs = 6;

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

        if (currentTab == 0) Users();
        else if (currentTab == 1) DrawHelpField();
        else if (currentTab == 2) DrawUserPictures();
        else if (currentTab == 3) PetNameWindow();
        else if (currentTab == 4) SettingsWindow();
        else if (currentTab == 5) { }
    }

    void DrawUserPictures()
    {
        foreach(var idk in PluginLink.NetworkingHandler.NetworkingCache.textureCache.Keys)
        {
            IDalamudTextureWrap texutre = PluginLink.NetworkingHandler.NetworkingCache.textureCache[idk];
            if (texutre == null) continue;

            ImGui.Image(texutre.ImGuiHandle, new System.Numerics.Vector2(100, 100));
        }
    }

    void PetNameWindow()
    {
        PluginLink.WindowHandler.GetWindow<PetRenameWindow>()?.Draw();
    }

    void SettingsWindow()
    {
        PluginLink.WindowHandler.GetWindow<ConfigWindow>()?.Draw();
    }

    int tableCounter = 0;

    void Users()
    {
        if (PluginHandlers.ClientState.IsPvPExcludingDen) return;
        tableCounter = 0;
        PluginLink.PettableUserHandler.LoopThroughUsers(NewDrawUser);
    }

    void NewDrawUser(PettableUser user)
    {
        if (!ImGui.BeginTable($"##usersTable{tableCounter++}", 5,  ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingMask))
            return;

        ImGui.TableNextRow();

        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"{user.UserName}");

        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted($"{SheetUtils.instance.GetWorldName(user.Homeworld)}");

        ImGui.TableSetColumnIndex(2);
        ImGui.TextUnformatted($"{(user.UserExists ? "O" : "X")}");

        ImGui.TableSetColumnIndex(3);
        ImGui.TextUnformatted($"{user.SerializableUser.length}");

        if (user.BattlePet.Has)
        {
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{user.BattlePet.ID}");

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{user.BattlePet.BaseName}");

            ImGui.TableSetColumnIndex(3);
            ImGui.TextUnformatted($"{user.BattlePet.CustomName}");

            ImGui.TableSetColumnIndex(4);
            ImGui.TextUnformatted($"{user.BattlePet.Index}");

            ImGui.TableNextRow();
        }

        if (user.Minion.Has)
        {
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{user.Minion.ID}");

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{user.Minion.BaseName}");

            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted($"{user.Minion.BaseNamePlural}");

            ImGui.TableSetColumnIndex(3);
            ImGui.TextUnformatted($"{user.Minion.CustomName}");

            ImGui.TableSetColumnIndex(4);
            ImGui.TextUnformatted($"{user.Minion.Index}");
        }

        ImGui.EndTable();
        NewLine();
    }
  
    unsafe void DrawHelpField()
    {
        if (PluginHandlers.ClientState.IsPvPExcludingDen) return;

        GameObject? target = PluginHandlers.TargetManager.Target!;
        if (target != null)
        {
            if (Button("Add Target"))
            {
                int minionTarget = target.ObjectIndex + 1;
                CSCompanion* companion = (CSCompanion*)CSGameObjectManager.GetGameObjectByIndex(minionTarget);
                int minID = -1;
                string minName = string.Empty;
                if (companion != null)
                {
                    minName = "[TESTNAME]";
                    minID = companion->Character.CharacterData.ModelCharaId;
                }
                PluginLink.PettableUserHandler.DeclareUser(
                    new SerializableUserV3(
                        new int[2] { -2, minID },
                        new string[2] { "[TESTNAME]", minName },
                        target.Name.ToString(),
                        (ushort)((PlayerCharacter)target).HomeWorld.Id), UserDeclareType.Add);
            }
        }

        if (Button("Add ALL Users EMPTY"))  AddUser(false, 0);
        if (Button("Add ALL Users"))        AddUser(true, 1000);
        if (Button("Add ALL Users Small"))  AddUser(true, 0);
        

        if (Button("Remove all custom users"))
        {
            for(int i = PluginLink.PettableUserHandler.Users.Count - 1; i >= 0; i--)
            {
                PettableUser user = PluginLink.PettableUserHandler.Users[i];
                SerializableUserV3 serializableUser = user.SerializableUser;
                for (int f = 0; f < user.SerializableUser.length; f++)
                {
                    if (serializableUser[i].Name != "[TESTNAME]") continue;
                    PluginLink.PettableUserHandler.DeclareUser(user.SerializableUser, UserDeclareType.Remove, false, false);
                    break;
                }
            }
            PluginLink.Configuration.Save();
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
                minID = companion->Character.CharacterData.ModelCharaId;
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
                    (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id), UserDeclareType.Add, false, false);
        }
        PluginLink.Configuration.Save();
    }

    int totalCount = 0;
    int playerCount = 0;

    void DrawUsers()
    {
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
        if (user.BattlePet.Has)
        {
            NewLabel(user.BattlePet.ID.ToString(), Styling.ListIDField);
            SameLine();
            NewLabel(user.BattlePet.Index.ToString(), Styling.ListIDField);
            SameLine();
            NewLabel(user.BattlePet.CustomName.ToString(), Styling.ListSmallNameField);
            NewLine();
        }
        
        if (user.Minion.Has)
        {
            NewLabel(user.Minion.ID.ToString(), Styling.ListIDField);
            SameLine();
            NewLabel(user.Minion.Index.ToString(), Styling.ListIDField);
            SameLine();
            NewLabel(user.Minion.CustomName.ToString(), Styling.ListSmallNameField);
            SameLine();
            NewLabel(user.Minion.BaseName.ToString(), Styling.ListSmallNameField);
            SameLine();
            NewLabel(user.BattlePet.BaseNamePlural.ToString(), Styling.ListSmallNameField);
        }

        NewLine();
    }
}