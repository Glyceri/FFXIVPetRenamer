using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services;

internal class PetServices
{
    public IPetLog PetLog { get; init; }

    public PetServices(DalamudServices services) 
    {
        PetLog = new PetLogWrapper(services.PluginLog);
    }
}
