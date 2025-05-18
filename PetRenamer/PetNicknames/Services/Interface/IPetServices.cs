using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.Interface;

internal interface IPetServices
{
    IPetLog          PetLog          { get; }
    IPetSheets       PetSheets       { get; }
    IStringHelper    StringHelper    { get; }
    IPetCastHelper   PetCastHelper   { get; }
    IPetActionHelper PetActionHelper { get; }
    Configuration    Configuration   { get; }
    ITargetManager   TargetManager   { get; }
}
