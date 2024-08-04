using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

internal interface IMapTooltipHook : IHookableElement
{
    void OverridePet(IPettablePet? pet);
}
