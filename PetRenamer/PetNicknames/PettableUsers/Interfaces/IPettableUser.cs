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
    /// <summary>
    /// Please do NOT set this value unless you know what you need it for
    /// </summary>
    bool Touched { get; set; }
    nint User { get; }
    bool IsLocalPlayer { get; }
    bool IsDirty { get; }
    bool Destroyed { get; }

    IPettableDatabaseEntry DataBaseEntry { get; }
    List<IPettablePet> PettablePets { get; }
    IPettablePet? GetPet(nint pet);
    IPettablePet? GetPet(GameObjectId gameObjectId);
    IPettablePet? GetYoungestPet(PetFilter filter = PetFilter.None);
    
    string? GetCustomName(IPetSheetData sheetData);

    void OnLastCastChanged(uint cast);
    void Set(Pointer<BattleChara> pointer);
    void CalculateBattlepets(ref List<Pointer<BattleChara>> pets);
    void NotifyOfDirty();

    void Destroy();

    enum PetFilter
    {
        None,
        Minion,
        BattlePet,
        Chocobo
    }
}
