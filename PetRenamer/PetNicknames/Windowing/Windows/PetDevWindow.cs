using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Ipc;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using ImGuiNET;
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

    protected override Vector2 MinSize { get; } = new Vector2(350, 136);
    protected override Vector2 MaxSize { get; } = new Vector2(2000, 2000);
    protected override Vector2 DefaultSize { get; } = new Vector2(800, 400);
    protected override bool HasModeToggle { get; } = true;

    float BarSize = 30 * ImGuiHelpers.GlobalScale;

    int currentActive = 0;
    List<DevStruct> devStructList = new List<DevStruct>();

    public PetDevWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, in IPettableUserList userList) : base(windowHandler, dalamudServices, configuration, "Pet Dev Window", ImGuiWindowFlags.None)
    {
        UserList = userList;

        if (configuration.debugModeActive && configuration.openDebugWindowOnStart)
        {
            Open();
        }

        devStructList.Add(new DevStruct("User List", DrawUserList));
        devStructList.Add(new DevStruct("IPC Tester", DrawIPCTester, OnIPCUpdate));
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

    ICallGateSubscriber<string>? getPlayerDataAll;
    ICallGateSubscriber<string, object>? onObjectChangeAll;
    ICallGateSubscriber<string, object>? setPlayerDataAll;
    ICallGateSubscriber<ulong, object>? clearPlayerData;

    string lastData = string.Empty;

    string targetMinionName = baseName;
    string targetBattlePetName = baseName;

    const string baseName = "[Test Name]";

    bool clicked = false;

    IGameObject? lastTarget = null;

    void SetBaseNames()
    {
        targetMinionName = baseName.ToString();
        targetBattlePetName = baseName.ToString();
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
                    RenameLabel.Draw($"Has Battle Pet [{bPet->NameString}]", true, ref targetBattlePetName, sizeIn, labelWidth: 300);
                    if (clicked)
                    {
                        int id = -bPet->Character.CharacterData.ModelCharaId;
                        startString += $"\n{id}^{targetBattlePetName}";
                    }
                }

                Character* minion = &bChara->CompanionObject->Character;
                if (minion != null)
                {
                    RenameLabel.Draw($"Has Minion [{minion->NameString}]", true, ref targetMinionName, sizeIn, labelWidth: 300);

                    if (clicked)
                    {
                        int id = minion->CharacterData.ModelCharaId;
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

        getPlayerDataAll = DalamudServices.PetNicknamesPlugin.GetIpcSubscriber<string>("PetRenamer.GetPlayerData");
        onObjectChangeAll = DalamudServices.PetNicknamesPlugin.GetIpcSubscriber<string, object>("PetRenamer.PlayerDataChanged");
        setPlayerDataAll = DalamudServices.PetNicknamesPlugin.GetIpcSubscriber<string, object>("PetRenamer.SetPlayerData");
        clearPlayerData = DalamudServices.PetNicknamesPlugin.GetIpcSubscriber<ulong, object>("PetRenamer.ClearPlayerData");

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
        foreach (IPettableUser? user in UserList.PettableUsers)
        {
            if (user == null) continue;
            NewDrawUser(user);
        }
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
