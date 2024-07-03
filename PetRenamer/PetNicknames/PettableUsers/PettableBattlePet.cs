using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableBattlePet : IPettableBattlePet
{
    public BattleChara* BattlePet { get; init; }

    public PettableBattlePet(BattleChara* battlePet)
    {
        BattlePet = battlePet;
    }
}
