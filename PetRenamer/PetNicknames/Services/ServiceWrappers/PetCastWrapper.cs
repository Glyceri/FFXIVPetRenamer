using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetCastWrapper : IPetCastHelper
{
    public IPettableEntity? LastCastDealer { get; private set; }
    public int              LastCastId     { get; private set; }

    private readonly IPettableUserList UserList;
    
    public PetCastWrapper(IPettableUserList userList) 
        => UserList = userList;
        
    public void SetLatestCast(nint target, nint dealer, int lastCastId)
    {
        LastCastDealer   = UserList.GetUser(dealer, false);
        LastCastDealer ??= UserList.GetPet(dealer);
        LastCastId       = lastCastId;
    }
}
