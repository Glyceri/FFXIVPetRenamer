namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUserList
{
    IPettableUser?[] PettableUsers { get; }
    IPettableUser? LocalPlayer { get; }

    IPettablePet? GetPet(nint pet);
    IPettableUser? GetUser(nint user);
    IPettablePet? GetPet(ulong petId);
    IPettableUser? GetUser(ulong userId);
    IPettableUser? GetUserFromOwnerID(uint ownerID);
    IPettableUser? GetUserFromContentID(ulong contentID);
    IPettableUser? GetUser(string username);
}
