using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetActionHelper
{
    IPettableUser? LastUser  { get; }
    bool           LastValid { get; }

    void SetLatestUser(nint user, bool valid);
}
