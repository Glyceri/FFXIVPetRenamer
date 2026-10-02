using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class XBMPetPartyHook : HookableElement
{
    public XBMPetPartyHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
        { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "XBMPetParty", XBMPresetPreviewPostRefresh);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(XBMPresetPreviewPostRefresh);   
    }
    
    private void XBMPresetPreviewPostRefresh(AddonEvent addonEvent, AddonArgs addonArgs)
    {
        AtkUnitBase* atkUnitBase = (AtkUnitBase*)addonArgs.Addon.Address;
        
        if (atkUnitBase == null)
        {
            return;
        }
     
        IPettableUser? localPlayer = PetServices.UserList.LocalPlayer;
        
        if (localPlayer == null)
        {
            return;
        }
        
        Span<AtkValue> spanArray  = atkUnitBase->AtkValuesSpan;
        int            spanLength = spanArray.Length;
        
        if (spanLength < 6)
        {
            return;
        }
        
        AtkValue amountValue = spanArray[5];
        
        if (amountValue.Type != AtkValueType.UInt)
        {
            return;
        }
        
        uint   amountOfPets = amountValue.UInt;
        uint[] petIconIds   = new uint[amountOfPets];
        
        for (int i = 0; i < amountOfPets; i++)
        {
            if (i >= amountOfPets)
            {
                break;
            }
            
            int index = (7 + (i * 77));
            
            if (index >= spanLength)
            {
                break;
            }
            
            AtkValue value = spanArray[index];
            
            if (value.Type != AtkValueType.UInt)
            {
                continue;
            }
            
            petIconIds[i] = value.UInt;
        }
        
        AtkComponentList* componentList = atkUnitBase->GetComponentListById(11);
        
        if (componentList == null)
        {
            return;
        }
        
        for (int i = 0; i < componentList->AllocatedItemRendererListLength; i++)
        {
            if (i >= amountOfPets)
            {
                break;
            }
            
            uint iconId = petIconIds[i];
            
            if (iconId == 0)
            {
                continue;
            }
            
            IPetSheetData? petData = PetServices.PetSheets.GetPetFromIcon(iconId);
            
            if (petData == null)
            {
                continue;
            }
            
            AtkComponentListItemRenderer* itemRenderer = componentList->GetItemRenderer(i);
            
            if (itemRenderer == null)
            {
                continue;
            }
            
            AtkTextNode* textNode = itemRenderer->GetTextNodeById(17);
            
            if (textNode == null)
            {
                continue;
            }
            
            PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowNamesInActivePetColour, textNode, petData, NameType.Raw, localPlayer, false);
        }
    }
}