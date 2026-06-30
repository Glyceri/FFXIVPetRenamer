using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased.Values;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class HoverHook : HookableElement
{
    private uint lastIconId;
    
    private static readonly NameTypeValue HoverNameType = new NameTypeValue()
    { 
        GermanValue  = NameType.Pronoun,
    };
    
    public HoverHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices) { }
    
    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreReceiveEvent, OnEvent);
    }
    
    private void HandlePetSheetData(IPetSheetData? petSheetData)
    {
        if (petSheetData == null)
        {
            PetServices.HoverService.SetHoveredPet(null);
            
            return;
        }
        
        if (PetServices.UserList.LocalPlayer == null)
        {
            return;
        }
        
        petSheetData = PetServices.PetSheets.MakeSoft(PetServices.UserList.LocalPlayer, petSheetData);
        
        NameType setNameType = HoverNameType.GetValue(DalamudServices);

        if (petSheetData.Model.SkeletonType == SkeletonType.BattlePet)
        {
            setNameType = NameType.Action;
        }
        
        PetServices.HoverService.SetCurrentNameType(setNameType);
        PetServices.HoverService.SetHoveredPet(petSheetData);
    }
    
    private unsafe void HandleAsComponentIcon(AtkComponentIcon* componentIcon)
    {
        if (componentIcon == null)
        {
            return;
        }
        
        if (lastIconId == componentIcon->IconId)
        {
            lastIconId = 0;
            
            return;
        }
        
        lastIconId = componentIcon->IconId;

        IPetSheetData? petSheetData = PetServices.PetSheets.GetPetFromIcon(componentIcon->IconId);
      
        HandlePetSheetData(petSheetData);
    }
    
    private unsafe bool HandleAsDragDropNode(AtkResNode* parentNode, AtkComponentBase* componentBase)
    {
        if (componentBase->GetComponentType() != ComponentType.DragDrop)
        {
            return false;
        }

        AtkComponentDragDrop* dragDropComponent = parentNode->GetAsAtkComponentDragDrop();

        if (dragDropComponent == null)
        {
            return false;
        }

        HandleAsComponentIcon(dragDropComponent->AtkComponentIcon);
        
        return true;
    }
    
    private unsafe bool HandleAsListItemRenderedNode(AtkResNode* parentNode, AtkComponentBase* componentBase)
    {
        if (componentBase->GetComponentType() != ComponentType.ListItemRenderer)
        {
            return false;
        }
        
        AtkComponentListItemRenderer* listItemRenderer = parentNode->GetAsAtkComponentListItemRenderer();
        
        if (listItemRenderer == null)
        {
            return false;
        }
        
        AtkResNode* resNode = listItemRenderer->GetNodeById(6);
        
        if (resNode == null)
        {
            return false;
        }
        
        AtkTextNode* atkTextNode = listItemRenderer->GetTextNodeById(6);
        
        if (atkTextNode == null)
        {
            return false;
        }
       
        if (!atkTextNode->OriginalTextPointer.HasValue)
        {
            return false;   
        }
        
        IPetSheetData? petSheetData = PetServices.PetSheets.GetPetFromName(atkTextNode->OriginalTextPointer.AsDalamudSeString().TextValue);
        
        HandlePetSheetData(petSheetData);
        
        return true;
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

        AtkResNode* resNode    = &collisionNode->AtkResNode;
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

        if (HandleAsDragDropNode(parentNode, componentBase))
        {
            return;
        }
        
        _ = HandleAsListItemRenderedNode(parentNode, componentBase);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(AddonEvent.PreReceiveEvent, OnEvent);
    }
}