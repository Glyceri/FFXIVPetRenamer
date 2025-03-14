namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetActionHelper
{
    nint LastUser { get; }
    bool LastValid { get; }

    void SetLatestUser(nint user, bool valid);
}
