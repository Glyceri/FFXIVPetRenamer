using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Newtonsoft.Json;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Logging;
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
    private static ICallGateProvider<bool>? Enabled;

    private static ICallGateProvider<Character, string, object>? SetCharacterNickname;
    private static ICallGateProvider<Character, string>? GetCharacterNickname;
    private static ICallGateProvider<string>? GetLocalCharacterNickname;
    private static ICallGateProvider<string, object>? LocalCharacterChangedNickname;
    private static ICallGateProvider<Character, object>? ClearCharacter;

    private static ICallGateProvider<GameObject, string>? GetPetNickname;
    private static ICallGateProvider<nint, string>? GetPetNicknameNint;

    public const string NameSpace = "PetRenamer.";
    public const string ApiVersionIdentifier = $"{NameSpace}{nameof(ApiVersion)}";
    public const string ReadyIdentifier = $"{NameSpace}{nameof(Ready)}";
    public const string EnabledIdentifier = $"{NameSpace}{nameof(Enabled)}";
    public const string DisposingIdentifier = $"{NameSpace}{nameof(Disposing)}";
    public const string SetCharacterNicknameIdentifier = $"{NameSpace}{nameof(SetCharacterNickname)}";
    public const string GetCharacterNicknameIdentifier = $"{NameSpace}{nameof(GetCharacterNickname)}";
    public const string GetLocalCharacterPetNicknameIdentifier = $"{NameSpace}{nameof(GetLocalCharacterNickname)}";

    public const string GetPetNicknameIdentifier = $"{NameSpace}{nameof(GetPetNickname)}";
    public const string GetPetNicknameNintIdentifier = $"{NameSpace}{nameof(GetPetNicknameNint)}";

    public const string ClearCharacterIdentifier = $"{NameSpace}{nameof(ClearCharacter)}";
    public const string LocalCharacterChangedPetNicknameIdentifier = $"{NameSpace}{nameof(LocalCharacterChangedNickname)}";

    internal static void Init(ref DalamudPluginInterface dalamudPluginInterface)
    {
        ApiVersion = dalamudPluginInterface.GetIpcProvider<(uint, uint)>(ApiVersionIdentifier);
        Ready = dalamudPluginInterface.GetIpcProvider<object>(ReadyIdentifier);
        Disposing = dalamudPluginInterface.GetIpcProvider<object>(DisposingIdentifier);
        Enabled = dalamudPluginInterface.GetIpcProvider<bool>(EnabledIdentifier);

        SetCharacterNickname = dalamudPluginInterface.GetIpcProvider<Character, string, object>(SetCharacterNicknameIdentifier);
        GetCharacterNickname = dalamudPluginInterface.GetIpcProvider<Character, string>(GetCharacterNicknameIdentifier);
        GetLocalCharacterNickname = dalamudPluginInterface.GetIpcProvider<string>(GetLocalCharacterPetNicknameIdentifier);
        ClearCharacter = dalamudPluginInterface.GetIpcProvider<Character, object>(ClearCharacterIdentifier);
        LocalCharacterChangedNickname = dalamudPluginInterface.GetIpcProvider<string, object>(LocalCharacterChangedPetNicknameIdentifier);

        GetPetNickname = dalamudPluginInterface.GetIpcProvider<GameObject, string>(GetPetNicknameIdentifier);
        GetPetNicknameNint = dalamudPluginInterface.GetIpcProvider<nint, string>(GetPetNicknameNintIdentifier);

        ApiVersion.RegisterFunc(VersionFunction);
        Enabled.RegisterFunc(() => true);
        SetCharacterNickname.RegisterAction(SetCharacterNicknameCallback);
        GetCharacterNickname.RegisterFunc(GetCharacterNicknameCallback);
        GetLocalCharacterNickname.RegisterFunc(GetLocalCharacterNicknameCallback);
        ClearCharacter.RegisterAction(ClearCharacterCallback);

        GetPetNickname.RegisterFunc(GetPetNicknameCallback);
        GetPetNicknameNint.RegisterFunc(GetPetNicknameFromNintCallback);
    }


    internal static void NotifyReady() => Ready?.SendMessage();
    internal static void NotifyDisposing() => Disposing?.SendMessage();
    internal static void ChangedPetNickname(NicknameData? data)
    {
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
        catch (Exception e) { PetLog.LogError(e, $"Error handling {nameof(SetCharacterNickname)} IPC."); }
    }

    public static string GetLocalCharacterNicknameCallback()
    {
        if (PluginHandlers.ClientState.LocalPlayer == null) return string.Empty;
        return GetCharacterNicknameCallback(PluginHandlers.ClientState.LocalPlayer);
    }

    static string GetPetNicknameFromNintCallback(nint pet)
    {
        PetBase pBase = PluginLink.PettableUserHandler.GetPet(pet);
        if (pBase == null) return null!;
        return pBase.CustomName;
    }

    static unsafe string GetPetNicknameCallback(GameObject pet) => GetPetNicknameFromNintCallback(pet?.Address ?? nint.Zero);

    static string GetCharacterNicknameCallback(Character character)
    {
        try
        {
            if (character is not PlayerCharacter playerCharacter) return string.Empty;
            PettableUser user = PluginLink.PettableUserHandler.GetUser(character.Address);
            if (user == null) return string.Empty;

            NicknameData data = new NicknameData(user.Minion.ID, user.Minion.UsedName, user.BattlePet.ID, user.BattlePet.UsedName);
            return data == null ? string.Empty : JsonConvert.SerializeObject(data);
        }
        catch (Exception e) { PetLog.LogError(e, $"Error handling {nameof(GetCharacterNickname)} IPC."); }

        return string.Empty;
    }

    static void ClearCharacterCallback(Character character)
    {
        if (character is not PlayerCharacter playerCharacter) return;
        PluginLink.IpcStorage.IpcAssignedNicknames.Remove((playerCharacter.Name.TextValue, playerCharacter.HomeWorld.Id));
    }

    internal static void DeInit()
    {
        ApiVersion?.UnregisterFunc();
        Enabled?.UnregisterFunc();

        GetPetNickname?.UnregisterFunc();
        GetPetNicknameNint?.UnregisterFunc();

        SetCharacterNickname?.UnregisterAction();
        GetCharacterNickname?.UnregisterFunc();
        GetLocalCharacterNickname?.UnregisterFunc();

        ClearCharacter?.UnregisterAction();

        LocalCharacterChangedNickname = null;
        Ready = null;
        Disposing = null;
    }
}
