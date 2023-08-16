using Dalamud.Game;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Serialization;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LocalUserSafetyUpdatable : Updatable
{
    public override void Update(Framework frameWork)
    {
        if (ConfigurationUtils.instance.GetLocalUserV2() != null) return;
        if (!PlayerUtils.instance.PlayerDataAvailable()) return;


        PlayerData? playerData = PlayerUtils.instance.GetPlayerData();
        if(playerData == null) return;
        SerializableUserV2 localUser = new SerializableUserV2(playerData.Value.playerName, playerData.Value.homeWorld);
        ConfigurationUtils.instance.AddNewUserV2(localUser);
    }
}

