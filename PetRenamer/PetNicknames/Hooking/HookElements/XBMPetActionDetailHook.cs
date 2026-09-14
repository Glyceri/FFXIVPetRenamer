using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class XBMPetActionDetailHook : HookableElement
{
    public XBMPetActionDetailHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
        { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "XBMPetActionDetail", XBMPetActionPostRefresh);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(XBMPetActionPostRefresh);   
    }
    
    private void XBMPetActionPostRefresh(AddonEvent addonEvent, AddonArgs addonArgs)
    {
        PetServices.HoverService.SetCurrentNameType(NameType.Raw);
        PetServices.HoverService.SetHoveredPet(null);
        
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
        
        AtkTextNode* textNode = atkUnitBase->GetTextNodeById(13);
        
        if (textNode == null)
        {
            return;
        }
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromName(textNode->NodeText.ExtractText());
        
        if (petData == null)
        {
            return;
        }
        
        PetServices.HoverService.SetCurrentNameType(NameType.Raw);
        PetServices.HoverService.SetHoveredPet(petData);
        
        PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowNamesInActivePetColour, textNode, petData, NameType.Raw, localPlayer);
    }
}