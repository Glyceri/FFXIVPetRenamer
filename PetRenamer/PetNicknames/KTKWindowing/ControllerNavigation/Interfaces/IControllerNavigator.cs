using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.KTKWindowing.Base;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkEventData.AtkInputData;

namespace PetRenamer.PetNicknames.KTKWindowing.ControllerNavigation.Interfaces;

internal interface IControllerNavigator
{
    public void RegisterAddon(KTKAddon addonToControl);
    public void UnregisterAddon();

    public bool OnCustomInput(NavigationInputId inputId, InputState inputState);
    public void SetFocus(NavigableComponent ktkComponent);
}
