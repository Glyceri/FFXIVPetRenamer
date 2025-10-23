using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetActionWrapper : IPetActionHelper
{
    public nint LastUser  { get; private set; }
    public bool LastValid { get; private set; }

    public void SetLatestUser(nint user, bool valid)
    {
        LastUser  = user;
        LastValid = valid;
    }
}
