using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.Hooking;

internal abstract class QuickHookableElement : HookableElement
{
    public QuickHookableElement(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, userList, petServices) { }

    public T Hook<T>(string addonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false) where T : ITextHook, new()
    {
        T t = new T();
        t.Setup(DalamudServices, UserList, PetServices, addonName, textPos, allowedCallback, isSoft);
        if (t.GetType() == typeof(SimpleTextHook)) (t as SimpleTextHook)!.SetUnfaulty();
        return t;
    }
}
