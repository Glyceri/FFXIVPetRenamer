using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableBattlePet : BasePettablePet, IPettableBattlePet
{
    public BattleChara* BattlePet { get => (BattleChara*)Address; }

    public PettableBattlePet(BattleChara* battlePet, IPettableUser owner, ISharingDictionary sharingDictionary, IPettableDatabaseEntry entry, IPetServices petServices) : base(&battlePet->Character, owner, sharingDictionary, entry, petServices, true) { }
}
