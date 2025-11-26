using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetCastWrapper : IPetCastHelper
{
    public nint LastCastTarget { get; private set; }
    public nint LastCastDealer { get; private set; }
    public int  LastCastID     { get; private set; }

    public void SetLatestCast(nint target, nint dealer, int lastCastID)
    {
        LastCastTarget = target;
        LastCastDealer = dealer;
        LastCastID     = lastCastID;
    }
}
