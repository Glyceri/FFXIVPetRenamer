namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUserTargetManager
{
    IPettableEntity? GetLeadingTarget();
    IPettableEntity? GetLeadingTargetOfLeadingTarget();

    IPettableEntity? GetSoftTarget();
    IPettableEntity? GetTarget();

    IPettableEntity? GetSoftTargetOfLeadingTarget();
    IPettableEntity? GetTargetOfLeadingTarget();

    IPettableEntity? GetTargetOfTarget();
    IPettableEntity? GetSoftTargetOfTarget();

    IPettableEntity? GetTargetOfSoftTarget();
    IPettableEntity? GetSoftTargetOfSoftTarget();
}
