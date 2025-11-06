using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Struct;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Implementations;

internal unsafe class BasicControllerNavigation : ControllerNavigation
{
    public BasicControllerNavigation(IPetServices petServices) 
        : base(petServices) { }

    public override bool OnCustomInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
    {
        CursorNavigationInfo? focusNavigationInfo = GetFocusedNavigationInfo();

        if (focusNavigationInfo == null)
        {
            return false;
        }

        if (IsNavigationBlocked(inputId, inputState, focusNavigationInfo.Value))
        {
            return false;
        }

        byte nextIndex = GetNextIndex(inputId, inputState, focusNavigationInfo.Value);

        if (!NextIndexIsValid(nextIndex))
        {
            return false;
        }

        NavigableComponent? nextComponent = GetComponentAtIndex(nextIndex);

        if (nextComponent == null)
        {
            return false;
        }

        return MoveFocus(nextComponent);
    }
}
