using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IBattleUser : IPettableEntity
{
    string       Name          { get; }
    ulong        ContentId     { get; }
    ushort       Homeworld     { get; }
    ulong        ObjectId      { get; }
    uint         EntityId      { get; }
    uint         ShortObjectId { get; }
    uint         CurrentCastId { get; }
    BattleChara* BattleChara   { get; }
}
