using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IPettableUser : IBattleUser
{
    /// <summary>
    /// Please do NOT set this value unless you know what you need it for
    /// </summary>
    public bool Touched { get; set; }

    void Set(Pointer<BattleChara> pointer);

    void Destroy();
}
