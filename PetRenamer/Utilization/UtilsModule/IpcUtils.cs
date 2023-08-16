using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Singleton;
using Dalamud.Logging;

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
                user = ConfigurationUtils.instance.GetUserV2(storedUser);
            }
            user?.SaveNickname(new SerializableNickname(kvp.Value.ID, kvp.Value.Nickname!));
            if(kvp.Value.ID != kvp.Value.BattleID)
                user?.SaveNickname(new SerializableNickname(kvp.Value.BattleID, kvp.Value.BattleNickname!));
        }
        PluginLink.Configuration.Save();
    }
}
