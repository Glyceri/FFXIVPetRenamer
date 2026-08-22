using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe abstract class PettableBattlePet : BasePettablePet, IPettableBattlePet
{
    public PettableBattlePet(BattleChara* battlePet, IPettableUser owner, ISharingDictionary sharingDictionary, IPettableDatabaseEntry entry, IPetServices petServices, SkeletonType skeletonType)
        : base(&battlePet->Character, owner, sharingDictionary, entry, petServices, skeletonType) { }
    
    public BattleChara* BattleChara
        => (BattleChara*)Address;
}
