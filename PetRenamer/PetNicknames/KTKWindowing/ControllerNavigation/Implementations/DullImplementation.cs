using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Implementations;

internal class DullImplementation : ControllerNavigation
{
    public DullImplementation(IPetServices petServices) 
        : base(petServices) { }

    public override bool OnCustomInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState)
        => false;
}
