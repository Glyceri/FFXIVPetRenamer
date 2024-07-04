using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System.Reflection;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IBattleUser
{
    string Name { get; }
    ulong ContentID { get; }
    ushort Homeworld { get; }
    ulong ObjectID { get; }
    uint ShortObjectID { get; } 
    BattleChara* BattleChara { get; }
}
