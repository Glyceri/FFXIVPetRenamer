using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Pet;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core;
using Dalamud.Game.ClientState.Objects.SubKinds;
using System.Text;
using System.Text.RegularExpressions;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class IpcUtils : UtilsRegistryType, ISingletonBase<IpcUtils>
{
    public static IpcUtils instance { get; set; } = null!;

    internal override void OnRegistered() => PluginLink.IpcStorage.Register(OnIpcChange);
    internal override void Dispose() => PluginLink.IpcStorage.Deregister(OnIpcChange);

    public void OnIpcChange(ref List<(IPlayerCharacter, string)> data)
    {
        foreach ((IPlayerCharacter, string) item in data)
            SetNickname(item.Item1, item.Item2);
    }

    public string GetNickname(nint pet)
    {
        PetBase pBase = PluginLink.PettableUserHandler.GetPet(pet);
        if (pBase == null) return null!;
        string customName = pBase.CustomName;
        if (customName == string.Empty) return null!;
        return customName;
    }

    public void SetNickname(IPlayerCharacter player, string data)
    {
        try
        {
            if (data == null!) return;
            PettableUser user = PluginLink.PettableUserHandler.GetUser(player.Address);
            // You may not edit the local user!
            if (user != null) 
                if (user.LocalUser) 
                    return;
            string[] lines = data.Split('\n');
            if (lines.Length == 0) return;
            string identifier = lines[0];


            if (identifier == PluginConstants.IpcClear) 
            {
                Clear(user!);
                return;
            }

            if (user == null)
            {
                PluginLink.PettableUserHandler.DeclareUser(new SerializableUserV3(player.Name.ToString(), (ushort)player.HomeWorld.Id), UserDeclareType.IPC);
                user = PluginLink.PettableUserHandler.GetUser(player.Name.ToString(), (ushort)player.HomeWorld.Id)!;
                if (user == null) return;
            }

            if (identifier == PluginConstants.IpcAll) Clear(user);
            if (identifier == PluginConstants.IpcAll || identifier == PluginConstants.IpcSingle) SetAll(user, lines);
        }
        catch { }
    }

    void Clear(PettableUser user)
    {
        if (user == null) return;
        user.SerializableUser.ClearAllIPC();
    }

    public void SetAll(PettableUser user, string[] data)
    {
        foreach (string line in data)
        {
            (int, string) parsedData = DataFromLine(line);
            if (parsedData.Item1 == -1) continue;
            if (!NameIsValid(parsedData.Item2)) continue;
            user.SerializableUser.SaveNickname(parsedData.Item1, parsedData.Item2, true, false, true);
        }
    }

    // Checks for URL
    Regex validateDateRegex = new Regex("^[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
    Regex validateUrlRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");

    bool NameIsValid(string name)
    {
        if (name == null) return false;
        if (name.Length > 64) return false;
        if (validateDateRegex.IsMatch(name)) return false;
        if (validateUrlRegex.IsMatch(name)) return false;
        return true;
    }

    (int, string) DataFromLine(string line)
    {
        if (line == null) return (-1, string.Empty);
        string[] splitLines = line.Split(PluginConstants.forbiddenCharacter);
        if (splitLines.Length != 2) return (-1, string.Empty);
        try
        {
            return (int.Parse(splitLines[0]), splitLines[1]);
        }
        catch
        {
            return (-1, string.Empty);
        }
    }

    public string GetAllLocalPlayerData()
    {
        PettableUser localPlayer = PluginLink.PettableUserHandler.LocalUser()!;
        if (localPlayer == null) return string.Empty;
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(PluginConstants.IpcAll);
        for (int i = 0; i < localPlayer.SerializableUser.length; i++)
        {
            QuickName quickName = localPlayer.SerializableUser[i];
            builder.Append(quickName.ID.ToString());
            builder.Append(PluginConstants.forbiddenCharacter.ToString());
            builder.AppendLine(quickName.RawName);
        }
        return builder.ToString();
    }

    public string CreateSingleChange(int id, string newNickname)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(PluginConstants.IpcSingle);
        builder.AppendLine($"{id}{PluginConstants.forbiddenCharacter}{newNickname}");
        return builder.ToString();
    }

    public void NotifyChange(int id, string newNickname) => IpcProvider.NotifyPlayerDataChangedSingle(CreateSingleChange(id, newNickname));
    public void SendAllData() => IpcProvider.NotifyPlayerDataChangedAll(GetAllLocalPlayerData());
}
