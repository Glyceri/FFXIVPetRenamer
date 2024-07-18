using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableBattlePet : BasePettablePet, IPettableBattlePet
{
    public BattleChara* BattlePet { get => (BattleChara*)PetPointer; }

    public PettableBattlePet(BattleChara* battlePet, in IPettableUser owner, in ISharingDictionary sharingDictionary, in IPettableDatabaseEntry entry, in IPetServices petServices) : base(&battlePet->Character, in owner, in sharingDictionary, in entry, in petServices, true)
    {
        
    }
}
