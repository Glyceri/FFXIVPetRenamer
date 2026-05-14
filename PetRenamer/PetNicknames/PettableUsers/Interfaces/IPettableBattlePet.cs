using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IPettableBattlePet : IPettablePet
{
    BattleChara* BattlePet { get; }
}
