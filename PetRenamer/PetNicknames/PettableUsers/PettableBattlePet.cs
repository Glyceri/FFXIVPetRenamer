using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableBattlePet : BasePettablePet, IPettableBattlePet
{
    public PettableBattlePet(BattleChara* battlePet, IPettableUser owner, ISharingDictionary sharingDictionary, IPettableDatabaseEntry entry, IPetServices petServices)
        : base(&battlePet->Character, owner, sharingDictionary, entry, petServices, SkeletonType.BattlePet) { }

    public BattleChara* BattlePet
        => (BattleChara*)Address;
}
