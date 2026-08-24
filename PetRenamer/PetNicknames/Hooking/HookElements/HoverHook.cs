using Dalamud.Game.Gui;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased.Values;

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
    
    private void OnHoverAction(object? _, HoveredAction? action)
    {
        PetServices.HoverService.SetHoveredPet(null);
        PetServices.HoverService.SetCurrentNameType(NameType.Raw);
        
        if (action == null)
        {
            return;
        }
        
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