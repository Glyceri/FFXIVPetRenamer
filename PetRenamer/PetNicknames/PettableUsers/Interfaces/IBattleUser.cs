using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IBattleUser
{
    string Name { get; }
    ulong ContentID { get; }
    ushort Homeworld { get; }
    ulong ObjectID { get; }
    uint ShortObjectID { get; } 
    uint CurrentCastID { get; }

    nint Address { get; }
    BattleChara* BattleChara { get; }
}
