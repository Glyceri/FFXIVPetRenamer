using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableCompanion : BasePettablePet, IPettableCompanion
{
    public Companion* Companion { get => (Companion*)PetPointer; }

    public PettableCompanion(Companion* companion, in IPettableUser owner, in ISharingDictionary sharingDictionary, in IPettableDatabaseEntry entry, in IPetServices petServices) : base(&companion->Character, in owner, in sharingDictionary, in entry, in petServices)
    {

    }
}
