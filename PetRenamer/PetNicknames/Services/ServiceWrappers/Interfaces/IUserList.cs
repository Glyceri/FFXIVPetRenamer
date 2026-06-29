using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.PettableUsers.Enums;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IUserList : IEnumerable<IPettableUser?>
{
    const int PettableUserArraySize = 101;
    const int IslandIndex           = 100;
    
    IPettableUser? this[int index] { get; set; }
    
    IPettableUser? LocalPlayer { get; }

    IPettablePet?  GetPet(nint pet);
    IPettablePet?  GetPet(GameObjectId petId);
    
    IPettableUser? GetUser(nint user, UserListFindType findType);
    IPettableUser? GetUserFromObjectId(GameObjectId objectId);
    IPettableUser? GetUserFromContentId(ulong contentId);
    IPettableUser? GetUser(string username, uint homeworld);
    
    void Recalculate();
}
