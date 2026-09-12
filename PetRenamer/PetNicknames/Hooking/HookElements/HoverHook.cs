using Dalamud.Game.Gui;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased.Values;
using System;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class HoverHook : HookableElement
{
    private static readonly NameTypeValue HoverNameType = new NameTypeValue()
    { 
        GermanValue  = NameType.Pronoun,
    };
    
    public HoverHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices) { }
    
    public override void Init()
    {
        DalamudServices.GameGui.HoveredActionChanged -= OnHoverAction;
        DalamudServices.GameGui.HoveredActionChanged += OnHoverAction;
    }
    
    protected override void OnDispose()
    {
        DalamudServices.GameGui.HoveredActionChanged -= OnHoverAction;
    }
    
    private bool IsActionHorn(HoveredAction action)
        => PluginConstants.HornActions.Contains(action.ActionId);
    
    private int HornIndex(HoveredAction action)
        => PluginConstants.HornActions.IndexOf(action.ActionId);
    
    private void OnHoverAction(object? _, HoveredAction? action)
    {
        PetServices.HoverService.SetHoveredPet(null);
        PetServices.HoverService.SetCurrentNameType(NameType.Raw);
        
        if (action == null)
        {
            return;
        }
        
        if (IsActionHorn(action))
        {
            HandleActionAsHorn(action);
        }
        else
        {
            HandleActionStandard(action);
        }
    }
    
    private void HandleActionAsHorn(HoveredAction action)
    {
        if (PetServices.UserList.LocalPlayer == null)
        {
            return;
        }
        
        int hornIndex = HornIndex(action);
        
        if (hornIndex < 0)
        {
            return;
        }
        
        XBMPet? pet = PetServices.HornService.GetPetForSlot((byte)hornIndex);
        
        if (pet == null)
        {
            return;
        }
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromIcon(pet.Value.Icon);
        
        if (petData == null)
        {
            return;
        }
        
        // TODO: Probably have to do some sort of softening for beast master pets
        
        PetServices.HoverService.SetHoveredPet(petData);
        PetServices.HoverService.SetCurrentNameType(NameType.Action);
    }
    
    private void HandleActionStandard(HoveredAction action)
    {
        if (PetServices.UserList.LocalPlayer == null)
        {
            return;
        }
        
        IPetSheetData? petData = PetServices.PetSheets.GetPetFromAction(action.ActionId);
        
        if (petData == null)
        {
            return;
        }
        
        IPetSheetData softData = PetServices.PetSheets.MakeSoft(PetServices.UserList.LocalPlayer, petData);
       
        PetServices.HoverService.SetHoveredPet(softData);
        PetServices.HoverService.SetCurrentNameType(HoverNameType.GetValue(DalamudServices));
        
        if (softData.Model.SkeletonType != SkeletonType.BattlePet)
        {
            return;
        }
        
        PetServices.HoverService.SetCurrentNameType(NameType.Action);
    }
}