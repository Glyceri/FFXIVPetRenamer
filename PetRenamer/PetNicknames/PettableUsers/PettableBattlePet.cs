using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableBattlePet : BasePettablePet, IPettableBattlePet
{
    public BattleChara* BattlePet { get => (BattleChara*)PetPointer; }

    public PettableBattlePet(BattleChara* battlePet, IPettableUser owner, IPettableDatabaseEntry entry, IPetServices petServices) : base(&battlePet->Character, owner, entry, petServices, true)
    {
        
    }
}
