using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Update.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface ITargetManager : IUpdatable
{
    public IPettableEntity? Target                       { get; }
    public IPettableEntity? SoftTarget                   { get; }
    public IPettableEntity? LeadingTarget                { get; }  
    public IPettableEntity? TargetOfTarget               { get; }
    public IPettableEntity? LeadingTargetOfTarget        { get; }
    public IPettableEntity? LeadingTargetOfLeadingTarget { get; }
    public IPettableEntity? TargetOfLeadingTarget        { get; }
    public IPettableEntity? FocusTarget                  { get; }
    public IPettableEntity? MouseOverTarget              { get; }
    public IPettableEntity? PreviousTarget               { get; }
    public IPettableEntity? GPoseTarget                  { get; }
    public IPettableEntity? MouseOverNameplateTarget     { get; }

    public void RegisterTargetChangedListener(Action callback);
    public void DeregisterTargetChangedListener(Action callback);
}
