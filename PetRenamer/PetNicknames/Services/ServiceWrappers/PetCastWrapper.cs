using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetCastWrapper : IPetCastHelper
{
    public unsafe BattleChara* LastCastTarget { get; private set; }
    public unsafe BattleChara* LastCastDealer { get; private set; }
    public int LastCastID { get; private set; }

    public unsafe void SetLatestCast(BattleChara* target, BattleChara* dealer, int lastCastID)
    {
        LastCastTarget = target;
        LastCastDealer = dealer;
        LastCastID = lastCastID;
    }
}
