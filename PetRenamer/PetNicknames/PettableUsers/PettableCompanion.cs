using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.IPC.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableCompanion : BasePettablePet, IPettableCompanion
{
    public Companion* Companion { get => (Companion*)Address; }

    public PettableCompanion(Companion* companion, IPettableUser owner, ISharingDictionary sharingDictionary, IPettableDatabaseEntry entry, IPetServices petServices) : base(&companion->Character, owner, sharingDictionary, entry, petServices) { }
}
