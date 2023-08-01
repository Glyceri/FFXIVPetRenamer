using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Serialization;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LocalUserSafetyUpdatable : Updatable
{
    PlayerUtils playerUtils { get; set; } = null!;

    public LocalUserSafetyUpdatable() : base()
    {
        playerUtils = PluginLink.Utils.Get<PlayerUtils>();
    }

    public override void Update(Framework frameWork)
    {
        if (PluginLink.Configuration.serializableUsers!.Length != 0) return;
        if (!playerUtils.PlayerDataAvailable()) return;


        PlayerData playerData = playerUtils.GetPlayerData()!.Value;
        PluginLink.Configuration.serializableUsers = new SerializableUser[1] { new SerializableUser(new SerializableNickname[0], playerData.playerName, playerData.homeWorld) };
    }
}

