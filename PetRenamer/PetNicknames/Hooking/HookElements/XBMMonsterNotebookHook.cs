using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class XBMMonsterNotebookHook : HookableElement
{
    private uint _hoveredIndex = 0;
    
    public XBMMonsterNotebookHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
        { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PreRefresh,  "XBMMonsterNotebook",   XBMMonsterNotebookPostRefresh);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, "XBMMonsterBookDetail", XBMMonsterBookDetailPostRefresh);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(XBMMonsterNotebookPostRefresh);   
        DalamudServices.AddonLifecycle.UnregisterListener(XBMMonsterBookDetailPostRefresh);   
    }
    
    private void XBMMonsterNotebookPostRefresh(AddonEvent addonEvent, AddonArgs args)
    {
        _hoveredIndex = 0;
        
        AtkUnitBase* atkUnitBase = (AtkUnitBase*)args.Addon.Address;
        
        if (atkUnitBase == null)
        {
            return;
        }
        
        if (atkUnitBase->AtkValuesSpan.Length < 6)
        {
            return;
        }
        
        AtkValue atkValue = atkUnitBase->AtkValuesSpan[5];
        
        if (atkValue.Type != AtkValueType.UInt)
        {
            return;
        }
        
        _hoveredIndex = atkValue.UInt;
    }
    
    private void XBMMonsterBookDetailPostRefresh(AddonEvent addonEvent, AddonArgs args)
    {
        AtkUnitBase* atkUnitBase = (AtkUnitBase*)args.Addon.Address;
        
        if (atkUnitBase == null)
        {
            return;
        }
     
        IPettableUser? localPlayer = PetServices.UserList.LocalPlayer;
        
        if (localPlayer == null)
        {
            return;
        }
        
        XBMPet? pet = PetServices.PetSheets.GetSheetXBMPet(_hoveredIndex);
        
        if (pet == null)
        {
            return;
        }
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromIcon(pet.Value.Icon);
        
        if (petData == null)
        {
            return;
        }
        
        AtkTextNode* textNode = atkUnitBase->GetTextNodeById(13);
        
        if (textNode == null)
        {
            return;
        }
        
        PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowNamesInActivePetColour, textNode, petData, NameType.Raw, localPlayer, allowOriginalPointer: false);
    }
}