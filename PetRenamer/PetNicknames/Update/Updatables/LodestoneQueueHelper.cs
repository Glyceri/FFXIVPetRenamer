using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Lodestone;
using PetRenamer.PetNicknames.Update.Interfaces;

namespace PetRenamer.PetNicknames.Update.Updatables;

internal class LodestoneQueueHelper(in LodestoneNetworker networker) : IUpdatable
{
    public bool Enabled { get; set; } = true;

    readonly LodestoneNetworker Networker = networker;

    public void OnUpdate(IFramework framework) => Networker.Update(framework);
}
