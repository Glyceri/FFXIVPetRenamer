using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;

namespace PetRenamer.PetNicknames.KTKWindowing.Interfaces;

internal interface ICustomInput
{
    public bool OnCustomGuideInput(NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState);
}
