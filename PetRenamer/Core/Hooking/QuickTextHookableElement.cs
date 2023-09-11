using Dalamud.Game;
using PetRenamer.Core.Hooking.Hooks.InternalHooks;
using PetRenamer.Core.PettableUserSystem;
using System;
using System.Collections.Generic;

namespace PetRenamer.Core.Hooking;

public class QuickTextHookableElement : HookableElement
{
    internal override sealed void OnDispose() 
    { 
        OnQuickDispose(); 
        foreach(QuickTextReplaceHook el in quickTextReplaceHooks)
            el?.Dispose();
    }

    internal override sealed void OnInit() 
    { 
        OnQuickInit(); 
    }

    internal override void OnUpdate(Framework framework) { }

    protected void OnBaseUpdate(Framework framework, bool allow)
    {
        foreach (QuickTextReplaceHook el in quickTextReplaceHooks)
            el?.OnUpdate(framework, allow);
    }

    internal virtual void OnQuickDispose() { }

    internal virtual void OnQuickInit() { }

    readonly List<QuickTextReplaceHook> quickTextReplaceHooks = new List<QuickTextReplaceHook>();

    protected void RegisterHook(string addonName, uint atkTextID, Func<int, bool> allowedToFunction, int atkBackgroundID = -1, Func<PettableUser> pettableUserFunc = null!) 
        => quickTextReplaceHooks.Add(new QuickTextReplaceHook(addonName, atkTextID, allowedToFunction, atkBackgroundID, pettableUserFunc));
    
}
