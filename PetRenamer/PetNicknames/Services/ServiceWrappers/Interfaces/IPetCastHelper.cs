namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IPetCastHelper
{
    public nint LastCastTarget { get; }
    public nint LastCastDealer { get; }
    public int  LastCastID     { get; }

    public void SetLatestCast(nint target, nint dealer, int lastCastID);
}
