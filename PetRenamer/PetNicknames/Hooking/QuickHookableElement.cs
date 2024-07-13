using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking;

internal abstract class QuickHookableElement : HookableElement
{
    public QuickHookableElement(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, userList, petServices) { }

    List<ITextHook> textHooks = new List<ITextHook>();

    public T Hook<T>(string addonName, uint[] textPos, Func<int, bool> allowedCallback, bool isSoft = false) where T : ITextHook, new()
    {
        T t = new T();
        t.Setup(DalamudServices, UserList, PetServices, addonName, textPos, allowedCallback, isSoft);
        // Cant use the [t is SimpleTextHook tHook] because it can only run this code if it is of the ACTUAL type SimpleTextHook.
        // Not any inherited type
        if (t.GetType() == typeof(SimpleTextHook))
        {
            (t as SimpleTextHook)!.SetUnfaulty();
        }
        textHooks.Add(t);
        return t;
    }

    public sealed override void Dispose()
    {
        OnDispose();

        foreach(ITextHook hook in textHooks)
        {
            hook.Dispose();
        }    

        textHooks.Clear();
    }

    public virtual void OnDispose() { }
}
