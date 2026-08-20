using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class TextElementHook : HookableElement
{
    private readonly Hook<AtkTextNode.Delegates.SetText> SetTextHook;
    private readonly Hook<AtkTextNode.Delegates.Destroy> DestroyTextHook;
    
    public TextElementHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
    {
        SetTextHook     = DalamudServices.Hooking.HookFromAddress<AtkTextNode.Delegates.SetText>((nint)AtkTextNode.MemberFunctionPointers.SetText,     SetTextDetour);
        DestroyTextHook = DalamudServices.Hooking.HookFromAddress<AtkTextNode.Delegates.Destroy>((nint)AtkTextNode.StaticVirtualTablePointer->Destroy, DestroyTextDetour);
    }

    public override void Init()
    {
        SetTextHook.Enable();
        DestroyTextHook.Enable();
    }

    protected override void OnDispose()
    {
        SetTextHook.Dispose();    
        DestroyTextHook.Dispose();
    }
    
    private void DestroyTextDetour(AtkTextNode* textNode, bool isFree)
    {
        DestroyTextHook.OriginalDisposeSafe(textNode, isFree);
        
        PetServices.StringHelper.Remove((nint)textNode);
    }
    
    private void SetTextDetour(AtkTextNode* textNode, CStringPointer stringPtr)
    {
        if (!PetServices.StringHelper.OurReplace)
        {
            PetServices.StringHelper.Remove((nint)textNode);
        }
        else if (!PetServices.StringHelper.Add((nint)textNode))
        {
            return;
        }
        
        SetTextHook.OriginalDisposeSafe(textNode, stringPtr);
    }
}