using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Singleton;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class IpcUtils : UtilsRegistryType, ISingletonBase<IpcUtils>
{
    public static IpcUtils instance { get; set; } = null!;

    internal override void OnRegistered()
    {
        PluginLink.IpcStorage.Register(OnIpcChange);
    }

    internal override void Dispose()
    {
        PluginLink.IpcStorage.Deregister(OnIpcChange);
    }

    public void OnIpcChange(Dictionary<(string, uint), NicknameData> data)
    {
     
        foreach (var kvp in data)
        {
            SerializableUserV2? storedUser = new SerializableUserV2(kvp.Key.Item1, (ushort)kvp.Key.Item2);
            SerializableUserV2? user = ConfigurationUtils.instance.GetUserV2(storedUser);
            if (user == null) 
            {
                ConfigurationUtils.instance.AddNewUserV2(user!);
                SerializableUserV2? user2 = ConfigurationUtils.instance.GetUserV2(storedUser);
                user2?.SaveNickname(new SerializableNickname(kvp.Value.ID, kvp.Value.Nickname!));
            }
            else
            {
                user?.SaveNickname(new SerializableNickname(kvp.Value.ID, kvp.Value.Nickname!));
            }
        }
    }
}
