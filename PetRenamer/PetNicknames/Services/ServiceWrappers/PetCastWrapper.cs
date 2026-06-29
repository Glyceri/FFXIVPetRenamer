using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetCastWrapper : IPetCastHelper
{
    public IPettableEntity? LastCastDealer { get; private set; }
    public int              LastCastId     { get; private set; }

    private readonly IPetServices PetServices;
    
    public PetCastWrapper(IPetServices petServices) 
        => PetServices = petServices;
        
    public void SetLatestCast(nint target, nint dealer, int lastCastId)
    {
        if (PetServices.Configuration.debugModeActive)
        {
            PetServices.PetLog.Log($"Setting latest cast for: {dealer}, {target}, {lastCastId}");
        }
        
        LastCastDealer   = PetServices.UserList.GetUser(dealer, UserListFindType.Direct);
        LastCastDealer ??= PetServices.UserList.GetPet(dealer);
        LastCastId       = lastCastId;
    }
}
