using PetRenamer.PetNicknames.Hooking.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

internal interface IActionTooltipHook : IHookableElement
{
    uint LastActionID { get; }
}
