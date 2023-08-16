using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Newtonsoft.Json;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Updatable.Updatables;
using PetRenamer.Utilization.UtilsModule;
using System;

namespace PetRenamer;

//Most code blatently stolen from https://github.com/Caraxi/Honorific/blob/master/IpcProvider.cs

public static class IpcProvider
{
    public const uint MajorVersion = 1;
    public const uint MinorVersion = 0;

    private static ICallGateProvider<(uint, uint)>? ApiVersion;
    private static ICallGateProvider<object>? Ready;
    private static ICallGateProvider<object>? Disposing;

    private static ICallGateProvider<Character, string, object>? SetCharacterNickname;
    private static ICallGateProvider<Character, string>? GetCharacterNickname;
    private static ICallGateProvider<string>? GetLocalCharacterNickname;
    private static ICallGateProvider<string, object>? LocalCharacterChangedNickname;
    private static ICallGateProvider<Character, object>? ClearCharacter;

    public const string NameSpace = "PetRenamer.";
    public const string ApiVersionIdentifier = $"{NameSpace}{nameof(ApiVersion)}";
    public const string ReadyIdentifier = $"{NameSpace}{nameof(Ready)}";
    public const string DisposingIdentifier = $"{NameSpace}{nameof(Disposing)}";
    public const string SetCharacterNicknameIdentifier = $"{NameSpace}{nameof(SetCharacterNickname)}";
    public const string GetCharacterNicknameIdentifier = $"{NameSpace}{nameof(GetCharacterNickname)}";
    public const string GetLocalCharacterPetNicknameIdentifier = $"{NameSpace}{nameof(GetLocalCharacterNickname)}";

    public const string ClearCharacterIdentifier = $"{NameSpace}{nameof(ClearCharacter)}";
    public const string LocalCharacterChangedPetNicknameIdentifier = $"{NameSpace}{nameof(LocalCharacterChangedNickname)}";

    internal static void Init(ref DalamudPluginInterface dalamudPluginInterface)
    {
        ApiVersion = dalamudPluginInterface.GetIpcProvider<(uint, uint)>(ApiVersionIdentifier);
        Ready = dalamudPluginInterface.GetIpcProvider<object>(ReadyIdentifier);
        Disposing = dalamudPluginInterface.GetIpcProvider<object>(DisposingIdentifier);

        SetCharacterNickname = dalamudPluginInterface.GetIpcProvider<Character, string, object>(SetCharacterNicknameIdentifier);
        GetCharacterNickname = dalamudPluginInterface.GetIpcProvider<Character, string>(GetCharacterNicknameIdentifier);
        GetLocalCharacterNickname = dalamudPluginInterface.GetIpcProvider<string>(GetLocalCharacterPetNicknameIdentifier);
        ClearCharacter = dalamudPluginInterface.GetIpcProvider<Character, object>(ClearCharacterIdentifier);
        LocalCharacterChangedNickname = dalamudPluginInterface.GetIpcProvider<string, object>(LocalCharacterChangedPetNicknameIdentifier);

        ApiVersion.RegisterFunc(VersionFunction);
        SetCharacterNickname.RegisterAction(SetCharacterNicknameCallback);
        GetCharacterNickname.RegisterFunc(GetCharacterNicknameCallback);
        GetLocalCharacterNickname.RegisterFunc(GetLocalCharacterNicknameCallback);
        ClearCharacter.RegisterAction(ClearCharacterCallback);
    }


    internal static void NotifyReady() => Ready?.SendMessage();
    internal static void NotifyDisposing() => Disposing?.SendMessage();
    internal static void ChangedPetNickname(NicknameData? data)
    {
        PluginLog.Log("Set nickname data: " + data?.ToNormalString() ?? string.Empty);

        if (PluginHandlers.ClientState.LocalPlayer is PlayerCharacter playerCharacter)
        {
            (string, uint) player = (playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id);
            if (!PluginLink.IpcStorage.IpcAssignedNicknames.TryAdd(player, data!))
                PluginLink.IpcStorage.IpcAssignedNicknames[player] = data!;
        }

        string jsonString = data == null ? string.Empty : JsonConvert.SerializeObject(data);
        LocalCharacterChangedNickname?.SendMessage(jsonString);
    }


    static (uint, uint) VersionFunction() => (MajorVersion, MinorVersion);

    static void SetCharacterNicknameCallback(Character character, string jsonData)
    {
        try
        {
            if (character is not PlayerCharacter playerCharacter) return;
            PluginLink.IpcStorage.IpcAssignedNicknames.Remove((playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id));
            if (jsonData == string.Empty) return;
            NicknameData? data = JsonConvert.DeserializeObject<NicknameData>(jsonData);
            if (data == null) return;
            PluginLink.IpcStorage.IpcAssignedNicknames.Add((playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id), data);
        }
        catch (Exception e) { PluginLog.Error(e, $"Error handling {nameof(SetCharacterNickname)} IPC."); }
    }

    public static string GetLocalCharacterNicknameCallback()
    {
        if (PluginHandlers.ClientState.LocalPlayer == null) return string.Empty;
        return GetCharacterNicknameCallback(PluginHandlers.ClientState.LocalPlayer);
    }

    static string GetCharacterNicknameCallback(Character character)
    {
        try
        {
            if (character is not PlayerCharacter playerCharacter) return string.Empty;
            (string, uint) player = (playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id);
            NicknameData? data = new NicknameData();
            SerializableUserV2 userv2 = ConfigurationUtils.instance.GetUserV2(new SerializableUserV2(player.Item1, (ushort)player.Item2));
            if(userv2 == null) return string.Empty;
            foreach (FoundPlayerCharacter chara in PluginLink.IpcStorage.characters)
            {
                if (chara == null) continue;
                if (chara.ownName != userv2.username || chara.ownHomeWorld != userv2.homeworld) continue;

                (int, string) cStr = (-1, string.Empty);
                (int, string) bStr = (-1, string.Empty);
                if (chara.HasCompanion())
                    cStr = FromIdAndUser(userv2, chara.GetCompanionID());
                
                if (chara.HasBattlePet())
                    bStr = FromIdAndUser(userv2, chara.GetBattlePetID());

                data.ID = cStr.Item1;
                data.Nickname = cStr.Item2;
                data.BattleID = bStr.Item1;
                data.BattleNickname = bStr.Item2;
                break;
            }
            string jsonString = data == null ? string.Empty : JsonConvert.SerializeObject(data);
            return jsonString;
        }
        catch (Exception e) { PluginLog.Error(e, $"Error handling {nameof(GetCharacterNickname)} IPC."); }

        return string.Empty;
    }

    static (int, string) FromIdAndUser(SerializableUserV2 user, int id)
    {
        SerializableNickname? nName = NicknameUtils.instance.GetNicknameV2(user, id);
        if (nName == null) return (-1, string.Empty);
        if (!nName.Valid()) return (-1, string.Empty);
        return (nName.ID, nName.Name);
    }

    static void ClearCharacterCallback(Character character)
    {
        if (character is not PlayerCharacter playerCharacter) return;
        PluginLink.IpcStorage.IpcAssignedNicknames.Remove((playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id));
    }

    internal static void DeInit()
    {
        ApiVersion?.UnregisterFunc();

        SetCharacterNickname?.UnregisterAction();
        GetCharacterNickname?.UnregisterFunc();
        GetLocalCharacterNickname?.UnregisterFunc();

        ClearCharacter?.UnregisterAction();

        LocalCharacterChangedNickname = null;
        Ready = null;
        Disposing = null;
    }
}
