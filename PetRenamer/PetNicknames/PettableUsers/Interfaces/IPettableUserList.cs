using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.PettableUsers.Enums;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUserList
{
    IPettableUser?[] PettableUsers { get; }
    IPettableUser?   LocalPlayer   { get; }

    IPettablePet?  GetPet(nint pet);
    IPettablePet?  GetPet(GameObjectId petId);
    
    IPettableUser? GetUser(nint user, UserListFindType findType);
    IPettableUser? GetUserFromObjectId(GameObjectId objectId);
    IPettableUser? GetUserFromContentId(ulong contentId);
    IPettableUser? GetUser(string username);
    
    void Recalculate();
}
