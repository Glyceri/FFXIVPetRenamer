using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LocalUserSafetyUpdatable : Updatable
{
    PlayerCharacter lastPlayer = null!;
    public override void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        if (lastPlayer == player) return;
        lastPlayer = player;
        if (player == null) return;
        if (PluginLink.Configuration.serializableUsersV3! == null) return;
        int count = PluginLink.Configuration.serializableUsersV3!.Length;
        for (int i = 0; i < count; i++)
            if (PluginLink.Configuration.serializableUsersV3[i].Equals(player.Name.ToString(), (ushort)player.HomeWorld.Id))
                return;    
       
        SerializableUserV3 newUser = new SerializableUserV3(player.Name.ToString(), (ushort)player.HomeWorld.Id);
        PluginLink.PettableUserHandler.DeclareUser(newUser, PettableUserSystem.Enums.UserDeclareType.Add);
        PluginLink.Configuration.Save();
    }
}

