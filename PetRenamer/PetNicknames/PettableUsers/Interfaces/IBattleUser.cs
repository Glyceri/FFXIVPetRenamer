using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IBattleUser : IPettableEntity
{
    string Name { get; }
    ulong ContentID { get; }
    ushort Homeworld { get; }
    ulong ObjectID { get; }
    uint ShortObjectID { get; } 
    uint CurrentCastID { get; }

    BattleChara* BattleChara { get; }
}
