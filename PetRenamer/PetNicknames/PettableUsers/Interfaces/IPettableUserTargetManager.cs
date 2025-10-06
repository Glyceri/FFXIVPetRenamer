namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUserTargetManager
{
    public IPettableEntity? GetLeadingTarget();
    public IPettableEntity? GetLeadingTargetOfLeadingTarget();

    public IPettableEntity? GetSoftTarget();
    public IPettableEntity? GetTarget();

    public IPettableEntity? GetSoftTargetOfLeadingTarget();
    public IPettableEntity? GetTargetOfLeadingTarget();

    public IPettableEntity? GetTargetOfTarget();
    public IPettableEntity? GetSoftTargetOfTarget();

    public IPettableEntity? GetTargetOfSoftTarget();
    public IPettableEntity? GetSoftTargetOfSoftTarget();
}
