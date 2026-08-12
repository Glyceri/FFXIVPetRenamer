using Dalamud.Hooking;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using DalamudSeString = Dalamud.Game.Text.SeStringHandling.SeString;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class PronounHook : HookableElement, IPronounHook
{
    public DalamudSeString? LastGottenPronoun         { get; private set; }
    public DalamudSeString? PreviousLastGottenPronoun { get; private set; }
    
    private delegate uint ProcessNounDelegate(RaptureTextModule* self, byte *a2, int a3, ulong a4, char a5, int a6, Utf8String* outputString);
    
    [Signature("40 55 53 56 57 41 54 41 56 41 57 48 8D AC 24 ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 ?? ?? ?? ?? 48 8B 85 ?? ?? ?? ?? 48 8B F9", DetourName = nameof(ProcessNounDetour))]
    private readonly Hook<ProcessNounDelegate>? ProcessNounHook = null!;
    
    public PronounHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
    {
        PetServices.NameService.RegisterPronounHook(this);
    }

    public override void Init()
    {
        ProcessNounHook?.Enable();
    }
    
    private uint ProcessNounDetour(RaptureTextModule* self, byte* a2, int a3, ulong a4, char a5, int a6, Utf8String* outputString)
    {
        uint returner = ProcessNounHook!.Original(self, a2, a3, a4, a5, a6, outputString);
        
        PetServices.PetLog.DevLogInfo($"ProcessNounDetour: {outputString->ToString()}");
        
        PreviousLastGottenPronoun = LastGottenPronoun;
        LastGottenPronoun         = outputString->StringPtr.AsDalamudSeString();

        return returner;
    }

    protected override void OnDispose()
    {
        ProcessNounHook?.Dispose();
    }
}