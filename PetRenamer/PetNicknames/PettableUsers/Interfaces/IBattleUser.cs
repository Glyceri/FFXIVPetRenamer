using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IBattleUser : IPettableEntity
{
    string       Name          { get; }
    ulong        ContentId     { get; }
    ushort       Homeworld     { get; }
    GameObjectId ObjectId      { get; }
    uint         CurrentCastId { get; }
    BattleChara* BattleChara   { get; }
}
