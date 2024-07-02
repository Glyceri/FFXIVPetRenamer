using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System.Reflection;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IBattleUser
{
    string Name { get; }
    ulong ContentID { get; }
    ushort Homeworld { get; }
    BattleChara* BattleChara { get; }
}
