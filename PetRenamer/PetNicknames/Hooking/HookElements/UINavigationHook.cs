using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class UINavigationHook : HookableElement
{
    private readonly IPettableDirtyCaller DirtyCaller;

    private readonly Hook<AtkUnitBase.Delegates.HandleCustomInput> HandleCustomInputHook;

    public UINavigationHook(DalamudServices services, IPetServices petServices, IPettableUserList userList,  IPettableDirtyListener dirtyListener, IPettableDirtyCaller dirtyCaller) 
        : base(services, userList, petServices, dirtyListener)
    {
        DirtyCaller           = dirtyCaller;

        HandleCustomInputHook = DalamudServices.Hooking.HookFromAddress<AtkUnitBase.Delegates.HandleCustomInput>((nint)AtkUnitBase.StaticVirtualTablePointer->HandleCustomInput, HandleCustomInputDetour);
    }

    public override void Init()
    {
        HandleCustomInputHook?.Enable();    
    }

    private bool HandleCustomInputDetour(AtkUnitBase* thisPtr, AtkEventData.AtkInputData* inputData)
    {
        bool returner = HandleCustomInputHook!.OriginalDisposeSafe(thisPtr, inputData);

        if (inputData != null)
        {
            NavigationInputId navigationInput = (NavigationInputId)inputData->InputId;

            returner |= DirtyCaller.DirtyNavigationInput((nint)thisPtr, navigationInput, inputData->State);
        }

        return returner;
    }

    protected override void OnDispose()
    {
        HandleCustomInputHook?.Dispose();
    }
}
