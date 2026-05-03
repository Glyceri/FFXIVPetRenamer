using PetRenamer.PetNicknames.Hooking.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

internal interface IPronounHook  : IHookableElement
{
    string? LastGottenPronoun         { get; }
    string? PreviousLastGottenPronoun { get;}
}