#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Legacy.Attributes;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.Legacy.LegacyElements;

[Legacy(new int[1] { 2 })]
internal class LegacyNamingVer2 : LegacyElement
{

    internal override void OnPlayerAvailable(int detectedVersion)
    {
        if (detectedVersion != 2) return;
        if (PluginLink.Configuration.users!.Length == 0) return;

        PlayerData? playerData = PluginLink.Utils.Get<PlayerUtils>().GetPlayerData();
        if(playerData == null) return;
        SerializableUser newSerializableUser = new SerializableUser((SerializableNickname[])PluginLink.Configuration.users.Clone(), playerData.Value.playerName, playerData.Value.homeWorld);
        PluginLink.Configuration.users = new SerializableNickname[0];
        PluginLink.Configuration.Version = 3;
        PluginLink.Configuration.serializableUsers = new SerializableUser[1] { newSerializableUser };
        PluginLink.Configuration.Save();
    }
}
#pragma warning restore CS0618 // Type or member is obsolete