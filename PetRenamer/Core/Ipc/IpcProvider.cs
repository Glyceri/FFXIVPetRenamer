using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Newtonsoft.Json;
using PetRenamer.Core.Handlers;
using System;
using System.Collections.Generic;

namespace PetRenamer;

//Most code blatently stolen from https://github.com/Caraxi/Honorific/blob/master/IpcProvider.cs

public static class IpcProvider
{
    public const uint MajorVersion = 1;
    public const uint MinorVersion = 0;

    public static ICallGateProvider<(uint, uint)>? ApiVersion;

    private static ICallGateProvider<object>? Ready;
    private static ICallGateProvider<object>? Disposing;
    private static ICallGateProvider<Character, string, object>? SetCharacterNickname;
    private static ICallGateProvider<Character, int, string>? GetCharacterNickname;


    public const string NameSpace = "PetRenamer.";
    public const string ApiVersionIdentifier = $"{NameSpace}{nameof(ApiVersion)}";
    public const string ReadyIdentifier = $"{NameSpace}{nameof(Ready)}";
    public const string DisposingIdentifier = $"{NameSpace}{nameof(Disposing)}";
    public const string SetCharacterNicknameIdentifier = $"{NameSpace}{nameof(SetCharacterNickname)}";
    public const string GetCharacterNicknameIdentifier = $"{NameSpace}{nameof(GetCharacterNickname)}";

    internal static void Init(ref DalamudPluginInterface dalamudPluginInterface)
    {
        ApiVersion = dalamudPluginInterface.GetIpcProvider<(uint, uint)>(ApiVersionIdentifier);
        Ready = dalamudPluginInterface.GetIpcProvider<object>(ReadyIdentifier);
        Disposing = dalamudPluginInterface.GetIpcProvider<object>(DisposingIdentifier);

        SetCharacterNickname = dalamudPluginInterface.GetIpcProvider<Character, string, object>(SetCharacterNicknameIdentifier);
        GetCharacterNickname = dalamudPluginInterface.GetIpcProvider<Character, int, string>(GetCharacterNicknameIdentifier);

        ApiVersion.RegisterFunc(VersionFunction);
        SetCharacterNickname.RegisterAction(SetCharacterNicknameCallback);
        GetCharacterNickname.RegisterFunc(GetCharacterNicknameCallback);
    }


    internal static void NotifyReady() => Ready?.SendMessage();
    internal static void NotifyDisposing() => Disposing?.SendMessage();

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
            PluginLink.IpcStorage.IpcAssignedNicknames[player] = nicknameDataList;
        }
        catch (Exception e) { PluginLog.Error(e, $"Error handling {nameof(SetCharacterNickname)} IPC."); }
    }

    static string GetCharacterNicknameCallback(Character character, int pet)
    {
        try
        {
            if (character is not PlayerCharacter playerCharacter) return string.Empty;
            (string, uint) player = (playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id);
            if (!PluginLink.IpcStorage.IpcAssignedNicknames.TryGetValue(player, out List<NicknameData>? data) || data == null) return string.Empty;
            foreach(NicknameData d in data)
                if(d.ID == pet)
                    return JsonConvert.SerializeObject(d);
        }
        catch (Exception e) { PluginLog.Error(e, $"Error handling {nameof(GetCharacterNickname)} IPC."); }

        return string.Empty;
    }


    internal static void DeInit()
    {
        ApiVersion?.UnregisterFunc();
        SetCharacterNickname?.UnregisterAction();

        Ready = null;
        Disposing = null;
    }
}
