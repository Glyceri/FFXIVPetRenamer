using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Windows.Attributes;
using System;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LocalUserSafetyUpdatable : Updatable
{
    IntPtr lastPlayer = IntPtr.Zero;
    public override void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        if (lastPlayer == player.Address) return;
        lastPlayer = player.Address;
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

