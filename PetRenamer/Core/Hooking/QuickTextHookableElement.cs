using Dalamud.Plugin.Services;
using PetRenamer.Core.Hooking.Hooks.InternalHooks;
using PetRenamer.Core.PettableUserSystem;
using System;
using System.Collections.Generic;

namespace PetRenamer.Core.Hooking;

public class QuickTextHookableElement : HookableElement
{
    internal override sealed void OnInit() 
    { 
        OnQuickInit(); 
    }

    internal virtual void OnQuickDispose() { }

    internal virtual void OnQuickInit() { }

    readonly List<QuickTextReplaceHook> quickTextReplaceHooks = new List<QuickTextReplaceHook>();

    protected void RegisterHook(string addonName, uint atkTextID, Func<int, bool> allowedToFunction = null!, int atkBackgroundID = -1, Func<PettableUser> pettableUserFunc = null!, Action<string> latestOutcome = null!) 
        => quickTextReplaceHooks.Add(new QuickTextReplaceHook(addonName, atkTextID, allowedToFunction, atkBackgroundID, pettableUserFunc, latestOutcome));

    protected void RegisterHook(string addonName, uint[] atkTextIDs, Func<int, bool> allowedToFunction = null!, int atkBackgroundID = -1, Func<PettableUser> pettableUserFunc = null!, Action<string> latestOutcome = null!)
        => quickTextReplaceHooks.Add(new QuickTextReplaceHook(addonName, atkTextIDs, allowedToFunction, atkBackgroundID, pettableUserFunc, latestOutcome));
}
