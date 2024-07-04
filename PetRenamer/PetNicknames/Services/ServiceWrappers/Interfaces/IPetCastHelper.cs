using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IPetCastHelper
{
    BattleChara* LastCastTarget { get; }
    BattleChara* LastCastDealer { get; }
    int LastCastID { get; }

    void SetLatestCast(BattleChara* target, BattleChara* dealer, int lastCastID);
}
