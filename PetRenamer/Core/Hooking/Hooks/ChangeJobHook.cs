using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.Core.Hooking.Attributes;
using System;

namespace PetRenamer.Core.Hooking.Hooks;

//[Hook]
internal class ChangeJobHook : HookableElement
{
    private delegate void ChangeJobDelegate(IntPtr data, uint job);

    

    [Signature("88 51 ?? 44 3B CA", DetourName = nameof(ChangeJobDetour))]
    private Hook<ChangeJobDelegate> _changeJobHook = null!;

    internal override void OnInit()
    {
        _changeJobHook?.Enable();

        
    }

    private void ChangeJobDetour(IntPtr data, uint job)
    {
        _changeJobHook.Original(data, job);
    }

    internal override void OnDispose()
    {
        _changeJobHook?.Disable();
        _changeJobHook?.Dispose();
        _changeJobHook = null!;
    }
}
