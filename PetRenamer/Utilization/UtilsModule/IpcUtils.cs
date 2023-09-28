using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Singleton;
using PetRenamer.Core.PettableUserSystem;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class IpcUtils : UtilsRegistryType, ISingletonBase<IpcUtils>
{
    public static IpcUtils instance { get; set; } = null!;

    internal override void OnRegistered() => PluginLink.IpcStorage.Register(OnIpcChange);
    internal override void Dispose() => PluginLink.IpcStorage.Deregister(OnIpcChange);

    public void OnIpcChange(ref Dictionary<(string, uint), NicknameData> data)
    {
        foreach (var kvp in data)
        {
            PluginLink.PettableUserHandler.DeclareUser(new SerializableUserV3(kvp.Key.Item1, (ushort)kvp.Key.Item2), Core.PettableUserSystem.Enums.UserDeclareType.Add);
            foreach(PettableUser user in PluginLink.PettableUserHandler.Users)
            {
                if (!user.SerializableUser.Equals(kvp.Key.Item1, (ushort)kvp.Key.Item2)) continue;
                user.SerializableUser.SaveNickname(kvp.Value.ID, kvp.Value.Nickname!);
                if (kvp.Value.ID != kvp.Value.BattleID)
                    user.SerializableUser.SaveNickname(kvp.Value.BattleID, kvp.Value.BattleNickname!);
            }
        }
        PluginLink.Configuration.Save();
    }
}
