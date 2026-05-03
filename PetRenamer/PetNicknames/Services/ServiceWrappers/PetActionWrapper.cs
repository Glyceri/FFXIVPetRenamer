using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetActionWrapper : IPetActionHelper
{
    public IPettableUser? LastUser  { get; private set; }
    public bool           LastValid { get; private set; }

    private readonly IPettableUserList UserList;
    
    public PetActionWrapper(IPettableUserList userList)
        => UserList = userList;    
    
    public void SetLatestUser(nint user, bool valid)
    {
        LastUser  = null;
        LastValid = valid;
        LastUser  = UserList.GetUser(user);
        
        if (!valid)
        {
            return;
        }
        
        LastValid = (LastUser != null);
    }
}
