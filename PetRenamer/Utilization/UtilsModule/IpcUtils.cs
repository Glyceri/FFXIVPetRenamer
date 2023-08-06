using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using PetRenamer.Core.Serialization;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class IpcUtils : UtilsRegistryType
{
    ConfigurationUtils ConfigurationUtils { get; set; } = null!;

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
        if (ConfigurationUtils == null) PluginLink.Utils.Get<ConfigurationUtils>();
        
        foreach (var kvp in data)
        {
            SerializableUserV2? storedUser = new SerializableUserV2(kvp.Key.Item1, (ushort)kvp.Key.Item2);
            SerializableUserV2? user = ConfigurationUtils!.GetUserV2(storedUser);
            if (user == null) 
            {
                ConfigurationUtils!.AddNewUserV2(user!);
                SerializableUserV2? user2 = ConfigurationUtils!.GetUserV2(storedUser);
                user2?.SaveNickname(new SerializableNickname(kvp.Value.ID, kvp.Value.Nickname!));
            }
            else
            {
                user?.SaveNickname(new SerializableNickname(kvp.Value.ID, kvp.Value.Nickname!));
            }
        }
    }
}
