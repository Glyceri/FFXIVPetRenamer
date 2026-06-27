using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.PetNicknames.Hooking.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

internal interface IPronounHook  : IHookableElement
{
    SeString? LastGottenPronoun         { get; }
    SeString? PreviousLastGottenPronoun { get; }
}