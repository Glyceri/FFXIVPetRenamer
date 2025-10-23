namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUserList
{
    public IPettableUser?[] PettableUsers { get; }
    public IPettableUser?   LocalPlayer   { get; }

    public IPettablePet?  GetPet(nint pet);
    public IPettableUser? GetUser(nint user, bool petMeansOwner = true);
    public IPettablePet?  GetPet(ulong petId);
    public IPettableUser? GetUser(ulong userId);
    public IPettableUser? GetUserFromObjectId(uint objectId);
    public IPettableUser? GetUserFromOwnerID(uint ownerID);
    public IPettableUser? GetUserFromContentID(ulong contentID);
    public IPettableUser? GetUser(string username);
}
