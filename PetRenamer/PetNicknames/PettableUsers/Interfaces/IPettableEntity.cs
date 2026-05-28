using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableEntity
{
    nint         Address  { get; }
    GameObjectId ObjectId { get; }
}
