using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal class PettableUserList : IPettableUserList
{
    public IPettableUser?[] pettableUsers { get; set; } = new IPettableUser[100];

    public IPettablePet? GetPet(nint pet)
    {
        foreach(IPettableUser? user  in pettableUsers)
        {
            if(user == null) continue;
            IPettablePet? pPet = user.GetPet(pet);
            if (pPet == null) continue;
            return pPet;
        }
        return null;
    }
}
