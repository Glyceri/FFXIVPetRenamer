using Dalamud.Game;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Handlers;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Updatable.Updatables;

//[Updatable]
internal class LocalUserSafetyUpdatable : Updatable
{
    public override void Update(Framework frameWork)
    {
        if (PluginLink.Configuration.serializableUsersV3! == null) return;
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        foreach (SerializableUserV3 user in PluginLink.Configuration.serializableUsersV3!)
            if (user.Equals(PluginHandlers.ClientState.LocalPlayer!.Name.ToString(), (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id))
                return;
       
        List<int> ids = new List<int>();
        foreach(int id in PluginConstants.allowedNegativePetIDS) 
            ids.Add(id);
        SerializableUserV3 newUser = new SerializableUserV3(ids.ToArray(), new string[ids.Count], PluginHandlers.ClientState.LocalPlayer!.Name.ToString(), (ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id);
        PluginLink.PettableUserHandler.DeclareUser(newUser, PettableUserSystem.Enums.UserDeclareType.Add);
        PluginLink.Configuration.Save();
    }
}

