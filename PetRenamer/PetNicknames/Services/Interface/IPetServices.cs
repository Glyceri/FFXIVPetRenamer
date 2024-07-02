using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.Interface;

internal interface IPetServices
{
    public IPetLog PetLog { get; }
    public Configuration Configuration { get; }
}
