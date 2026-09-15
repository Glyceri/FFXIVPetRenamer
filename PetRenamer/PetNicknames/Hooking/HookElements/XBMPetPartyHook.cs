using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

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
        
        if (atkUnitBase->AtkValuesSpan.Length < 6)
        {
            return;
        }
        
        AtkValue amountValue = atkUnitBase->AtkValuesSpan[5];
        
        if (amountValue.Type != AtkValueType.UInt)
        {
            return;
        }
        
        uint   amountOfPets = amountValue.UInt;
        uint[] petIconIds   = new uint[amountOfPets];
        
        for (int i = 0; i < amountOfPets; i++)
        {
            AtkValue value = atkUnitBase->AtkValuesSpan[7 + (i * 77)];
            
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