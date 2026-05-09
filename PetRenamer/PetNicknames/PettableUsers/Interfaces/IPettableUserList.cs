namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUserList
{
    IPettableUser?[] PettableUsers { get; }
    IPettableUser?   LocalPlayer   { get; }

    IPettablePet?  GetPet(nint pet);
    IPettableUser? GetUser(nint user, bool petMeansOwner = true);
    IPettablePet?  GetPet(ulong petId);
    IPettableUser? GetUser(ulong userId);
    IPettableUser? GetUserFromObjectId(uint objectId);
    IPettableUser? GetUserFromOwnerId(uint ownerId);
    IPettableUser? GetUserFromContentId(ulong contentId);
    IPettableUser? GetUserFromEntityId(uint entityId);
    IPettableUser? GetUser(string username);
}
