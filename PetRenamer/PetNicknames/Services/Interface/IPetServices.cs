using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.Interface;

internal interface IPetServices
{
    public IPetLog PetLog { get; }
    public IPetSheets PetSheets { get; }
    public IStringHelper StringHelper { get; }
    public Configuration Configuration { get; }
}
