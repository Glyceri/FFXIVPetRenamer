using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableUsers;

internal unsafe class PettableCompanion : IPettableCompanion
{
    public Companion* Companion { get; init; }

    public PettableCompanion(Companion* c)
    {
        Companion = c;
    }

    
}
