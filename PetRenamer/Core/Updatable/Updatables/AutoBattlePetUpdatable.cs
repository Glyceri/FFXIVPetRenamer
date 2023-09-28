using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable(1)]
internal class AutoBattlePetUpdatable : Updatable
{
    public override void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;

        List<int> missingIDs = new List<int>();

        foreach(int id in PluginConstants.allowedNegativePetIDS)
        {
            bool found = false;
            user.SerializableUser.LoopThroughBreakable(nickname => {
                if (nickname.Item1 == id) 
                { 
                    found = true; 
                    return true; 
                }
                return false; 
            });
            if(!found)
            missingIDs.Add(id);
        }

        foreach (int id in missingIDs)
            user.SerializableUser.SaveNickname(id, "", true, false, true);
    }
}
