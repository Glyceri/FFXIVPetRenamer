using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class LodestoneQueueHelper(in LodestoneNetworker networker, in IImageDatabase imageDatabase) : IUpdatable
{
    public bool Enabled { get; set; } = true;

    readonly LodestoneNetworker Networker = networker;
    readonly IImageDatabase ImageDatabase = imageDatabase;

    public void OnUpdate(IFramework framework)
    {
        ImageDatabase.Update();
        Networker.Update(framework);
    }
}
