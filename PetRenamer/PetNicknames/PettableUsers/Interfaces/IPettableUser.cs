using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUser : IBattleUser
{
    /// <summary>
    /// Please do NOT set this value unless you know what you need it for
    /// </summary>
    public bool Touched { get; set; }
    public IPettableDatabaseEntry DataBaseEntry { get; }
    public List<IPettablePet> PettablePets { get; }

    void Set(Pointer<BattleChara> pointer);
    void CalculateBattlepets(ref List<Pointer<BattleChara>> pets);

    void Destroy();
}
