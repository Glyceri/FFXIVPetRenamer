using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface ITargetManager
{
    IPettableEntity? Target                     { get; }
    IPettableEntity? TargetOfTarget             { get; }
    IPettableEntity? FocusTarget                { get; }
    IPettableEntity? MouseOverTarget            { get; }
    IPettableEntity? PreviousTarget             { get; }
    IPettableEntity? GPoseTarget                { get; }
    IPettableEntity? MouseOverNameplateTarget   { get; }
}
