using Dalamud.Plugin.Services;

namespace PetRenamer.PetNicknames.Update.Interfaces;

internal interface IUpdatable
{
    bool Enabled { get; }
    
    void OnUpdate(IFramework framework);
}
