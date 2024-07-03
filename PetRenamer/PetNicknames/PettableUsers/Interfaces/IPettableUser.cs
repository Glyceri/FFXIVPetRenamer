using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUser : IBattleUser
{
    bool IsActive { get; }  
    /// <summary>
    /// Please do NOT set this value unless you know what you need it for
    /// </summary>
    bool Touched { get; set; }
    IPettableDatabaseEntry DataBaseEntry { get; }
    List<IPettablePet> PettablePets { get; }
    IPettablePet? GetPet(nint pet);

    void Set(Pointer<BattleChara> pointer);
    void CalculateBattlepets(ref List<Pointer<BattleChara>> pets);

    void Destroy();
}
