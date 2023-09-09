using Dalamud.Game;
using PetRenamer.Core.Hooking.Hooks.InternalHooks;
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

    List<QuickTextReplaceHook> quickTextReplaceHooks = new List<QuickTextReplaceHook>();

    protected void RegisterHook(string addonName, uint atkTextID, int atkBackgroundID) 
        => quickTextReplaceHooks.Add(new QuickTextReplaceHook(addonName, atkTextID, atkBackgroundID));
    
}
