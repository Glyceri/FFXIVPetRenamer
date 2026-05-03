using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class HoverHook : HookableElement
{
    private uint lastIconId = 0;
    
    public HoverHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) 
        : base(services, userList, petServices, dirtyListener) { }
    
    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreReceiveEvent, OnEvent);
    }
    
    private unsafe void HandleAsComponentIcon(AtkComponentIcon* componentIcon)
    {
        if (componentIcon == null)
        {
            return;
        }
        
        if (lastIconId == componentIcon->IconId)
        {
            return;
        }
        
        lastIconId =  componentIcon->IconId;

        IPetSheetData? petSheetData = PetServices.PetSheets.GetPetFromIcon(componentIcon->IconId);

        if (petSheetData == null)
        {
            PetServices.HoverService.SetHoveredPet(null);
            
            return;
        }
        
        NameType setNameType = NameType.Pronoun;

        if (petSheetData.Model.SkeletonType == SkeletonType.BattlePet)
        {
            setNameType = NameType.Action;
        }
        
        PetServices.HoverService.SetCurrentNameType(setNameType);
        PetServices.HoverService.SetHoveredPet(petSheetData);
    }
    
    private unsafe void HandleAsDragDropNode(AtkResNode* parentNode, AtkComponentBase* componentBase)
    {
        if (componentBase->GetComponentType() != ComponentType.DragDrop)
        {
            return;
        }

        AtkComponentDragDrop* dragDropComponent = parentNode->GetAsAtkComponentDragDrop();

        if (dragDropComponent == null)
        {
            return;
        }

        HandleAsComponentIcon(dragDropComponent->AtkComponentIcon);
    }
    
    private unsafe void OnEvent(AddonEvent type, AddonArgs args)
    {
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

        HandleAsDragDropNode(parentNode, componentBase);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(AddonEvent.PreReceiveEvent, OnEvent);
    }
}