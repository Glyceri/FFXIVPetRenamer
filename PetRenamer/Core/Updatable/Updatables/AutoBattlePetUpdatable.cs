using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable(1)]
internal class AutoBattlePetUpdatable : Updatable
{
    readonly List<int> missingIDs = new List<int>();

    public override void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        missingIDs.Clear();

        foreach (int id in RemapUtils.instance.bakedBattlePetSkeletonToName.Keys)
        {
            bool found = false;
            for (int i = 0; i < user.SerializableUser.length; i++)
            {
                if (user.SerializableUser[i].ID != id) continue;
                found = true;
                break;
            }
            if (!found) missingIDs.Add(id);
        }

        foreach (int id in missingIDs)
            user.SerializableUser.SaveNickname(id, "", true, true);
    }
}
