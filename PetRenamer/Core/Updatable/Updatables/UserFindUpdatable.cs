using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable(-10)]
internal class UserFindUpdatable : Updatable
{
    const double petResetTime = 1;
    const double resetTime = 5;
    double timer = resetTime;
    double petTimer = petResetTime;

    public override void Update(ref IFramework frameWork, ref PlayerCharacter player)
    {
        timer += frameWork.UpdateDelta.TotalSeconds;
        petTimer += frameWork.UpdateDelta.TotalSeconds;
        bool shouldReset = timer >= resetTime;
        bool petResetTimer = petTimer >= petResetTime;
        if (shouldReset) timer -= resetTime;
        if (petResetTimer) petTimer -= petResetTime;
        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
            PettableUserUtils.instance.Solve(user, shouldReset, petResetTimer);
    }
}