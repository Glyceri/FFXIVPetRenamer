namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IBattleUser : IPettableBattleEntity
{
    uint CurrentCastId { get; }
}
