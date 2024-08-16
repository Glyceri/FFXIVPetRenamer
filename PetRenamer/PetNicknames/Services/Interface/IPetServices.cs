using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.Interface;

internal interface IPetServices
{
    public IPetLog PetLog { get; }
    public IPetSheets PetSheets { get; }
    public IStringHelper StringHelper { get; }
    public IPetCastHelper PetCastHelper { get; }
    public IPetActionHelper PetActionHelper { get; }
    public Configuration Configuration { get; }
}
