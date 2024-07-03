namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUserList
{
    IPettableUser?[] pettableUsers { get; set; }

    IPettablePet? GetPet(nint pet);
}
