using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Ipc;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Components;
using PetRenamer.PetNicknames.Windowing.Components.Labels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetDevWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;

    protected override Vector2 MinSize { get; } = new Vector2(350, 136);
    protected override Vector2 MaxSize { get; } = new Vector2(2000, 2000);
    protected override Vector2 DefaultSize { get; } = new Vector2(800, 400);
    protected override bool HasModeToggle { get; } = true;

    float BarSize = 30 * ImGuiHelpers.GlobalScale;

    int currentActive = 0;
    List<DevStruct> devStructList = new List<DevStruct>();

    public PetDevWindow(WindowHandler windowHandler, DalamudServices dalamudServices, Configuration configuration, IPettableUserList userList, IPettableDatabase database) : base(windowHandler, dalamudServices, configuration, "Pet Dev Window", ImGuiWindowFlags.None)
    {
        UserList = userList;
        Database = database;

        if (configuration.debugModeActive && configuration.openDebugWindowOnStart)
        {
            Open();
        }

        devStructList.Add(new DevStruct("User List", DrawUserList));
        devStructList.Add(new DevStruct("IPC Tester", DrawIPCTester, OnIPCUpdate));
        devStructList.Add(new DevStruct("Database", DrawDatabase));
    }

    public override void OnOpen()
    {
        if (devStructList.Count <= currentActive) return;

        devStructList[currentActive].requestUpdate?.Invoke(true);
    }

    public override void OnClose()
    {
        for (int i = 0; i < devStructList.Count; i++)
        {
            devStructList[i].requestUpdate?.Invoke(false);
        }
    }

    protected override void OnDraw()
    {
        if (devStructList.Count == 0) return;

        ImGui.BeginTabBar("##DevTabBar");

        for (int i = 0; i < devStructList.Count; i++)
        {
            if (!ImGui.TabItemButton(devStructList[i].title)) continue;
            int lastActive = currentActive;
            if (lastActive == i) continue;
            currentActive = i;
            devStructList[lastActive].requestUpdate?.Invoke(false);
            devStructList[currentActive].requestUpdate?.Invoke(true);
        }

        devStructList[currentActive].onSelected?.Invoke();

        ImGui.EndTabBar();
    }

    void DrawDatabase()
    {
        IPettableDatabaseEntry[] entries = Database.DatabaseEntries;

        foreach (IPettableDatabaseEntry entry in entries)
        {
            DrawDatabaseUser(entry);
        }
    }

    void DrawDatabaseUser(IPettableDatabaseEntry user)
    {
        if (!ImGui.BeginTable($"##usersTable{WindowHandler.InternalCounter}", 5, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingMask))
            return;

        ImGui.TableNextRow();

        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted(user.IsActive ? "O" : "X");

        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted($"{user.Name}");

        ImGui.TableSetColumnIndex(2);
        ImGui.TextUnformatted(user.Homeworld.ToString());

        ImGui.TableSetColumnIndex(3);
        ImGui.TextUnformatted(user.IsIPC ? "O" : "X");

        ImGui.TableSetColumnIndex(4);
        ImGui.TextUnformatted(UserList.GetUserFromContentID(user.ContentID) != null ? "O" : "X");

        ImGui.EndTable();
    }

    ICallGateSubscriber<string>? getPlayerDataAll;
    ICallGateSubscriber<string, object>? onObjectChangeAll;
    ICallGateSubscriber<string, object>? setPlayerDataAll;
    ICallGateSubscriber<ulong, object>? clearPlayerData;

    string lastData = string.Empty;

    string targetMinionName = baseName;
    string targetBattlePetName = baseName;
    Vector3? targetEdgeColour = null;
    Vector3? targetTextColour = null;

    const string baseName = "[Test Name]";

    bool clicked = false;

    IGameObject? lastTarget = null;

    void SetBaseNames()
    {
        targetMinionName = baseName.ToString();
        targetBattlePetName = baseName.ToString();
        targetEdgeColour = null;
        targetTextColour = null;
    }

    unsafe void DrawIPCTester()
    {
        Vector2 size = new Vector2(ImGui.GetContentRegionAvail().X, 30 * ImGuiHelpers.GlobalScale);

        if (LabledLabel.DrawButton("Recollect Data", "Click here##recollectButton", size))
        {
            GrabLastData();
        }

        if (LabledLabel.DrawButton("Clear Last Data", "Click here##clearButton", size))
        {
            lastData = string.Empty;
        }

        if (Listbox.Begin("##IPCBox", new Vector2(ImGui.GetContentRegionAvail().X, 200)))
        {
            string cleanedData = lastData;
            if (!cleanedData.IsNullOrWhitespace())
            {
                byte[] data = Convert.FromBase64String(cleanedData);
                cleanedData = Encoding.Unicode.GetString(data);
                ImGui.Text(cleanedData);
            }
            Listbox.End();
        }

        ImGui.NewLine();
        ImGui.NewLine();

        IGameObject? target = DalamudServices.TargetManager.Target;

        if (target?.Address != lastTarget?.Address)
        {
            lastTarget = target;
            SetBaseNames();
        }

        IPlayerCharacter? player = target as IPlayerCharacter;

        bool hasTarget = player != null;

        if (hasTarget)
        {
            hasTarget = player!.ObjectIndex != 0;
        }

        if (hasTarget)
        {
            IPettableUser? user = UserList.GetUser(player!.Address);
            if (user != null)
            {
                hasTarget = !user.DataBaseEntry.IsActive;
            }
        }

        LabledLabel.Draw("Target Available", hasTarget ? "Yes" : "No", size);

        if (hasTarget)
        {

            if (Listbox.Begin("##TargetBox", ImGui.GetContentRegionAvail()))
            {
                Vector2 sizeIn = new Vector2(ImGui.GetContentRegionAvail().X, 30 * ImGuiHelpers.GlobalScale);

                LabledLabel.Draw("Target", player!.Name.TextValue, sizeIn);

                BattleChara* bChara = (BattleChara*)player.Address;

                if (LabledLabel.DrawButton("Clear Data", "Click here##clearIPCTarget", sizeIn))
                {
                    ClearIPC(bChara->ObjectIndex);
                }

                string startString = "[PetNicknames(2)]\n";
                startString += bChara->NameString + "\n";
                startString += bChara->HomeWorld + "\n";
                startString += bChara->ContentId + "\n";
                startString += "[-411,-417,-416,-415,-407]";

                
                BattleChara* bPet = CharacterManager.Instance()->LookupPetByOwnerObject(bChara);
                if (bPet != null)
                {
                    RenameLabel.Draw($"Has Battle Pet [{bPet->NameString}]", true, ref targetBattlePetName, ref targetEdgeColour, ref targetTextColour, sizeIn, labelWidth: 300);
                    if (clicked)
                    {
                        int id = -bPet->Character.ModelContainer.ModelCharaId;
                        startString += $"\n{id}^{targetBattlePetName}";
                    }
                }

                Character* minion = &bChara->CompanionObject->Character;
                if (minion != null)
                {
                    RenameLabel.Draw($"Has Minion [{minion->NameString}]", true, ref targetMinionName, ref targetEdgeColour, ref targetTextColour, sizeIn, labelWidth: 300);

                    if (clicked)
                    {
                        int id = minion->ModelContainer.ModelCharaId;
                        startString += $"\n{id}^{targetMinionName}";
                    }
                }

                if (clicked)
                {
                    DalamudServices.PluginLog.Debug(startString);
                    SendAll(startString);
                }

                clicked = LabledLabel.DrawButton("Apply Data", "Click here##applyDataIPC", sizeIn);

                Listbox.End();
            }
        }
    }

    void ClearIPC(ulong chara)
    {
        try
        {
            clearPlayerData?.InvokeAction(chara);
        }
        catch (Exception e) { DalamudServices.PluginLog.Debug(e.Message); }
    }

    void SendAll(string data)
    {
        try
        {
            setPlayerDataAll?.InvokeAction(data);
        }
        catch (Exception e) { DalamudServices.PluginLog.Debug(e.Message); }
    }

    void OnIPCUpdate(bool active)
    {
        if (active)
        {
            ActivateIPC();
        }
        else
        {
            DeactivateIPC();
        }
    }

    void ActivateIPC()
    {
        lastData = string.Empty;

        getPlayerDataAll = DalamudServices.DalamudPlugin.GetIpcSubscriber<string>("PetRenamer.GetPlayerData");
        onObjectChangeAll = DalamudServices.DalamudPlugin.GetIpcSubscriber<string, object>("PetRenamer.PlayerDataChanged");
        setPlayerDataAll = DalamudServices.DalamudPlugin.GetIpcSubscriber<string, object>("PetRenamer.SetPlayerData");
        clearPlayerData = DalamudServices.DalamudPlugin.GetIpcSubscriber<ulong, object>("PetRenamer.ClearPlayerData");

        onObjectChangeAll?.Unsubscribe(OnPlayerDataChanged);
        onObjectChangeAll?.Subscribe(OnPlayerDataChanged);

        GrabLastData();
        SetBaseNames();
    }

    void GrabLastData()
    {
        try
        {
            lastData = getPlayerDataAll?.InvokeFunc() ?? string.Empty;
        }
        catch { }
    }

    void DeactivateIPC()
    {
        lastData = string.Empty;

        onObjectChangeAll?.Unsubscribe(OnPlayerDataChanged);

        getPlayerDataAll = null;
        onObjectChangeAll = null;
        setPlayerDataAll = null;
        clearPlayerData = null;

        SetBaseNames();
    }

    void OnPlayerDataChanged(string data)
    {
        lastData = data;
    }

    void DrawUserList()
    {
        DrawBattlePetCount();
        foreach (IPettableUser? user in UserList.PettableUsers)
        {
            if (user == null) continue;
            NewDrawUser(user);
        }
    }

    unsafe void DrawBattlePetCount()
    {
        int battlePetCount = 0;

        for (int i = 0; i < 100; i++)
        {
            BattleChara* bChara = CharacterManager.Instance()->BattleCharas[i];
            if (bChara == null) continue;

            ObjectKind objKind = bChara->ObjectKind;
            if (objKind != ObjectKind.BattleNpc) continue;

            uint ownerID = bChara->OwnerId;
            if (ownerID == 0xE0000000) continue;

            battlePetCount++;
        }

        int glyceriEstematedBattlePetCount = 0;

        for (int i = 0; i < 100; i++)
        {
            IPettableUser? user = UserList.PettableUsers[i];
            if (user == null) continue;

            foreach(IPettablePet? pet in user.PettablePets)
            {
                if (pet == null) continue;
                if (pet is not IPettableBattlePet) continue;

                glyceriEstematedBattlePetCount++;
            }
        }

        LabledLabel.Draw("Accurate Battle Pet Count: ", $"{battlePetCount}", new Vector2(ImGui.GetContentRegionAvail().X, BarSize), labelWidth: 400);
        LabledLabel.Draw("My calculated Battle Pet Count (These should be equal): ", $"{glyceriEstematedBattlePetCount}", new Vector2(ImGui.GetContentRegionAvail().X, BarSize), labelWidth: 400);
    }

    void NewDrawUser(IPettableUser user)
    {
        if (!ImGui.BeginTable($"##usersTable{WindowHandler.InternalCounter}", 5, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingMask))
            return;

        ImGui.TableNextRow();

        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"{user.Name}");

        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted(user.DataBaseEntry.HomeworldName);

        ImGui.TableSetColumnIndex(2);
        ImGui.TextUnformatted(user.IsActive ? "O" : "X");

        ImGui.TableSetColumnIndex(3);
        ImGui.TextUnformatted(user.DataBaseEntry.ActiveDatabase.Length.ToString());

        foreach (IPettablePet pet in user.PettablePets)
        {
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{pet.SkeletonID}");

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(pet.Name);

            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted(pet.PetData?.BaseSingular);

            ImGui.TableSetColumnIndex(3);
            ImGui.TextUnformatted(pet.PetData?.BasePlural);

            ImGui.TableSetColumnIndex(4);
            ImGui.TextUnformatted(pet.Index.ToString());
        }

        ImGui.EndTable();
    }

    protected override void OnDispose()
    {
        DeactivateIPC();
    }
}

struct DevStruct
{
    public readonly string title;
    public readonly Action onSelected;
    public readonly Action<bool>? requestUpdate;

    public DevStruct(string title, Action onSelected, Action<bool>? requestUpdate = null)
    {
        this.title = title;
        this.onSelected = onSelected;
        this.requestUpdate = requestUpdate;
    }
}
