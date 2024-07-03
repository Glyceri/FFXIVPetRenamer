using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System.Reflection;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IBattleUser
{
    string Name { get; }
    ulong ContentID { get; }
    ushort Homeworld { get; }
    uint ObjectID { get; }
    BattleChara* BattleChara { get; }
}
