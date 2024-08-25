using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IPettableUser : IBattleUser
{
    bool IsActive { get; }  
    bool IsLocalPlayer { get; }
    bool IsDirty { get; }

    IPettableDatabaseEntry DataBaseEntry { get; }
    List<IPettablePet> PettablePets { get; }
    IPettablePet? GetPet(nint pet);
    IPettablePet? GetPet(GameObjectId gameObjectId);
    IPettablePet? GetYoungestPet(PetFilter filter = PetFilter.None);
    
    string? GetCustomName(IPetSheetData sheetData);

    void OnLastCastChanged(uint cast);
    void Update();
    void SetBattlePet(BattleChara* battlePet);
    void RemoveBattlePet(BattleChara* battlePet);
    void SetCompanion(Companion* companion);
    void RemoveCompanion(Companion* companion);
    void RefreshCast();
    void Dispose(IPettableDatabase database);

    enum PetFilter
    {
        None,
        Minion,
        BattlePet,
        Chocobo
    }
}
