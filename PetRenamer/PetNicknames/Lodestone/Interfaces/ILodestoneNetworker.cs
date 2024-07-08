using PetRenamer.PetNicknames.Lodestone.Structs;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Lodestone.Interfaces;

internal interface ILodestoneNetworker
{
    ILodestoneQueueElement SearchCharacter(IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, LodestoneSearchData> success, Action<Exception> failure);
}
