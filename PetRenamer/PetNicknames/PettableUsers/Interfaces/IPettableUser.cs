using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUser : IBattleUser
{
    bool IsActive { get; }  
    nint User { get; }
    bool IsLocalPlayer { get; }
    bool IsDirty { get; }

    IPettableDatabaseEntry DataBaseEntry { get; }
    List<IPettablePet> PettablePets { get; }
    IPettablePet? GetPet(nint pet);
    IPettablePet? GetPet(GameObjectId gameObjectId);
    IPettablePet? GetYoungestPet(PetFilter filter = PetFilter.None);
    
    string? GetCustomName(IPetSheetData sheetData);

    void OnLastCastChanged(uint cast);
    void Set(Pointer<BattleChara> pointer);
    void CalculateBattlepets(ref List<Pointer<BattleChara>> pets);
    void RefreshCast();

    enum PetFilter
    {
        None,
        Minion,
        BattlePet,
        Chocobo
    }
}
