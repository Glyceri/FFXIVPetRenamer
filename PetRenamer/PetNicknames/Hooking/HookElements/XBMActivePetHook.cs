using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased.Values;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class XBMActivePetHook : HookableElement
{
    private static readonly NameTypeValue ActivePetNameType = new NameTypeValue();
    
    public XBMActivePetHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices)
        { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, "XBMActivePet", XBMActivePetPostRefresh);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(XBMActivePetPostRefresh);
    }
    
    private void XBMActivePetPostRefresh(AddonEvent addonEvent, AddonArgs addonArgs)
    {
        HandleComponentNode((AtkUnitBase*)addonArgs.Addon.Address, 6, 0);
        HandleComponentNode((AtkUnitBase*)addonArgs.Addon.Address, 11, 1);
        HandleComponentNode((AtkUnitBase*)addonArgs.Addon.Address, 16, 2);
    }
    
    private void HandleComponentNode(AtkUnitBase* unitBase, uint nodeId, byte hornIndex)
    {
        if (unitBase == null)
        {
            return;
        }
        
        AtkComponentBase* componentBase = unitBase->GetComponentByNodeId(nodeId);
        
        if (componentBase == null)
        {
            return;
        }
        
        AtkTextNode* textNode = componentBase->GetTextNodeById(13);
        
        if (textNode == null)
        {
            return;
        }
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromName(textNode->NodeText.ExtractText());
        
        if (petData == null)
        {
            return;
        }
        
        if (!PetServices.HornService.TryGetPetForSlot(hornIndex, out XBMPet? hornPet))
        {
            return;
        }
        
        if (hornPet.Value.Icon != petData.Icon)
        {
            return;
        }
        
        PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowNamesInActivePetColour, textNode, petData, ActivePetNameType.GetValue(DalamudServices));
    }
}