namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IBattleUser : IPettableBattleEntity
{
    string       Name          { get; }
    ulong        ContentId     { get; }
    ushort       Homeworld     { get; }
    uint         CurrentCastId { get; }
}
