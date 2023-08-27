using Dalamud.Game;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LocalUserSafetyUpdatable : Updatable
{
    public override void Update(Framework frameWork)
    {
        if (PluginLink.Configuration.serializableUsersV3! == null) return;
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        foreach (SerializableUserV3 user in PluginLink.Configuration.serializableUsersV3!)
            if (user.Equals(PluginHandlers.ClientState.LocalPlayer!.Name.ToString(), (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id))
                return;
       
       
        SerializableUserV3 newUser = new SerializableUserV3(PluginHandlers.ClientState.LocalPlayer!.Name.ToString(), (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id);
        PluginLink.PettableUserHandler.DeclareUser(newUser, PettableUserSystem.Enums.UserDeclareType.Add);
        PluginLink.Configuration.Save();
    }
}

