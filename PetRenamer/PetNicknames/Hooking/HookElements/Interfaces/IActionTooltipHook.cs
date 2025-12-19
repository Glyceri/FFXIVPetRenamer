using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

internal interface IActionTooltipHook : IHookableElement
{
    public IPetSheetData? CurrentlyHoveredPet { get; }
}
