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

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class IpcUtils : UtilsRegistryType, ISingletonBase<IpcUtils>
{
    public static IpcUtils instance { get; set; } = null!;

    internal override void OnRegistered() => PluginLink.IpcStorage.Register(OnIpcChange);
    internal override void Dispose() => PluginLink.IpcStorage.Deregister(OnIpcChange);

    public void OnIpcChange(ref List<(PlayerCharacter, string)> data)
    {
        foreach ((PlayerCharacter, string) item in data)
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

    public void SetNickname(PlayerCharacter player, string data)
    {
        try
        {
            if (data == null!) return;
            PettableUser user = PluginLink.PettableUserHandler.GetUser(player.Address);
            string[] lines = data.Split('\n');
            if (lines.Length == 0) return;
            string identifier = lines[0];


            if (identifier == PluginConstants.IpcClear) 
            {
                Clear(user);
                return;
            }

            if (user == null)
            {
                PluginLink.PettableUserHandler.DeclareUser(new SerializableUserV3(player.Name.ToString(), (ushort)player.HomeWorld.Id), UserDeclareType.IPC);
                PluginLink.PettableUserHandler.GetUser(player.Name.ToString(), (ushort)player.HomeWorld.Id);
                if (user == null) return;
            }

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
            user.SerializableUser.SaveNickname(parsedData.Item1, parsedData.Item2, true, false, true);
        }
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
            builder.AppendLine(quickName.ID.ToString());
            builder.AppendLine(PluginConstants.forbiddenCharacter.ToString());
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
