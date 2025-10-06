using Dalamud.Plugin.Services;

namespace PetRenamer.PetNicknames.Update.Interfaces;

internal interface IUpdatable
{
    public bool Enabled { get; set; }
    public void OnUpdate(IFramework framework);
}
