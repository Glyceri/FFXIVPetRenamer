using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IPettableBattleEntity : IPettableEntity
{
    BattleChara* BattleChara { get; }
}