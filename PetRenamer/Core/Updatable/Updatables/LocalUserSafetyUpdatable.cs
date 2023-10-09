using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LocalUserSafetyUpdatable : Updatable
{
    public override void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        if (PluginLink.Configuration.serializableUsersV3! == null) return;
        foreach (SerializableUserV3 user in PluginLink.Configuration.serializableUsersV3!)
            if (user.Equals(player.Name.ToString(), (ushort)player.HomeWorld.Id))
                return;

        SerializableUserV3 newUser = new SerializableUserV3(player.Name.ToString(), (ushort)player.HomeWorld.Id);
        PluginLink.PettableUserHandler.DeclareUser(newUser, PettableUserSystem.Enums.UserDeclareType.Add);
        PluginLink.Configuration.Save();
    }
}

