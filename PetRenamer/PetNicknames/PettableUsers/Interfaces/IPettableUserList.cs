using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUserList
{
    IPettableUser?[] pettableUsers { get; set; }
}
