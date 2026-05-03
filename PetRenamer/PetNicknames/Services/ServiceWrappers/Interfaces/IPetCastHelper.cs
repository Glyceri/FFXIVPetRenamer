using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetCastHelper
{
    IPettableEntity? LastCastDealer { get; }
    int              LastCastId     { get; }

    void SetLatestCast(nint target, nint dealer, int lastCastId);
}
