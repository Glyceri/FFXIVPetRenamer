using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class LodestoneQueueHelper(LodestoneNetworker networker, IImageDatabase imageDatabase) : IUpdatable
{
    public bool Enabled
        => true;
    
    public void OnUpdate(IFramework framework)
    {
        imageDatabase.Update();
        networker.Update(framework);
    }
}
