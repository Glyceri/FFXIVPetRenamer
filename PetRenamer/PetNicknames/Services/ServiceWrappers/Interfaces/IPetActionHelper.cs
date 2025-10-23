namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetActionHelper
{
    public nint LastUser  { get; }
    public bool LastValid { get; }

    public void SetLatestUser(nint user, bool valid);
}
