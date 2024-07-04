using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.Hooking.Interfaces;

internal interface ITextHook
{
    bool Faulty { get; }
    void Setup(DalamudServices service, IPettableUserList userList, IPetServices petServices, string AddonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false);
}
