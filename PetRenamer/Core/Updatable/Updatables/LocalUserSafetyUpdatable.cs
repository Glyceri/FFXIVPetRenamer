using Dalamud.Game;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Serialization;
using Dalamud.Game.ClientState.Objects.SubKinds;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LocalUserSafetyUpdatable : Updatable
{
    public override void Update(Framework frameWork)
    {
        if (!PlayerUtils.instance.PlayerDataAvailable()) return;
        PlayerUtils utils = PlayerUtils.instance;
        PlayerCharacter character = utils.PlayerCharacter!;
        if(character == null) return; 

        SerializableUserV2 localUser = new SerializableUserV2(character.Name.ToString(), (ushort)character.HomeWorld.Id);
        ConfigurationUtils.instance.AddNewUserV2(localUser);
    }
}

