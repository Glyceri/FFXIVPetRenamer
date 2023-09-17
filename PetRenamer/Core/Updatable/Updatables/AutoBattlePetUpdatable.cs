using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable(1)]
internal class AutoBattlePetUpdatable : Updatable
{
    public override void Update(Framework frameWork)
    {
        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;

        foreach(int id in PluginConstants.allowedNegativePetIDS)
            if(!user.SerializableUser.HasID(id))
                user.SerializableUser.SaveNickname(id, "", true, false, true);
    }
}
