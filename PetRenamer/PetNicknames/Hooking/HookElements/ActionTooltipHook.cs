using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookTypes;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal  class ActionTooltipHook : QuickHookableElement, IActionTooltipHook
{
    public IPetSheetData? CurrentlyHoveredPet { get; private set; }

    private readonly ActionTooltipTextHook TooltipTextHook;
    private readonly ActionTooltipTextHook ActionTooltipTextHook;

    public ActionTooltipHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) 
        : base(services, petServices, userList, dirtyListener) 
    {
        TooltipTextHook = Hook<ActionTooltipTextHook>("Tooltip", [2], Allowed, false, true);
        TooltipTextHook.Register(3);

        ActionTooltipTextHook = Hook<ActionTooltipTextHook>("ActionDetail", [5], Allowed, false, true);
        ActionTooltipTextHook.Register();
    }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreReceiveEvent, OnEvent);
    }

    private bool Allowed(PetSkeleton id)
        => PetServices.Configuration.showOnTooltip;

    private unsafe void OnEvent(AddonEvent type, AddonArgs args)
    {
        CurrentlyHoveredPet = null;

        AtkStage* atkStage = AtkStage.Instance();

        if (atkStage == null)
        {
            return;
        }

        AtkCollisionManager* collisionManager = AtkStage.Instance()->AtkCollisionManager;

        if (collisionManager == null)
        {
            return;
        }

        AtkCollisionNode* collisionNode = collisionManager->IntersectingCollisionNode;

        if (collisionNode == null)
        {
            return;
        }

        AtkResNode* resNode = &collisionNode->AtkResNode;

        if (resNode == null)
        {
            return;
        }

        AtkResNode* parentNode = resNode->ParentNode;

        if (parentNode == null)
        {
            return;
        }

        if (parentNode->GetNodeType() != NodeType.Component)
        {
            return;
        }

        AtkComponentNode* componentNode = parentNode->GetAsAtkComponentNode();

        if (componentNode == null)
        {
            return;
        }

        AtkComponentBase* componentBase = componentNode->Component;

        if (componentBase == null)
        {
            return;
        }

        if (componentBase->GetComponentType() != ComponentType.DragDrop)
        {
            return;
        }

        AtkComponentDragDrop* dragDropComponent = parentNode->GetAsAtkComponentDragDrop();

        if (dragDropComponent == null)
        {
            return;
        }

        AtkComponentIcon* componentIcon = dragDropComponent->AtkComponentIcon;

        if (componentIcon == null)
        {
            return;
        }

        IPetSheetData? petSheetData = PetServices.PetSheets.GetPetFromIcon(componentIcon->IconId);

        if (petSheetData == null)
        {
            return;
        }

        CurrentlyHoveredPet = petSheetData;

        TooltipTextHook.SetPetSheetData(CurrentlyHoveredPet);
        ActionTooltipTextHook.SetPetSheetData(CurrentlyHoveredPet);
    }

    protected override void OnQuickDispose()
    {
        TooltipTextHook?.Dispose();
        ActionTooltipTextHook?.Dispose();

        DalamudServices.AddonLifecycle.UnregisterListener(AddonEvent.PreReceiveEvent, OnEvent);
    }
}
