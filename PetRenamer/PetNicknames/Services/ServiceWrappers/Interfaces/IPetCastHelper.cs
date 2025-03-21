namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IPetCastHelper
{
    nint LastCastTarget { get; }
    nint LastCastDealer { get; }
    int LastCastID { get; }

    void SetLatestCast(nint target, nint dealer, int lastCastID);
}
