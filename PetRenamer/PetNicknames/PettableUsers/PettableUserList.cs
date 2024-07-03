using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal class PettableUserList : IPettableUserList
{
    public IPettableUser?[] pettableUsers { get; set; } = new IPettableUser[100];
}
