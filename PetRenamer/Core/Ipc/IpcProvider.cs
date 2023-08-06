﻿using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Newtonsoft.Json;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;

namespace PetRenamer;

//Most code blatently stolen from https://github.com/Caraxi/Honorific/blob/master/IpcProvider.cs

public static class IpcProvider
{
    public const uint MajorVersion = 1;
    public const uint MinorVersion = 0;

    private static ICallGateProvider<(uint, uint)>? ApiVersion;
    private static ICallGateProvider<object>? Ready;
    private static ICallGateProvider<object>? Disposing;

    private static ICallGateProvider<Character, string, object>? SetCharacterPetNickname;
    private static ICallGateProvider<Character, string>? GetCharacterMinionNickname;
    private static ICallGateProvider<string>? GetLocalCharacterMinionNickname;

    private static ICallGateProvider<Character, string>? GetCharacterPetNickname;
    private static ICallGateProvider<string>? GetLocalCharacterPetNickname;

    private static ICallGateProvider<string, object>? LocalCharacterChangedPetNickname;
    private static ICallGateProvider<Character, object>? ClearCharacter;

    public const string NameSpace = "PetRenamer.";
    public const string ApiVersionIdentifier = $"{NameSpace}{nameof(ApiVersion)}";
    public const string ReadyIdentifier = $"{NameSpace}{nameof(Ready)}";
    public const string DisposingIdentifier = $"{NameSpace}{nameof(Disposing)}";
    public const string SetCharacterNicknameIdentifier = $"{NameSpace}{nameof(SetCharacterPetNickname)}";
    public const string GetCharacterNicknameIdentifier = $"{NameSpace}{nameof(GetCharacterMinionNickname)}";
    public const string GetLocalCharacterPetNicknameIdentifier = $"{NameSpace}{nameof(GetLocalCharacterMinionNickname)}";

    public const string GetCharacterPetNicknameIdentifier = $"{NameSpace}{nameof(GetCharacterPetNickname)}";
    public const string GetLocalCharacterPetPetNicknameIdentifier = $"{NameSpace}{nameof(GetLocalCharacterPetNickname)}";

    public const string ClearCharacterIdentifier = $"{NameSpace}{nameof(ClearCharacter)}";
    public const string LocalCharacterChangedPetNicknameIdentifier = $"{NameSpace}{nameof(LocalCharacterChangedPetNickname)}";

    static RemapUtils remapUtils = null!;
    static NicknameUtils nicknameUtils = null!;
    static ConfigurationUtils configurationUtils = null!;

    internal static void Init(ref DalamudPluginInterface dalamudPluginInterface)
    {
        remapUtils = PluginLink.Utils.Get<RemapUtils>();
        nicknameUtils = PluginLink.Utils.Get<NicknameUtils>();
        configurationUtils = PluginLink.Utils.Get<ConfigurationUtils>();

        ApiVersion = dalamudPluginInterface.GetIpcProvider<(uint, uint)>(ApiVersionIdentifier);
        Ready = dalamudPluginInterface.GetIpcProvider<object>(ReadyIdentifier);
        Disposing = dalamudPluginInterface.GetIpcProvider<object>(DisposingIdentifier);

        SetCharacterPetNickname = dalamudPluginInterface.GetIpcProvider<Character, string, object>(SetCharacterNicknameIdentifier);
        GetCharacterMinionNickname = dalamudPluginInterface.GetIpcProvider<Character, string>(GetCharacterNicknameIdentifier);
        GetLocalCharacterMinionNickname = dalamudPluginInterface.GetIpcProvider<string>(GetLocalCharacterPetNicknameIdentifier);

        GetCharacterPetNickname = dalamudPluginInterface.GetIpcProvider<Character, string>(GetCharacterPetNicknameIdentifier);
        GetLocalCharacterPetNickname = dalamudPluginInterface.GetIpcProvider<string>(GetLocalCharacterPetPetNicknameIdentifier);

        ClearCharacter = dalamudPluginInterface.GetIpcProvider<Character, object>(ClearCharacterIdentifier);
        LocalCharacterChangedPetNickname = dalamudPluginInterface.GetIpcProvider<string, object>(LocalCharacterChangedPetNicknameIdentifier);

        ApiVersion.RegisterFunc(VersionFunction);
        SetCharacterPetNickname.RegisterAction(SetCharacterNicknameCallback);
        GetCharacterMinionNickname.RegisterFunc(GetCharacterNicknameCallback);
        GetLocalCharacterMinionNickname.RegisterFunc(GetLocalCharacterNicknameCallback);
        GetCharacterPetNickname.RegisterFunc(GetCharacterPetNicknameCallback);
        GetLocalCharacterPetNickname.RegisterFunc(GetLocalCharacterPetNicknameCallback);
        ClearCharacter.RegisterAction(ClearCharacterCallback);
    }


    internal static void NotifyReady() => Ready?.SendMessage();
    internal static void NotifyDisposing() => Disposing?.SendMessage();
    internal static void ChangedPetNickname(NicknameData? data)
    {
        string jsonString = data == null ? string.Empty : JsonConvert.SerializeObject(data);
        LocalCharacterChangedPetNickname?.SendMessage(jsonString);
    }


    static (uint, uint) VersionFunction() => (MajorVersion, MinorVersion);

    static void SetCharacterNicknameCallback(Character character, string jsonData)
    {
        try
        {
            if (character is not PlayerCharacter playerCharacter) return;
            (string, uint) player = (playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id);
            List<NicknameData> nicknameDataList;
            bool gotData = PluginLink.IpcStorage.IpcAssignedNicknames.TryGetValue(player, out nicknameDataList!);
            if (!gotData) PluginLink.IpcStorage.IpcAssignedNicknames.Add(player, nicknameDataList = new List<NicknameData>());
            if (jsonData == string.Empty) return;
            NicknameData? nicknameData = JsonConvert.DeserializeObject<NicknameData>(jsonData);
            if (nicknameData == null) return;
            bool setName = false;
            foreach (NicknameData data in nicknameDataList)
                if (data.IDEquals(nicknameData))
                {
                    data.Nickname = nicknameData.Nickname;
                    setName = true;
                    break;
                }
            if(!setName) nicknameDataList.Add(nicknameData);
            if (nicknameData.Nickname == string.Empty)
                nicknameDataList.Remove(nicknameData);
            PluginLink.IpcStorage.IpcAssignedNicknames[player] = nicknameDataList;
        }
        catch (Exception e) { PluginLog.Error(e, $"Error handling {nameof(SetCharacterPetNickname)} IPC."); }
    }

    static string GetCharacterPetNicknameCallback(Character character)
    {
        try
        {
            if (character is not PlayerCharacter playerCharacter) return string.Empty;
            (string, uint) player = (playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id);
            if (!PluginLink.IpcStorage.IpcAssignedNicknames.TryGetValue(player, out List<NicknameData>? data) || data == null) return string.Empty;
            PlayerData? pData = PluginLink.Utils.Get<PlayerUtils>().GetPlayerData();
            if(pData == null ) return string.Empty;
            if (pData.Value.companionData == null ) return string.Empty;
            int id = remapUtils.GetPetIDFromClass(pData.Value.companionData.Value.currentModelID);
            foreach (NicknameData d in data)
                if (d.ID == id)
                    return nicknameUtils.GetNicknameV2(configurationUtils.GetUserV2(playerCharacter.Name.TextValue, (ushort)playerCharacter.HomeWorld.Id), id)?.Name ?? string.Empty;
            return string.Empty;
        }
        catch (Exception e) { PluginLog.Error(e, $"Error handling {nameof(GetCharacterMinionNickname)} IPC."); }

        return string.Empty;
    }

    static string GetLocalCharacterNicknameCallback()
    {
        if(PluginHandlers.ClientState.LocalPlayer == null) return string.Empty;
        return GetCharacterNicknameCallback(PluginHandlers.ClientState.LocalPlayer);
    }

    static string GetCharacterNicknameCallback(Character character)
    {
        try
        {
            if (character is not PlayerCharacter playerCharacter) return string.Empty;
            (string, uint) player = (playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id);
            if (!PluginLink.IpcStorage.IpcAssignedNicknames.TryGetValue(player, out List<NicknameData>? data) || data == null) return string.Empty;
            PlayerData? pData = PluginLink.Utils.Get<PlayerUtils>().GetPlayerData();
            if (pData == null) return string.Empty;
            if (pData.Value.companionData == null) return string.Empty;
            foreach (NicknameData d in data)
                if (d.ID == pData.Value.job)
                    return pData.Value.companionData.Value.currentCompanionName;
            return string.Empty;
        }
        catch (Exception e) { PluginLog.Error(e, $"Error handling {nameof(GetCharacterMinionNickname)} IPC."); }

        return string.Empty;
    }

    static string GetLocalCharacterPetNicknameCallback()
    {
        if (PluginHandlers.ClientState.LocalPlayer == null) return string.Empty;
        return GetCharacterPetNicknameCallback(PluginHandlers.ClientState.LocalPlayer);
    }

    static void ClearCharacterCallback(Character character)
    {
        if (character is not PlayerCharacter playerCharacter) return;
        PluginLink.IpcStorage.IpcAssignedNicknames.Remove((playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id));
    }

    internal static void DeInit()
    {
        ApiVersion?.UnregisterFunc();

        SetCharacterPetNickname?.UnregisterAction();
        GetCharacterMinionNickname?.UnregisterFunc();
        GetLocalCharacterMinionNickname?.UnregisterFunc();

        GetCharacterPetNickname?.UnregisterFunc();
        GetLocalCharacterPetNickname?.UnregisterFunc();

        ClearCharacter?.UnregisterAction();

        LocalCharacterChangedPetNickname = null;
        Ready = null;
        Disposing = null;
    }
}
