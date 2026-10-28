using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface ITargetManager
{
    IPettableEntity? Target                       { get; }
    IPettableEntity? SoftTarget                   { get; }
    IPettableEntity? LeadingTarget                { get; }  
    IPettableEntity? TargetOfTarget               { get; }
    IPettableEntity? LeadingTargetOfTarget        { get; }
    IPettableEntity? LeadingTargetOfLeadingTarget { get; }
    IPettableEntity? TargetOfLeadingTarget        { get; }
    IPettableEntity? FocusTarget                  { get; }
    IPettableEntity? MouseOverTarget              { get; }
    IPettableEntity? PreviousTarget               { get; }
    IPettableEntity? GPoseTarget                  { get; }
    IPettableEntity? MouseOverNameplateTarget     { get; }

    
    IPettableEntity? GetLeadingTarget(IPettableBattleEntity? battleEntity);
    IPettableEntity? GetLeadingTargetOfLeadingTarget(IPettableBattleEntity? battleEntity);

    IPettableEntity? GetSoftTarget(IPettableBattleEntity? battleEntity);
    IPettableEntity? GetTarget(IPettableBattleEntity? battleEntity);

    IPettableEntity? GetSoftTargetOfLeadingTarget(IPettableBattleEntity? battleEntity);
    IPettableEntity? GetTargetOfLeadingTarget(IPettableBattleEntity? battleEntity);

    IPettableEntity? GetTargetOfTarget(IPettableBattleEntity? battleEntity);
    IPettableEntity? GetSoftTargetOfTarget(IPettableBattleEntity? battleEntity);

    IPettableEntity? GetTargetOfSoftTarget(IPettableBattleEntity? battleEntity);
    IPettableEntity? GetSoftTargetOfSoftTarget(IPettableBattleEntity? battleEntity);
}
